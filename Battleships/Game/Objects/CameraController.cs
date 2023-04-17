using System.Numerics;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
using Battleships.Framework.Tweening;
using Battleships.Game.Data;

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
        const float MOVEMENT_SPEED = 1f;

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

        /// <summary>
        /// The mover tween.
        /// </summary>
        private Tween<Vector3>? _moverTween;

        /// <inheritdoc/>
        public override void Start()
        {
            _camera = GetGameObjectFromGame<Camera>();
            _coordinator = GetGameObjectFromGame<GameCoordinator>();
        }

        /// <summary>
        /// Constructs a mover tween from the camera's position to newPosition.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        private void ConstructMoverTween(Vector3 newPosition)
        {
            _moverTween?.Kill();

            _moverTween = new Tween<Vector3>(
                _camera!.Target,
                newPosition,
                MOVEMENT_SPEED,
                TimeEasing.OutCubic,
                (a, b, t) => a.LinearInterpolation(b, t),
                position => _camera!.SnapTo(position),
                fin =>
                {
                    _camera!.SnapTo(fin);
                    _moverTween = null;

                    Objective = CameraObjective.Idle;
                });

            GetGameObjectFromGame<TweenEngine>()!
                .AddTween(_moverTween);
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

            if (_moverTween == null)
            {
                var ourId = Peer!.PeerId.Value;
                var targetId = Objective == CameraObjective.MoveToSelf ?
                    ourId :
                    (ourId + 1) % 2;

                var target = _coordinator!.GetPlayfieldForPlayer(targetId);

                ConstructMoverTween(target!.Position);
            }
        }
    }
}
