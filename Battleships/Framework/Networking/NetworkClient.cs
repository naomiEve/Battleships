using System.Net;
using System.Net.Sockets;

namespace Battleships.Framework.Networking
{
    /// <summary>
    /// A network client.
    /// </summary>
    internal class NetworkClient : NetworkPeer
    {
        private readonly TcpClient _client;
        private readonly IPAddress _serverAddress;
        private readonly int _serverPort;

        /// <inheritdoc/>
        public override bool Ready => _client.Connected;

        /// <summary>
        /// Constructs a new network client from the given address and port.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public NetworkClient(IPAddress address, int port)
        {
            _serverAddress = address;
            _serverPort = port;

            _client = new TcpClient();
        }

        /// <inheritdoc/>
        public override void WaitUntilReady()
        {
            Console.WriteLine("Waiting for connection...");
            _client.Connect(_serverAddress, _serverPort);
            
            Console.WriteLine("Done!");
        }
    }
}
