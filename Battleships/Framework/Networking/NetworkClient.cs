using System;
using System.IO;
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

        private NetworkStream? _stream;

        /// <summary>
        /// Constructs a new network client from the given address and port.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public NetworkClient(IPAddress address, int port)
            : base()
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

            _stream = _client.GetStream();

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

            var read = _stream!.Read(memory.Span);
            return read;
        }
    }
}
