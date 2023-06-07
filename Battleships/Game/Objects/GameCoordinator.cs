using System.Numerics;
using Battleships.Framework.Data;
using Battleships.Framework.Networking;
using Battleships.Framework.Objects;
using Battleships.Game.Data;
using Battleships.Game.Messages;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// The battleships coordinator.
    /// </summary>
    internal class GameCoordinator : GameObject,
        ISingletonObject
    {
        /// <summary>
        /// The distance between both playfields.
        /// </summary>
        const float PLAYFIELD_DISTANCE = 30f;

        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState State { get; private set; }

        /// <summary>
        /// The ship count.
        /// </summary>
        public static int ShipCount => _shipLengths.Length;
        
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
        public override async void Start()
        {
            Peer?.MessageRegistry.RegisterMessage<FinishedBuildingMessage>(mesg =>
            {
                var buildMesg = (FinishedBuildingMessage)mesg;

                GetGameObjectFromGame<GameLog>()!.AddMessageToLog($"Player {buildMesg.id} finished building.");

                _playfields![buildMesg.id].FinishedBuilding = true;
                CheckIfAllFinishedBuilding();
            });

            Peer?.MessageRegistry.RegisterMessage<SetBomberMessage>(mesg =>
            {
                var bomberMesg = (SetBomberMessage)mesg;

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
                var field = _playfields![resultMesg.field];
                var pos = new Vector2Int(resultMesg.x, resultMesg.y);

                // We've hit, continue bombing.
                if (resultMesg.hit)
                {
                    GetGameObjectFromGame<GameLog>()!
                        .AddMessageToLog($"And hit a ship!");

                    SetState(GameState.PlayerBombing);
                    field.SpawnShipDebrisAt(pos);
                }
                else
                {
                    GetGameObjectFromGame<GameLog>()!
                        .AddMessageToLog($"And missed.");

                    field.SpawnBuoyAt(pos);
                }
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

            Peer?.MessageRegistry.RegisterMessage<ShipSunkMessage>(mesg =>
            {
                var shipSunkMesg = (ShipSunkMessage)mesg;
                var field = _playfields![shipSunkMesg.playfield];

                GetGameObjectFromGame<GameLog>()!
                        .AddMessageToLog($"You've sunk a ship.");

                field.SurroundSunkShipWithBuoys(new(shipSunkMesg.x, shipSunkMesg.y), shipSunkMesg.facing, shipSunkMesg.length);
            });

            ThisGame!.AddGameObject<GameLog>();

            _camera = ThisGame!.AddGameObject<CameraController>();

            // Construct the playfields
            var player1Playfield = ThisGame!.AddGameObject<ShipPlayfield>();
            var player2Playfield = ThisGame!.AddGameObject<ShipPlayfield>();

            // Move the second player's playfield further.
            player2Playfield.Position = new Vector3(PLAYFIELD_DISTANCE, 0, 0);

            SetPlayfieldForPlayer(0, player1Playfield);
            SetPlayfieldForPlayer(1, player2Playfield);

            SetState(GameState.ShipBuilding);

            // Wait a bit for the game to stabilize before displaying the announcement.
            // Otherwise we'd get a bogus dt in the TweenEngine update.
            await Task.Delay(100);
            GetGameObjectFromGame<AnnouncementController>()!
                .DisplayAnnouncement(AnnouncementController.AnnouncementType.BuildYourFleet);

            GetGameObjectFromGame<GameLog>()!
                .AddMessageToLog("Welcome to battleships.");
        }

        /// <summary>
        /// Check if all players have finished building.
        /// </summary>
        private void CheckIfAllFinishedBuilding()
        {
            if (!Peer!.IsHost)
                return;

            var count = _playfields!.Count(field => field.FinishedBuilding);

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
            var newBomberIsUs = player == Peer!.PeerId;

            GetGameObjectFromGame<GameLog>()!
                .AddMessageToLog(newBomberIsUs ? "Your turn." : "Opponent's turn.");

            SetState(newBomberIsUs ? GameState.PlayerBombing : GameState.OtherPlayerBombing);
            GetGameObjectFromGame<AnnouncementController>()!
                .DisplayAnnouncement(newBomberIsUs ? AnnouncementController.AnnouncementType.YourTurn : AnnouncementController.AnnouncementType.OpponentsTurn);

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
            GetGameObjectFromGame<AnnouncementController>()!
                .DisplayAnnouncement(winner == 1 ? AnnouncementController.AnnouncementType.Player1Won : AnnouncementController.AnnouncementType.Player2Won);

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
                        GetGameObjectFromGame<GameLog>()!.AddMessageToLog($"Player {Peer?.PeerId} finished building.");

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
    }
}
