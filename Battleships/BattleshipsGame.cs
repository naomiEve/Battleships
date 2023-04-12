using System.Numerics;
using Battleships.Framework;
using Battleships.Framework.Data;
using Battleships.Framework.Rendering;
using Battleships.Messages;
using Raylib_cs;

namespace Battleships
{
    /// <summary>
    /// The game logic for battleships.
    /// </summary>
    internal class BattleshipsGame : NetworkedGame
    {
        /// <summary>
        /// The camera.
        /// </summary>
        private Camera? _camera;

        /// <summary>
        /// The cube renderer.
        /// </summary>
        private CubeRenderer? _cube;

        /// <summary>
        /// Construct a new battleship logic with the given launch options.
        /// </summary>
        /// <param name="opts">The launch options.</param>
        public BattleshipsGame(LaunchOptions opts)
            : base(new Vector2Int(800, 600), "Battleships", opts)
        {
            
        }

        /// <inheritdoc/>
        protected override void RegisterMessages()
        {
            Peer.MessageRegistry.RegisterMessage<TestMessage>(mesg =>
            {
                var testMseg = (TestMessage)mesg;
                Console.WriteLine($"Received a new test message! value={testMseg.value}");
            });
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            _camera = new Camera(new Vector3(0f, 10f, 10f), 10f);
            //_camera.AddShaderPass<PosterizationShaderPass>();
            //_camera.AddShaderPass<PixelizationShaderPass>();
            _camera.Rotate(new Vector3(45, 0, 0));

            Renderer = _camera;

            _cube = AddGameObject<CubeRenderer>();
            AddGameObject<GridRenderer>();
        }

        /// <inheritdoc/>
        protected override void Update(float dt)
        {
            base.Update(dt);

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_A))
                Peer.Send(new TestMessage());

            var dim = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
            var positionWithinScreen = (Raylib.GetMousePosition() - dim / 2f) / dim;
            positionWithinScreen.Y = -positionWithinScreen.Y;

            if (MathF.Abs(positionWithinScreen.Y) >= 0.4f ||
                MathF.Abs(positionWithinScreen.X) >= 0.4f)
            {
                var newPos = new Vector3(positionWithinScreen.X, 0f, positionWithinScreen.Y) * 8f * dt;
                _camera!.Move(newPos);
            }

            var ray = _camera!.MouseRay(Raylib.GetMousePosition());
            var hit = Raylib.GetRayCollisionQuad(ray, new Vector3(-5f, 0f, -5f), new Vector3(-5f, 0f, 5f), new Vector3(5f, 0f, 5f), new Vector3(5f, 0f, -5f));
            if (hit.hit)
            {
                // Translate this hit to the square
                var point = hit.point;
                point.X = MathF.Floor(point.X);
                point.Z = MathF.Floor(point.Z);

                _cube!.Position = new Vector3(point.X + 0.5f, 0.5f, point.Z + 0.5f);
            }
        }
    }
}
