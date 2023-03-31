using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Battleships.Framework.Networking
{
    /// <summary>
    /// A network server.
    /// </summary>
    internal class NetworkServer : NetworkPeer
    {
        private readonly TcpListener _server;
        private TcpClient? _client;

        /// <summary>
        /// Construct a network server.
        /// </summary>
        /// <param name="port">The port to listen on.</param>
        public NetworkServer(int port)
            : base()
        {
            _server = new TcpListener(IPAddress.Any, port);
            IsCurrentLockstepPeer = true;
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

            _client = _server.AcceptTcpClient();
            Console.WriteLine("Done!");
        }

        /// <inheritdoc/>
        protected override void SendBytes(ReadOnlySpan<byte> bytes)
        {
            var stream = _client!.GetStream();
            stream.Write(bytes);
        }

        /// <inheritdoc/>
        protected override int ReceiveBytes(Memory<byte> memory)
        {
            var stream = _client!.GetStream();
            if (!stream.DataAvailable)
                return 0;

            return stream.Read(memory.Span);
        }
    }
}
