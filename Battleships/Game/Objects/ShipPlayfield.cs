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
        /// The offset from the top right of the tile to the center.
        /// </summary>
        public const float HALF_TILE_OFFSET = 0.5f;

        /// <summary>
        /// The field.
        /// </summary>
        private readonly ShipPart[,] _field;

        /// <summary>
        /// The field containing debris.
        /// </summary>
        private readonly GameObject[,] _debrisField;

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
            _debrisField = new GameObject[FIELD_SIZE, FIELD_SIZE];
        }

        /// <summary>
        /// Converts a position ([1, 1]) to a letter-number pair (A1).
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The pair.</returns>
        private static string FieldToLetterNumberPair(Vector2Int position)
        {
            return $"{(char)('A' + position.X)}{position.Y + 1}";
        }

        /// <summary>
        /// Clears the playfield.
        /// </summary>
        public void ClearPlayfield()
        {
            for (var x = 0; x < FIELD_SIZE; x++)
            {
                for (var y = 0; y < FIELD_SIZE; y++)
                {
                    if (_field[x, y] != null)
                        ThisGame!.RemoveGameObject(_field[x, y]);

                    if (_debrisField[x, y] != null)
                        ThisGame!.RemoveGameObject(_debrisField[x, y]);

                    _field[x, y] = null!;
                    _debrisField[x, y] = null!;
                }
            }

            FinishedBuilding = false;
            ShipCounter!.ShipsLeft = GameCoordinator.ShipCount;
        }

        /// <summary>
        /// Does the field bombing cinematic.
        /// </summary>
        /// <param name="position">The position to bomb.</param>
        public async void DoBombFieldCinematic(Vector2Int position)
        {
            GetGameObjectFromGame<GameLog>()!
                .AddMessageToLog($"You attempted to shoot {FieldToLetterNumberPair(position)}.");

            // First, select a random cannon to fire from on our field.
            var ourField = GetGameObjectFromGame<GameCoordinator>()!
                .GetPlayfieldForPlayer((Owner + 1) % GameCoordinator.PLAYER_COUNT)!;

            var cannon = ourField._field
                            .Flatten()
                            .Where(part => part is not null && part.Cannon is not null)
                            .Where(part => !part.Underwater)
                            .Select(part => part.Cannon)
                            .RandomElement();

            // Now, pan the camera to our field.
            _camera!.Objective = CameraObjective.MoveToSelf;

            await AsyncHelper.While(() => _camera.Objective != CameraObjective.Idle);

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
        /// Does the field have debris at a position?
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Whether it has debris there.</returns>
        public bool HasDebrisAt(Vector2Int position)
        {
            if (position.X < 0 || position.X >= FIELD_SIZE ||
                position.Y < 0 || position.Y >= FIELD_SIZE)
            {
                return true;
            }

            return _debrisField[position.X, position.Y] != null;
        }

        /// <summary>
        /// Does the field have a ship at a position?
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Whether it has a ship there.</returns>
        public bool HasShipAt(Vector2Int position)
        {
            if (position.X < 0 || position.X >= FIELD_SIZE ||
                position.Y < 0 || position.Y >= FIELD_SIZE)
            {
                return true;
            }

            return _field[position.X, position.Y] != null;
        }

        /// <summary>
        /// Sends a bombing hit result back to the opponent.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="passLockstep">Should we pass the lockstep?</param>
        /// <param name="part">The part, if any.</param>
        private void SendBombingHitResult(Vector2Int position, bool passLockstep, ShipPart? part = null)
        {
            var hit = part is not null;

            GetGameObjectFromGame<GameLog>()!
                .AddMessageToLog($"Opponent fired at {FieldToLetterNumberPair(position)} and {(hit ? "hit a ship" : "missed")}.");

            Peer?.Send(new BombingResultMessage
            {
                hit = hit,
                x = position.X,
                y = position.Y,
                field = Owner,
                facing = hit ? part!.Ship!.ShipFacing : Ship.Facing.Down
            }, passLockstep: passLockstep);
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
                SendBombingHitResult(position, false);

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            var part = _field[position.X, position.Y];
            if (part == null)
            {
                SpawnBuoyAt(position);

                SendBombingHitResult(position, false);

                // Wait a while before we move the fields again, so we can display the particle effect.
                await Task.Delay(TimeSpan.FromSeconds(1));

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            if (part.Hit)
            {
                SendBombingHitResult(position, false);

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            part.Hit = true;
            SpawnFireAt(position, part);

            // Check if we have any more of this ship's parts alive.
            var shipPartsLeft = part.Ship?
                .Parts.Count(part => !part.Hit);

            if (shipPartsLeft.HasValue &&
                shipPartsLeft < 1)
            {
                var ship = part.Ship!;

                var playSound = true;
                foreach (var shipPart in ship.Parts)
                {
                    shipPart!.Sink();

                    // Spawn debris on each of the fields.
                    SpawnShipDebrisAt(PositionToFieldCoordinates(shipPart!.InitialPosition)!.Value, playSound);
                    playSound = false;
                }
                
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
            SendBombingHitResult(position, unsunk > 0, part);

            // If we have no more afloat pieces, send the field cleared message.
            if (unsunk == 0)
            {
                Peer?.Send(new FieldClearedMessage
                {
                    id = Peer!.PeerId!.Value
                }, passLockstep: !Peer!.IsHost);
            }
        }

        /// <summary>
        /// Spawns ship debris at a position.
        /// </summary>
        /// <param name="position">The position.</param>
        public void SpawnShipDebrisAt(Vector2Int position, bool playSound = true)
        {
            // Dirty hack.
            if (HasDebrisAt(position) && _debrisField[position.X, position.Y] is Buoy)
                ThisGame!.RemoveGameObject(_debrisField[position.X, position.Y]);

            var pos = FieldCoordinatesToPosition(position);

            var debris = ThisGame!.AddGameObject<ShipDebris>();
            debris.Position = pos;
            debris.Playfield = this;
            debris.FloatUp();

            SpawnFireAt(position, debris, playSound);

            _debrisField[position.X, position.Y] = debris;
        }

        /// <summary>
        /// Spawns an explosion particle effect at a given position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="followed">The object to follow.</param>
        /// <param name="playSound">Whether we should play the explosion sound while doing so.</param>
        public void SpawnFireAt(Vector2Int position, IPositionedObject followed = null!, bool playSound = true)
        {
            var pos = FieldCoordinatesToPosition(position);
            pos.Y += 0.3f;

            ThisGame!.AddGameObject<ParticleEffect>()
                .WithPosition(pos)
                .WithAtlas(ThisGame.AssetDatabase.Get<TextureAsset>("fire_atlas")!)
                .WithDuration(1f)
                .WithLooping(true)
                .Following(followed, new Vector3(0, 0.4f, 0))
                .Fire();

            if (playSound)
            {
                ThisGame!.AssetDatabase
                    .Get<SoundAsset>("explosion")?
                    .Play();
            }
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
            if (HasDebrisAt(position))
                return;

            if (HasShipAt(position))
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

            _debrisField[position.X, position.Y] = buoy;
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

            var begin = new Vector3(Position.X + x - 5 + HALF_TILE_OFFSET,   5f, Position.Y + y - 5 + HALF_TILE_OFFSET);
            var end   = new Vector3(Position.X + x - 5 + HALF_TILE_OFFSET, 0.5f, Position.Y + y - 5 + HALF_TILE_OFFSET);
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

            return new Vector3(x + HALF_TILE_OFFSET, 0f, y + HALF_TILE_OFFSET) + Position;
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

            for (var x = min.X; x <= max.X; x++)
            {
                for (var y = min.Y; y <= max.Y; y++)
                {
                    if (x < 0 || x >= FIELD_SIZE || 
                        y < 0 || y >= FIELD_SIZE)
                        continue;

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
            const float color = 0.75f;
            const float lineWidth = 3f;

            Rlgl.rlBegin(DrawMode.LINES);
            Rlgl.rlSetLineWidth(lineWidth);

            for (var i = -halfSlices; i <= halfSlices; i++)
            {
                Rlgl.rlColor3f(color, color, color);
                Rlgl.rlColor3f(color, color, color);
                Rlgl.rlColor3f(color, color, color);
                Rlgl.rlColor3f(color, color, color);

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
            // Every tile has size 1, so from the center we have 5 tiles to the left and right.
            const float halfTiles = FIELD_SIZE / 2f;

            return Raylib.GetRayCollisionQuad(ray,
                new Vector3(Position.X - halfTiles, 0f, Position.Z - halfTiles),
                new Vector3(Position.X - halfTiles, 0f, Position.Z + halfTiles),
                new Vector3(Position.X + halfTiles, 0f, Position.Z + halfTiles),
                new Vector3(Position.X + halfTiles, 0f, Position.Z - halfTiles)
            );
        }
    }
}
