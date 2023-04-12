using System.Net;
using Battleships.Framework.Data;
using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Messages;

namespace Battleships.Framework
{
    /// <summary>
    /// A base class for any networked game.
    /// </summary>
    internal abstract class NetworkedGame : Game
    {
        /// <summary>
        /// The network peer.
        /// </summary>
        public NetworkPeer Peer { get; init; }

        /// <summary>
        /// Constructs a new networked game.
        /// </summary>
        /// <param name="dimensions">The dimensions of the window.</param>
        /// <param name="title">The title of the window.</param>
        /// <param name="opts">The options.</param>
        public NetworkedGame(Vector2Int dimensions, string title, LaunchOptions opts)
            : base(dimensions, $"{title} ({opts.Mode})", opts)
        {
            Peer = ConstructNetworkPeerFromOptions(opts);
            RegisterMessages();
        }

        /// <summary>
        /// Constructs a new network peer from the given launch options.
        /// </summary>
        /// <param name="opts">The options.</param>
        /// <returns>The constructed network peer.</returns>
        /// <exception cref="ArgumentException">Thrown whenever the mode is not one of [server, client].</exception>
        private static NetworkPeer ConstructNetworkPeerFromOptions(LaunchOptions opts)
        {
            return opts.Mode switch
            {
                "server" => new NetworkServer(opts.Port),
                "client" => new NetworkClient(IPAddress.Parse(opts.Ip), opts.Port),
                _ => throw new ArgumentException("Invalid mode specified!", nameof(opts))
            };
        }

        /// <inheritdoc/>
        protected override void Preinitialize()
        {
            Peer.WaitUntilReady();
        }

        /// <summary>
        /// Receives messages in addition to doing what is described by: <see cref="Game.Update(float)"/>
        /// </summary>
        protected override void Update(float dt)
        {
            Peer.ReceiveMessages();

            if (!Peer.Ready)
                ShouldClose = true;

            base.Update(dt);
        }

        /// <inheritdoc/>
        protected override void Destroy()
        {
            Peer.Send(new DisconnectMessage(), SendMode.Extra);
        }

        /// <summary>
        /// Registers all the messages associated with this game.
        /// </summary>
        protected abstract void RegisterMessages();
    }
}
