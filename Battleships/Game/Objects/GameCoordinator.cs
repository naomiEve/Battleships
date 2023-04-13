using Battleships.Framework.Objects;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// The battleships coordinator.
    /// </summary>
    internal class GameCoordinator : GameObject,
        ISingletonObject
    {
        /// <summary>
        /// The playfields for each player.
        /// </summary>
        private ShipPlayfield[]? _playfields;

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
    }
}
