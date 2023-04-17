using System.Numerics;
using Battleships.Framework.Data;
using Battleships.Framework.Networking;
using Battleships.Framework.Objects;
using Battleships.Game.Data;
using Battleships.Game.Messages;
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
        const float PLAYFIELD_DISTANCE = 15f;

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

        /// <summary>
        /// The sizes of the ships we're placing in the initial round.
        /// </summary>
        //private readonly static int[] _shipLengths = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
        private readonly static int[] _shipLengths = { 4, 3 };
        /// <summary>
        /// The current index of the ship length.
        /// </summary>
        private int _shipLengthIndex = 0;

        /// <inheritdoc/>
        public override void Start()
        {
            Peer?.MessageRegistry.RegisterMessage<FinishedBuildingMessage>(mesg =>
            {
                var buildMesg = (FinishedBuildingMessage)mesg;

                _playfields![buildMesg.id].FinishedBuilding = true;
                CheckIfAllFinishedBuilding();
            });

            Peer?.MessageRegistry.RegisterMessage<SetBomberMessage>(mesg =>
            {
                var bomberMesg = (SetBomberMessage)mesg;
                Console.WriteLine($"bomber={bomberMesg.id}");
                SetBomber(bomberMesg.id, false);
            });

            Peer?.MessageRegistry.RegisterMessage<BombFieldMessage>(mesg =>
            {
                var bombMesg = (BombFieldMessage)mesg;

                var playfield = _playfields![bombMesg.field];
                playfield.TryBombField(new Vector2Int(bombMesg.x, bombMesg.y));
            });

            Peer?.MessageRegistry.RegisterMessage<BombingResultMessage>(mesg =>
            {
                var resultMesg = (BombingResultMessage)mesg;

                // We've hit, continue bombing.
                if (resultMesg.hit)
                    SetState(GameState.PlayerBombing);
            });

            Peer?.MessageRegistry.RegisterMessage<FieldClearedMessage>(mesg =>
            {
                var clearedMesg = (FieldClearedMessage)mesg;

                var winner = (clearedMesg.id + 1) % 2;
                SetGameOver(winner, true);                
            });

            Peer?.MessageRegistry.RegisterMessage<GameOverMessage>(mesg =>
            {
                var gameOverMesg = (GameOverMessage)mesg;

                SetGameOver(gameOverMesg.winner, false);
            });

            _camera = ThisGame!.AddGameObject<CameraController>();

            // Construct the playfields
            var player1Playfield = ThisGame!.AddGameObject<ShipPlayfield>();
            var player2Playfield = ThisGame!.AddGameObject<ShipPlayfield>();

            // Move the second player's playfield further.
            player2Playfield.Position = new Vector3(PLAYFIELD_DISTANCE, 0, 0);

            SetPlayfieldForPlayer(0, player1Playfield);
            SetPlayfieldForPlayer(1, player2Playfield);

            SetState(GameState.ShipBuilding);
        }

        /// <summary>
        /// Check if all players have finished building.
        /// </summary>
        private void CheckIfAllFinishedBuilding()
        {
            if (!Peer!.IsHost)
                return;

            var count = _playfields!.Count(field => field.FinishedBuilding);
            Console.WriteLine(count);

            // If everyone finished building, commence the next part.
            if (count == _playfields!.Length)
            {
                // Roll a die to decide who gets the next move.
                var player = Random.Shared.Next(0, 1);
                SetBomber(player, true);
            }
        }

        /// <summary>
        /// Sets the current bomber.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="sendMessage">Should we send the message to the other player?</param>
        public void SetBomber(int player, bool sendMessage)
        {
            SetState(player == Peer!.PeerId ? GameState.PlayerBombing : GameState.OtherPlayerBombing);

            if (sendMessage)
            {
                Peer?.Send(new SetBomberMessage
                {
                    id = player,
                }, passLockstep: State == GameState.OtherPlayerBombing);
            }
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
                    _shipLengthIndex = 0;

                    foreach (var field in _playfields!)
                        field.SetPreviewLength(_shipLengths[_shipLengthIndex]);
                    break;

                case GameState.Waiting:
                    _camera!.Objective = CameraObjective.Idle;
                    break;

                case GameState.PlayerBombing:
                    _camera!.Objective = CameraObjective.MoveToEnemy;
                    break;

                case GameState.OtherPlayerBombing:
                    _camera!.Objective = CameraObjective.MoveToSelf;
                    break;

                case GameState.GameOver:
                    _camera!.Objective = CameraObjective.Idle;
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

        /// <summary>
        /// Sets the game over state.
        /// </summary>
        /// <param name="winner">The winner index.</param>
        /// <param name="sendMessage">Should we send a message to the other peer?</param>
        public void SetGameOver(int winner, bool sendMessage)
        {
            SetState(GameState.GameOver);
            var gameOver = ThisGame!.AddGameObject<GameOverText>();
            gameOver.Winner = winner;

            if (sendMessage)
            {
                Peer?.Send(new GameOverMessage
                {
                    winner = winner
                }, SendMode.Extra);
            }
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            // If we're building the ships, we need to check how many ships we've built.
            // And stop if we've built them all.
            if (State == GameState.ShipBuilding)
            {
                if (ThisGame?.GetCountOfObjectsOfType<Ship>() > _shipLengthIndex)
                {
                    _shipLengthIndex += 1;
                    if (_shipLengthIndex < _shipLengths.Length)
                    {
                        _playfields![Peer!.PeerId!.Value].SetPreviewLength(_shipLengths[_shipLengthIndex]);
                    }
                    else
                    {
                        _playfields![Peer!.PeerId!.Value].FinishedBuilding = true;
                        Peer!.Send(new FinishedBuildingMessage
                        {
                            id = Peer!.PeerId!.Value
                        }, SendMode.Extra);
                        SetState(GameState.Waiting);

                        CheckIfAllFinishedBuilding();
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void DrawUI()
        {
            Raylib.DrawText($"Current state: {State}.", 0, 0, 20, Color.BLACK);
            Raylib.DrawText($"Current camera objective: {_camera!.Objective}", 0, 25, 20, Color.BLACK);
        }
    }
}
