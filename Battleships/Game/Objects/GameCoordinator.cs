using Battleships.Framework.Objects;
using Battleships.Game.Data;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// The battleships coordinator.
    /// </summary>
    internal class GameCoordinator : GameObject,
        ISingletonObject,
        IUIObject
    {
        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState State { get; private set; }
        
        /// <summary>
        /// The playfields for each player.
        /// </summary>
        private ShipPlayfield[]? _playfields;

        /// <inheritdoc/>
        public override void Start()
        {
            
        }

        /// <summary>
        /// Set a playfield for a player.
        /// </summary>
        /// <param name="player">The player index.</param>
        /// <param name="playfield">The playfield.</param>
        public void SetPlayfieldForPlayer(int player, ShipPlayfield playfield)
        {
            _playfields ??= new ShipPlayfield[2];
            _playfields[player] = playfield;

            playfield.Owner = player;
        }

        /// <summary>
        /// Move a camera to a playfield.
        /// </summary>
        /// <param name="player">The index of the player whose playfield we're moving to.</param>
        public void MoveCameraToPlayfield(int player)
        {
            // TODO
        }

        /// <inheritdoc/>
        public void DrawUI()
        {
            Raylib.DrawText($"Current state: {State}.", 0, 0, 20, Color.BLACK);
        }
    }
}
