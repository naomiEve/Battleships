using System.Numerics;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
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
        /// The distance between both playfields.
        /// </summary>
        const int PLAYFIELD_DISTANCE = 50;

        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState State { get; private set; }
        
        /// <summary>
        /// The playfields for each player.
        /// </summary>
        private ShipPlayfield[]? _playfields;

        /// <summary>
        /// The camera controller.
        /// </summary>
        private CameraController? _camera;

        /// <inheritdoc/>
        public override void Start()
        {
            // Construct the playfields
            var player1Playfield = ThisGame!.AddGameObject<ShipPlayfield>();
            var player2Playfield = ThisGame!.AddGameObject<ShipPlayfield>();

            // Move the second player's playfield further.
            player2Playfield.Position = new Vector3(PLAYFIELD_DISTANCE, 0, PLAYFIELD_DISTANCE);

            SetPlayfieldForPlayer(0, player1Playfield);
            SetPlayfieldForPlayer(1, player2Playfield);

            _camera = ThisGame.AddGameObject<CameraController>();

            SetState(GameState.ShipBuilding);
        }

        /// <summary>
        /// Set our own state.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetState(GameState state)
        {
            State = state;

            switch (State)
            {
                case GameState.ShipBuilding:
                    _camera!.Objective = CameraObjective.MoveToSelf;
                    break;

                case GameState.PlayerBombing:
                    _camera!.Objective = CameraObjective.MoveToEnemy;
                    break;

                case GameState.OtherPlayerBombing:
                    _camera!.Objective = CameraObjective.MoveToSelf;
                    break;

                case GameState.GameOver:
                    _camera!.Objective = CameraObjective.Locked;
                    break;
            }
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
        /// Gets the playfield for a given player.
        /// </summary>
        /// <param name="player">The player id.</param>
        /// <returns>The playfield.</returns>
        public ShipPlayfield? GetPlayfieldForPlayer(int player)
        {
            return _playfields?[player];
        }

        /// <inheritdoc/>
        public void DrawUI()
        {
            Raylib.DrawText($"Current state: {State}.", 0, 0, 20, Color.BLACK);
        }
    }
}
