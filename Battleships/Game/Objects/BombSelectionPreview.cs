using System.Numerics;
using System.Runtime.CompilerServices;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// The preview for the spot we're going to be bombing on.
    /// </summary>
    internal class BombSelectionPreview : GameObject,
        IDrawableGameObject
    {
        /// <summary>
        /// The playfield this selection is tied to.
        /// </summary>
        public ShipPlayfield? Playfield { get; set; }

        /// <summary>
        /// The position of the bomb selection.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Did we hit anything?
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

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                Playfield!.SendBombField(point.Value);

            _hit = true;
        }

        /// <inheritdoc/>
        public void Draw()
        {
            if (!_hit)
                return;

            Raylib.DrawCube(Position, 1f, 1f, 1f, new Color(255, 0, 0, 128));
        }
    }
}
