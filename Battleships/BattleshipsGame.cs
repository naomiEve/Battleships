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
        /// The game coordinator.
        /// </summary>
        private GameCoordinator? _gameCoordinator;

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
            _camera = AddGameObject<Camera>()
                .WithPosition(new Vector3(0, 10f, 10f))
                .WithFOV(10f)
                .WithProjectionType(CameraProjection.CAMERA_ORTHOGRAPHIC)
                .WithShaderPass<PixelizationShaderPass>();

            _camera.Rotate(new Vector3(45, 0, 0));
            CurrentRenderer = _camera;

            _gameCoordinator = AddGameObject<GameCoordinator>();
            AddGameObject<ShipPlayfield>();
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
        }
    }
}
