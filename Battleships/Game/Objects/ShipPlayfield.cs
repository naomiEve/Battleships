using System.Numerics;
using System.Runtime.CompilerServices;
using Battleships.Framework.Data;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
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
        /// Construct a new ship playfield.
        /// </summary>
        public ShipPlayfield()
        {
            _field = new ShipPart[FIELD_SIZE, FIELD_SIZE];
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

            _coordinator?.SetState(Data.GameState.Waiting);
        }

        /// <summary>
        /// Tries to bomb a field at the given coordinates.
        /// </summary>
        /// <param name="position">The position.</param>
        public void TryBombField(Vector2Int position)
        {
            if (position.X < 0 || position.X >= FIELD_SIZE ||
                position.Y < 0 || position.Y >= FIELD_SIZE)
            {
                Peer?.Send(new BombingResultMessage
                {
                    hit = false
                }, passLockstep: false);

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            var part = _field[position.X, position.Y];
            if (part == null)
            {
                Peer?.Send(new BombingResultMessage
                {
                    hit = false
                }, passLockstep: false);
                
                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            if (part.Sunk)
            {
                Peer?.Send(new BombingResultMessage
                {
                    hit = false
                }, passLockstep: false);

                _coordinator?.SetBomber(Peer!.PeerId!.Value, true);
                return;
            }

            part.Sink();

            var unsunk = GetUnsunkPieces();

            Peer?.Send(new BombingResultMessage
            {
                hit = true
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
        /// Gets the amount of unsunk pieces.
        /// </summary>
        /// <returns>The unsunk pieces.</returns>
        private int GetUnsunkPieces()
        {
            var count = 0;
            for (var x = 0; x < FIELD_SIZE; x++)
                for (var y = 0; y < FIELD_SIZE; y++)
                    count += (_field[x, y]?.Sunk == false) ? 1 : 0;

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
            part.Position = new Vector3(Position.X + x - 5 + 0.5f, 0.5f, Position.Y + y - 5 + 0.5f);

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
            var spacing = 1f;
            var halfSlices = 5;

            Rlgl.rlBegin(DrawMode.LINES);
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
