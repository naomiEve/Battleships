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
            // If we're idling, do nothing.
            if (Objective == CameraObjective.Idle)
                return;

            // Now, if we're moving, we first need to know our own peer id.
            if (Peer?.PeerId == null)
                return;

            var ourId = Peer!.PeerId.Value;
            var targetId = Objective == CameraObjective.MoveToSelf ?
                ourId :
                (ourId + 1) % 2;

            var target = _coordinator!.GetPlayfieldForPlayer(targetId);
            _camera!.SnapTo(target!.Position);
            Objective = CameraObjective.Idle;
        }
    }
}
