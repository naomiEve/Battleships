using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Battleships.Framework.Networking.Messages;
using Battleships.Framework.Networking.ServiceDiscovery;

namespace Battleships.Framework.Networking
{
    /// <summary>
    /// A network server.
    /// </summary>
    internal class NetworkServer : NetworkPeer
    {
        /// <summary>
        /// The TCP server.
        /// </summary>
        private readonly TcpListener _server;

        /// <summary>
        /// The client that connected to us.
        /// </summary>
        private TcpClient? _client;

        /// <summary>
        /// The port we're listening on.
        /// </summary>
        private readonly int _port;

        /// <summary>
        /// The network stream we're sending on.
        /// </summary>
        private NetworkStream? _stream;

        /// <summary>
        /// Construct a network server.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public NetworkServer(int port)
            : base()
        {
            _server = new TcpListener(IPAddress.Any, port);
            _port = port;
            IsCurrentLockstepPeer = true;

            // The server is always the zeroth peer.
            PeerId = 0;
        }

        /// <summary>
        /// Sets up the server.
        /// </summary>
        [SuppressMessage("Interoperability", "CA1416:Walidacja zgodności z platformą", Justification = "Inner function has a check for this.")]
        private void SetupServer()
        {
            // Enable NAT traversal for windows based platforms.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                _server.AllowNatTraversal(true);

            _server.Start();
        }

        /// <inheritdoc/>
        public override void WaitUntilReady()
        {
            SetupServer();
            Console.WriteLine("Waiting for a client...");

            // Broadcast this service.
            var discovery = new ServiceDiscoveryServer();
            discovery.BroadcastService(_port);

            _client = _server.AcceptTcpClient();
            _stream = _client.GetStream();

            // If someone has connected, we can safely get rid of the service discovery.
            discovery.StopBroadcastingService();

            // Set their client id.
            Send(new SetClientIdMessage
            {
                id = PeerId!.Value + 1
            }, SendMode.Extra);

            Console.WriteLine("Done!");
        }

        /// <inheritdoc/>
        protected override void SendBytes(ReadOnlySpan<byte> bytes)
        {
            _stream!.Write(bytes);
        }

        /// <inheritdoc/>
        protected override int ReceiveBytes(Memory<byte> memory)
        {
            if (!_stream!.DataAvailable)
                return 0;

            return _stream!.Read(memory.Span);
        }
    }
}
