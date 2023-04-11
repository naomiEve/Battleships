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
            _camera = new Camera();
            _camera.AddShaderPass<PosterizationShaderPass>();
            _camera.AddShaderPass<PixelizationShaderPass>();
            _camera.RotateY(45f);

            Renderer = _camera;
        }

        /// <inheritdoc/>
        protected override void Draw()
        {
            Raylib.DrawGrid(10, 1.0f);

            var ray = _camera!.MouseRay(Raylib.GetMousePosition());
            var hit = Raylib.GetRayCollisionQuad(ray, new Vector3(-5f, 0f, -5f), new Vector3(-5f, 0f, 5f), new Vector3(5f, 0f, 5f), new Vector3(5f, 0f, -5f));
            if (hit.hit)
            {
                // Translate this hit to the square
                var point = hit.point;
                point.X = MathF.Floor(point.X);
                point.Z = MathF.Floor(point.Z);

                Raylib.DrawCube(new Vector3(point.X + 0.5f, 0.5f, point.Z + 0.5f), 1f, 1f, 1f, Color.RED);
            }
            else
            {
                Raylib.DrawText("Bleh", 0, 0, 30, Color.BLACK);
            }
        }

        /// <inheritdoc/>
        protected override void Update(float dt)
        {
            base.Update(dt);

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_A))
                Peer.Send(new TestMessage());
        }
    }
}
