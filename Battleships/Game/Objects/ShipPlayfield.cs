using System.Numerics;
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
        /// The field.
        /// </summary>
        private readonly CubeRenderer[,] _field;

        /// <summary>
        /// The preview cube.
        /// </summary>
        private CubeRenderer? _previewCube;

        /// <summary>
        /// The camera.
        /// </summary>
        private Camera? _camera;

        /// <summary>
        /// Construct a new ship playfield.
        /// </summary>
        public ShipPlayfield()
        {
            _field = new CubeRenderer[10, 10];
        }

        public void CreateCube(int x, int y)
        {
            if (_field[x, y] != null)
                return;

            var cube = ThisGame!.AddGameObject<CubeRenderer>();
            cube.Position = new Vector3(x - 5 + 0.5f, 0.5f, y - 5 + 0.5f);

            _field[x, y] = cube;
        }

        /// <inheritdoc/>
        public override void Start()
        {
            _previewCube = ThisGame!.AddGameObject<CubeRenderer>();
            _camera = GetGameObjectFromGame<Camera>();

            Peer?.MessageRegistry.RegisterMessage<CreateCubeMessage>(mesg =>
            {
                var cubeMesg = (CreateCubeMessage)mesg;

                CreateCube(cubeMesg.x, cubeMesg.y);
            });
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            var collision = ThisGame!.CastRay(_camera!.MouseRay(Raylib.GetMousePosition()));
            if (collision == null)
                return;

            // Translate this hit to the square
            var point = collision.Value.point;
            point.X = MathF.Floor(point.X);
            point.Z = MathF.Floor(point.Z);

            _previewCube!.Position = new Vector3(point.X + 0.5f, 0.5f, point.Z + 0.5f);

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                var x = (int)point.X + 5;
                var y = (int)point.Z + 5;

                if (_field[x, y] != null)
                    return;

                CreateCube(x, y);

                Peer!.Send(new CreateCubeMessage
                {
                    x = x,
                    y = y
                }, Framework.Networking.SendMode.Extra);
            }
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
