using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Data;
using Battleships.Framework.Extensions;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Tweening;
using Battleships.Game.Data;
using Battleships.Game.Messages;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A playfield for battleships.
    /// </summary>
    internal class ShipPlayfield : GameObject,
        IDrawableGameObject,
        IRaycastTargettableObject
    {
        /// <summary>
        /// The size of the field.
        /// </summary>
        private const int FIELD_SIZE = 10;

        /// <summary>
        /// The field.
        /// </summary>
        private readonly ShipPart[,] _field;

        /// <summary>
        /// The field containing buoys.
        /// </summary>
        private readonly Buoy[,] _buoyField;

        /// <summary>
        /// The preview cube.
        /// </summary>
        private ShipBuilderPreview? _shipPreview;

        /// <summary>
        /// The game coordinator.
        /// </summary>
        private GameCoordinator? _coordinator;

        /// <summary>
        /// The camera.
        /// </summary>
        private CameraController? _camera;

        /// <summary>
        /// The owner of this playfield.
        /// </summary>
        public int Owner { get; set; }

        /// <summary>
        /// Is this playfield finished?
        /// </summary>
        public bool FinishedBuilding { get; set; }

        /// <summary>
        /// The position of the grid.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The bomb selection preview.
        /// </summary>
        public BombSelectionPreview? BombPreview { get; set; }

        /// <summary>
        /// The ship counter.
        /// </summary>
        public ShipCounter? ShipCounter { get; set; }

        /// <summary>
        /// Construct a new ship playfield.
        /// </summary>
        public ShipPlayfield()
        {
            _field = new ShipPart[FIELD_SIZE, FIELD_SIZE];
            _buoyField = new Buoy[FIELD_SIZE, FIELD_SIZE];
        }

        /// <summary>
        /// Does the field bombing cinematic.
        /// </summary>
        /// <param name="position">The position to bomb.</param>
        public async void DoBombFieldCinematic(Vector2Int position)
        {
            // First, select a random cannon to fire from on our field.
            var ourField = GetGameObjectFromGame<GameCoordinator>()!
                .GetPlayfieldForPlayer((Owner + 1) % 2)!;

            var cannon = ourField._field
                            .Flatten()
                            .Where(part => part is not null && part.Cannon is not null)
                            .Where(part => !part.Underwater)
                            .Select(part => part.Cannon)
                            .RandomElement();

            // Now, pan the camera to our field.
            _camera!.Objective = CameraObjective.MoveToSelf;

            Console.WriteLine(_camera!.Objective);
            await AsyncHelper.While(() => _camera.Objective != CameraObjective.Idle);
            Console.WriteLine(_camera!.Objective);

            // Wait just a while more.
            await Task.Delay(TimeSpan.FromMilliseconds(1000));

            // Spawn an explosion at the cannon's position.
            cannon?.PlayShooting();

            // Wait again.
            await Task.Delay(TimeSpan.FromMilliseconds(800));

            // Go back.
            _camera!.Objective = CameraObjective.MoveToEnemy;
            await AsyncHelper.While(() => _camera.Objective != CameraObjective.Idle);
            
            SendBombField(position);
        }

        /// <summary>
        /// Tries to bomb a field.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SendBombField(Vector2Int position)
        {
            Peer?.Send(new BombFieldMessage
            {
                x = position.X,
                y = position.Y,
                field = Owner
            });

            _coordinator?.SetState(GameState.Waiting);
        }

        /// <summary>
        /// Does the field have a buoy at a position?
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Whether it has a buoy there.</returns>
        public bool HasBuoyAt(Vector2Int position)
        {
            if (position.X < 0 || position.X >= FIELD_SIZE ||
                position.Y < 0 || position.Y >= FIELD_SIZE)
            {
                return true;
            }

            return _buoyField[position.X, position.Y] != null;
        }

        /// <summary>
        /// Tries to bomb a field at the given coordinates.
        /// </summary>
        /// <param name="position">The position.</param>
        public async void TryBombField(Vector2Int position)
        {
            if (position.X < 0 || position.X >= FIELD_SIZE ||
                position.Y < 0 || position.Y >= FIELD_SIZE)
            {
                Peer?.Send(new BombingResultMessage
                {
                    hit = false,
                    x = position.X,
                    y = position.Y,
                    field = Owner
                }, passLockstep: false);

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            var part = _field[position.X, position.Y];
            if (part == null)
            {
                SpawnBuoyAt(position);

                Peer?.Send(new BombingResultMessage
                {
                    hit = false,
                    x = position.X,
                    y = position.Y,
                    field = Owner
                }, passLockstep: false);

                // Wait a while before we move the fields again, so we can display the particle effect.
                await Task.Delay(TimeSpan.FromSeconds(1));

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            if (part.Hit)
            {
                Peer?.Send(new BombingResultMessage
                {
                    hit = false,
                    x = position.X,
                    y = position.Y,
                    field = Owner
                }, passLockstep: false);

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            part.Hit = true;
            SpawnFireAt(position);

            // Check if we have any more of this ship's parts alive.
            var shipPartsLeft = part.Ship?
                .Parts.Count(part => !part.Hit);

            if (shipPartsLeft.HasValue &&
                shipPartsLeft < 1)
            {
                var ship = part.Ship!;

                foreach (var shipPart in ship.Parts)
                    shipPart?.Sink();

                SurroundSunkShipWithBuoys(ship.Position, ship.ShipFacing, ship.Length);

                Peer?.Send(new ShipSunkMessage
                {
                    playfield = Owner,
                    x = ship.Position.X,
                    y = ship.Position.Y,
                    facing = ship.ShipFacing,
                    length = ship.Length
                }, passLockstep: false);
            }

            var unsunk = GetUnsunkPieces();
            Peer?.Send(new BombingResultMessage
            {
                hit = true,
                x = position.X,
                y = position.Y,
                field = Owner
            }, passLockstep: unsunk > 0);

            // If we have no more afloat pieces, send the field cleared message.
            if (unsunk == 0)
            {
                Peer?.Send(new FieldClearedMessage
                {
                    id = Peer!.PeerId!.Value
                });
            }
        }

        /// <summary>
        /// Spawns an explosion particle effect at a given position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SpawnFireAt(Vector2Int position)
        {
            var pos = FieldCoordinatesToPosition(position);
            pos.Y += 0.3f;

            ThisGame!.AddGameObject<ParticleEffect>()
                .WithPosition(pos)
                .WithAtlas(ThisGame.AssetDatabase.Get<TextureAsset>("fire_atlas")!)
                .WithDuration(1f)
                .WithLooping(true)
                .Following(_field[position.X, position.Y], new Vector3(0, 0.4f, 0))
                .Fire();

            ThisGame!.AssetDatabase
                .Get<SoundAsset>("explosion")?
                .Play();
        }

        /// <summary>
        /// Spawns a water splash at a position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="playSound">Should we play the sound?</param>
        public void SpawnSplashAt(Vector2Int position, bool playSound = true)
        {
            ThisGame!.AddGameObject<ParticleEffect>()
                .WithPosition(FieldCoordinatesToPosition(position))
                .WithAtlas(ThisGame.AssetDatabase.Get<TextureAsset>("splash_atlas")!)
                .WithDuration(0.3f)
                .Fire();

            if (playSound)
            {
                ThisGame!
                    .AssetDatabase.Get<SoundAsset>("splash")!
                    .Play();
            }
        }

        /// <summary>
        /// Spawns a buoy (and a splash) at a position.
        /// </summary>
        /// <param name="position">The position to spawn at.</param>
        /// <param name="playSound">Should we play the splash sound?</param>
        public void SpawnBuoyAt(Vector2Int position, bool playSound = true)
        {
            if (HasBuoyAt(position))
                return;

            var buoy = ThisGame!.AddGameObject<Buoy>();
            var begin = FieldCoordinatesToPosition(position);
            begin.Y = 5f;

            var end = begin;
            end.Y = 0f;

            var playedSound = false;

            new Tween<Vector3>()
                .WithBeginningValue(begin)
                .WithEndingValue(end)
                .WithEasing(TimeEasing.OutElastic)
                .WithTime(1f + Random.Shared.NextSingle() * 2f)
                .WithIncrementer((a, b, t) =>
                {
                    if (t >= 1 && !playedSound)
                    {
                        SpawnSplashAt(position, playSound);
                        playedSound = true;
                    }

                    return a.LinearInterpolation(b, t);
                })
                .WithUpdateCallback(pos => buoy.Position = pos)
                .WithFinishedCallback(fin => buoy.Position = fin)
                .BindTo(GetGameObjectFromGame<TweenEngine>()!);

            _buoyField[position.X, position.Y] = buoy;
        }

        /// <summary>
        /// Gets the amount of unsunk pieces.
        /// </summary>
        /// <returns>The unsunk pieces.</returns>
        private int GetUnsunkPieces()
        {
            var count = 0;
            for (var x = 0; x < FIELD_SIZE; x++)
                for (var y = 0; y < FIELD_SIZE; y++)
                    count += (_field[x, y]?.Hit == false) ? 1 : 0;

            return count;
        }

        /// <summary>
        /// Creates a ship part at the desired x & y coordinates of the playfield.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="parent">The parent ship.</param>
        /// <returns>The created ship part.</returns>
        public ShipPart CreateShipPart(int x, int y, Ship parent)
        {
            if (_field[x, y] != null)
                return _field[x, y];

            var part = ThisGame!.AddGameObject<ShipPart>();
            part.Ship = parent;

            var begin = new Vector3(Position.X + x - 5 + 0.5f, 5f, Position.Y + y - 5 + 0.5f);
            var end = new Vector3(Position.X + x - 5 + 0.5f, 0.5f, Position.Y + y - 5 + 0.5f);
            var playedSound = false;

            new Tween<Vector3>()
                .WithBeginningValue(begin)
                .WithEndingValue(end)
                .WithEasing(TimeEasing.OutElastic)
                .WithTime(1f)
                .WithIncrementer((a, b, t) =>
                {
                    if (t >= 1 && !playedSound)
                    {
                        SpawnSplashAt(new Vector2Int(x, y));
                        playedSound = true;
                    }

                    return a.LinearInterpolation(b, t);
                })
                .WithUpdateCallback(pos => part.InitialPosition = pos)
                .WithFinishedCallback(fin => part.InitialPosition = fin)
                .BindTo(ThisGame!.GetGameObjectOfType<TweenEngine>()!);

            _field[x, y] = part;
            return part;
        }

        /// <summary>
        /// Returns whether a ship can be built at this location.
        /// </summary>
        /// <param name="position">The position of the first element of this ship.</param>
        /// <param name="length">The length of this ship</param>
        /// <param name="facing">The facing of the ship.</param>
        /// <returns>A bool for whether we can build here.</returns>
        public bool CanBuildShipAt(Vector2Int position, float length, Ship.Facing facing)
        {
            // Helper for checking whether a ship part collides with any neighbors.
            bool CollidesWithNeighbors(Vector2Int position)
            {
                for (var x = position.X - 1; x <= position.X + 1; x++)
                {
                    for (var y = position.Y - 1; y <= position.Y + 1; y++)
                    {
                        if (x == position.X &&
                            y == position.Y)
                        {
                            if (x < 0 || x >= FIELD_SIZE ||
                                y < 0 || y >= FIELD_SIZE)
                            {
                                return true;
                            }
                            continue;
                        }

                        if (x < 0 || x >= FIELD_SIZE ||
                            y < 0 || y >= FIELD_SIZE)
                        {
                            continue;
                        }

                        if (_field[x, y] != null)
                            return true;

                        Raylib.DrawCube(new Vector3(x - 5 + 0.5f, 0.5f, y - 5 + 0.5f), 1f, 1f, 1f, Color.GREEN);
                    }
                }

                return false;
            }

            for (var i = 0; i < length; i++)
            {
                var partPos = position;
                if (facing == Ship.Facing.Down)
                    partPos.Y += i;
                else
                    partPos.X += i;

                if (CollidesWithNeighbors(partPos))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Builds a ship beginning with the given coordinates.
        /// </summary>
        /// <param name="initialPos">The initial position.</param>
        /// <param name="length">The length of the ship.</param>
        /// <param name="facing">The facing of the ship.</param>
        public void BuildShip(Vector2Int initialPos, int length, Ship.Facing facing)
        {
            Console.WriteLine("sheep");

            ThisGame?.AddGameObject<Ship>()!
                .ForPlayfield(this)
                .AtPosition(initialPos)
                .WithLength(length)
                .WithFacing(facing)
                .BuildShip();
        }

        /// <summary>
        /// Transforms a world space position into the playfield coordinates.
        /// </summary>
        /// <param name="vector">The world space position.</param>
        /// <returns>Playfield coordinates.</returns>
        public Vector2Int? PositionToFieldCoordinates(Vector3 vector)
        {
            vector.X = MathF.Floor(vector.X - Position.X);
            vector.Z = MathF.Floor(vector.Z - Position.Z);

            var x = (int)vector.X + 5;
            var y = (int)vector.Z + 5;

            if (x < 0 || x >= FIELD_SIZE ||
                y < 0 || y >= FIELD_SIZE)
                return null;

            return new Vector2Int(x, y);
        }

        /// <summary>
        /// Transforms field coordinates to a position in the world.
        /// </summary>
        /// <param name="field">The field coordinates.</param>
        /// <returns>The position within the world.</returns>
        public Vector3 FieldCoordinatesToPosition(Vector2Int field)
        {
            var x = field.X - 5;
            var y = field.Y - 5;

            return new Vector3(x + 0.5f, 0f, y + 0.5f) + Position;
        }

        /// <summary>
        /// Surrounds a sunk ship with buoys.
        /// </summary>
        /// <param name="beginningPosition">The beginning position.</param>
        /// <param name="facing">The facing of the ship.</param>
        /// <param name="length">The length.</param>
        public void SurroundSunkShipWithBuoys(Vector2Int beginningPosition, Ship.Facing facing, int length)
        {
            // First, mark that we have lost ships.
            ShipCounter!.ShipsLeft -= 1;

            var endingPosition = beginningPosition;
            if (facing == Ship.Facing.Down)
            {
                endingPosition.Y += length;
                endingPosition.X += 1;
            }
            else
            {
                endingPosition.X += length;
                endingPosition.Y += 1;
            }

            beginningPosition.X -= 1;
            beginningPosition.Y -= 1;

            var min = new Vector2Int(
                Math.Min(beginningPosition.X, endingPosition.X),
                Math.Min(beginningPosition.Y, endingPosition.Y)
            );

            var max = new Vector2Int(
                Math.Max(beginningPosition.X, endingPosition.X),
                Math.Max(beginningPosition.Y, endingPosition.Y)
            );

            //SpawnBuoyAt(beginningPosition);
            //SpawnBuoyAt(endingPosition);
            
            for (var x = min.X; x <= max.X; x++)
            {
                for (var y = min.Y; y <= max.Y; y++)
                {
                    if (x < 0 || x >= FIELD_SIZE || 
                        y < 0 || y >= FIELD_SIZE)
                        continue;

                    //if ((x == beginningPosition.X && facing == Ship.Facing.Down) || 
                    //    (y == beginningPosition.Y && facing == Ship.Facing.Right))
                    //    continue;

                    Console.WriteLine($"[x:{x}, y:{y}]");

                    SpawnBuoyAt(new(x, y), (x == min.X && y == min.Y));
                }
            }
        }

        /// <summary>
        /// Sets the length of the preview.
        /// </summary>
        /// <param name="length">The new length.</param>
        public void SetPreviewLength(int length)
        {
            _shipPreview!.Length = length;
        }

        /// <inheritdoc/>
        public override void Start()
        {
            BombPreview = ThisGame!.AddGameObject<BombSelectionPreview>();
            BombPreview.Playfield = this;

            _shipPreview = ThisGame!.AddGameObject<ShipBuilderPreview>();
            _shipPreview!.Playfield = this;
            _shipPreview.Length = 4;

            ShipCounter = ThisGame!.AddGameObject<ShipCounter>();
            ShipCounter.Playfield = this;

            _coordinator = GetGameObjectFromGame<GameCoordinator>();
            _camera = GetGameObjectFromGame<CameraController>();
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            _shipPreview!.Enabled = Owner == Peer!.PeerId &&
                _coordinator?.State == GameState.ShipBuilding &&
                _camera?.Objective == CameraObjective.Idle;

            BombPreview!.Enabled = Owner != Peer!.PeerId &&
                _coordinator?.State == GameState.PlayerBombing &&
                _camera?.Objective == CameraObjective.Idle;
        }

        /// <inheritdoc/>
        public void Draw()
        {
            const float spacing = 1f;
            const int halfSlices = 5;

            Rlgl.rlBegin(DrawMode.LINES);
            Rlgl.rlSetLineWidth(3f);

            for (var i = -halfSlices; i <= halfSlices; i++)
            {
                Rlgl.rlColor3f(0.75f, 0.75f, 0.75f);
                Rlgl.rlColor3f(0.75f, 0.75f, 0.75f);
                Rlgl.rlColor3f(0.75f, 0.75f, 0.75f);
                Rlgl.rlColor3f(0.75f, 0.75f, 0.75f);

                Rlgl.rlVertex3f(Position.X + i * spacing, Position.Y, Position.Z + -halfSlices * spacing);
                Rlgl.rlVertex3f(Position.X + i * spacing, Position.Y, Position.Z + halfSlices * spacing);

                Rlgl.rlVertex3f(Position.X + -halfSlices * spacing, Position.Y, Position.Z + i * spacing);
                Rlgl.rlVertex3f(Position.X + halfSlices * spacing, Position.Y, Position.Z + i * spacing);
            }

            Rlgl.rlEnd();
        }

        /// <inheritdoc/>
        public RayCollision Collide(Ray ray)
        {
            return Raylib.GetRayCollisionQuad(ray,
                new Vector3(Position.X + -5f, 0f, Position.Z + -5f),
                new Vector3(Position.X + -5f, 0f, Position.Z + 5f),
                new Vector3(Position.X + 5f, 0f, Position.Z + 5f),
                new Vector3(Position.X + 5f, 0f, Position.Z + -5f)
            );
        }
    }
}
