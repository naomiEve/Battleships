using System.Net;
using Battleships.Framework;
using Battleships.Framework.Data;
using Battleships.Framework.Networking;
using Battleships.Messages;
using Raylib_cs;

namespace Battleships
{
    /// <summary>
    /// The game logic for battleships.
    /// </summary>
    internal class BattleshipsLogic : IGameLogic
    {
        private NetworkPeer _peer;

        /// <summary>
        /// Construct a new battleship logic with the given launch options.
        /// </summary>
        /// <param name="opts">The launch options.</param>
        public BattleshipsLogic(LaunchOptions opts)
        {
            _peer = ConstructNetworkPeerFromOptions(opts);

            RegisterMessages();

            _peer.WaitUntilReady();
        }

        /// <summary>
        /// Constructs a new network peer from the given launch options.
        /// </summary>
        /// <param name="opts">The options.</param>
        /// <returns>The constructed network peer.</returns>
        /// <exception cref="ArgumentException">Thrown whenever the mode is not one of [server, client].</exception>
        private NetworkPeer ConstructNetworkPeerFromOptions(LaunchOptions opts)
        {
            return opts.Mode switch
            {
                "server" => new NetworkServer(opts.Port),
                "client" => new NetworkClient(IPAddress.Parse(opts.Ip), opts.Port),
                _ => throw new ArgumentException("Invalid mode specified!", nameof(opts))
            };
        }

        /// <summary>
        /// Registers all of the actively used messages.
        /// </summary>
        private void RegisterMessages()
        {
            _peer.RegisterMessage<TestMessage>();
        }

        /// <inheritdoc/>
        public void Destroy()
        {
            Console.WriteLine("goot bye :'(");
        }

        /// <inheritdoc/>
        public void Draw()
        {
            Raylib.ClearBackground(Color.RED);
            Raylib.DrawCircle(Raylib.GetMouseX(), Raylib.GetMouseY(), 40, Color.BLUE);
        }

        /// <inheritdoc/>
        public void Preinitialize()
        {
            Console.WriteLine("helo :)");
        }

        /// <inheritdoc/>
        public void Update(float dt)
        {
            if (!_peer.Ready)
            {
                Console.WriteLine("Connection lost.");
                Raylib.CloseWindow();
                return;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_A))
                _peer.Send(new TestMessage());
        }
    }
}
