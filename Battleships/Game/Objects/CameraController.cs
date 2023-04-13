using System.Numerics;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
using Battleships.Game.Data;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A controller for the camera.
    /// </summary>
    internal class CameraController : GameObject
    {
        /// <summary>
        /// How many units a second are we moving?
        /// </summary>
        const float MOVEMENT_SPEED = 5f;

        /// <summary>
        /// The current objective the controller is following.
        /// </summary>
        public CameraObjective Objective { get; set; } = CameraObjective.Idle;

        /// <summary>
        /// The camera.
        /// </summary>
        private Camera? _camera;

        /// <summary>
        /// The game coordinator.
        /// </summary>
        private GameCoordinator? _coordinator;

        /// <inheritdoc/>
        public override void Start()
        {
            _camera = GetGameObjectFromGame<Camera>();
            _coordinator = GetGameObjectFromGame<GameCoordinator>();
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            // A locked camera does nothing.
            if (Objective == CameraObjective.Locked)
                return;

            // If we're idling, allow for free movement.
            if (Objective == CameraObjective.Idle)
            {
                var dim = new Vector2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
                var positionWithinScreen = (Raylib.GetMousePosition() - dim / 2f) / dim;
                positionWithinScreen.Y = -positionWithinScreen.Y;

                if (MathF.Abs(positionWithinScreen.Y) >= 0.4f ||
                    MathF.Abs(positionWithinScreen.X) >= 0.4f)
                {
                    var newPos = new Vector3(positionWithinScreen.X, 0f, positionWithinScreen.Y) * 8f * dt;
                    _camera!.Move(newPos);
                }

                return;
            }

            // Now, if we're moving, we first need to know our own peer id.
            if (Peer?.PeerId == null)
                return;

            var ourId = Peer!.PeerId.Value;
            var targetId = Objective == CameraObjective.MoveToSelf ?
                ourId :
                (ourId + 1) % 2;

            var targetPlayfield = _coordinator!.GetPlayfieldForPlayer(targetId);
            var targetPos = targetPlayfield!.Position;

            Console.WriteLine($"Moving to: {_camera!.Position + targetPos} (camera at: {_camera!.Position})");

            // TODO(pref): lerp
            _camera!.Move(targetPos);
            Objective = CameraObjective.Idle;
        }
    }
}
