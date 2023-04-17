using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// An object responsible for drawing some debug information.
    /// </summary>
    internal class DebugObject : GameObject,
        IUIObject
    {
        /// <summary>
        /// The game coordinator.
        /// </summary>
        private GameCoordinator? _coordinator;

        /// <summary>
        /// The camera controller.
        /// </summary>
        private CameraController? _camera;

        /// <inheritdoc/>
        public override void Start()
        {
            _coordinator = GetGameObjectFromGame<GameCoordinator>();
            _camera = GetGameObjectFromGame<CameraController>();
        }

        /// <inheritdoc/>
        public void DrawUI()
        {
            Raylib.DrawText($"Current state: {_coordinator?.State}.", 0, 0, 20, Color.BLACK);
            Raylib.DrawText($"Current camera objective: {_camera!.Objective}", 0, 25, 20, Color.BLACK);
        }
    }
}
