using System.Numerics;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A preview for the ship.
    /// </summary>
    internal class ShipBuilderPreview : GameObject,
        IDrawableGameObject
    {
        /// <summary>
        /// The playfield this is tied to.
        /// </summary>
        public ShipPlayfield? Playfield { get; set; }

        /// <summary>
        /// The length of the ship.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The facing of the ship.
        /// </summary>
        public Ship.Facing Facing { get; set; }

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Did we hit the playfield with a raycast the last time?
        /// </summary>
        private bool _hit;

        /// <summary>
        /// The camera.
        /// </summary>
        private Camera? _camera;

        /// <inheritdoc/>
        public override void Start()
        {
            _camera = GetGameObjectFromGame<Camera>();
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            // If we've pressed R, flip the facing.
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))
                Facing = Facing == Ship.Facing.Down ? Ship.Facing.Right : Ship.Facing.Down;

            var collision = ThisGame!.CastRay(_camera!.MouseRay(Raylib.GetMousePosition()));
            if (collision == null)
            {
                _hit = false;
                return;
            }

            var point = Playfield!.PositionToFieldCoordinates(collision.Value.point);
            if (point == null)
            {
                _hit = false;
                return;
            }

            var floorPoint = collision.Value.point;
            floorPoint.X = MathF.Floor(floorPoint.X) + 0.5f;
            floorPoint.Z = MathF.Floor(floorPoint.Z) + 0.5f;
            floorPoint.Y = 0.5f;

            Position = floorPoint;

            if (!Playfield!.CanBuildShipAt(point.Value, Length, Facing))
            {
                _hit = false;
                return;
            }

            _hit = true;

            // If we've pressed mouse left, construct the ship.
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                Playfield!.BuildShip(point.Value, Length, Facing);
        }

        /// <inheritdoc/>
        public void Draw()
        {
            var color = Color.VIOLET;
            if (!_hit)
                color = Color.RED;

            for (var i = 0; i < Length; i++)
            {
                var pos = Position;
                if (Facing == Ship.Facing.Down)
                    pos.Z += i;
                else
                    pos.X += i;

                Raylib.DrawCube(pos, 1f, 1f, 1f, color);
            }
        }
    }
}
