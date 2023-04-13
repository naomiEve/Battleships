using System.Numerics;
using Battleships.Framework.Data;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
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
        /// The camera.
        /// </summary>
        private Camera? _camera;

        /// <summary>
        /// The owner of this playfield.
        /// </summary>
        public int Owner { get; set; }

        /// <summary>
        /// Construct a new ship playfield.
        /// </summary>
        public ShipPlayfield()
        {
            _field = new ShipPart[FIELD_SIZE, FIELD_SIZE];
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
            part.Position = new Vector3(x - 5 + 0.5f, 0.5f, y - 5 + 0.5f);

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
            vector.X = MathF.Floor(vector.X);
            vector.Z = MathF.Floor(vector.Z);

            var x = (int)vector.X + 5;
            var y = (int)vector.Z + 5;

            if (x < 0 || x >= FIELD_SIZE ||
                y < 0 || y >= FIELD_SIZE)
                return null;

            return new Vector2Int(x, y);
        }

        /// <inheritdoc/>
        public override void Start()
        {
            _shipPreview = ThisGame!.AddGameObject<ShipBuilderPreview>();
            _shipPreview!.Playfield = this;
            _shipPreview.Length = 3;

            _camera = GetGameObjectFromGame<Camera>();

            Peer?.MessageRegistry.RegisterMessage<CreateCubeMessage>(mesg =>
            {
                var cubeMesg = (CreateCubeMessage)mesg;
                CreateShipPart(cubeMesg.x, cubeMesg.y, null!);
            });

            Peer?.MessageRegistry.RegisterMessage<BombFieldMessage>(mesg =>
            {
                var bombMesg = (BombFieldMessage)mesg;
                _field[bombMesg.x, bombMesg.y]?.Sink();
            });
        }

        /// <inheritdoc/>
        public void Draw()
        {
            Raylib.DrawGrid(10, 1.0f);
        }

        /// <inheritdoc/>
        public RayCollision Collide(Ray ray)
        {
            return Raylib.GetRayCollisionQuad(ray, new Vector3(-5f, 0f, -5f), new Vector3(-5f, 0f, 5f), new Vector3(5f, 0f, 5f), new Vector3(5f, 0f, -5f)); ;
        }
    }
}
