using System.Net;
using System.Net.Sockets;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking.ServiceDiscovery
{
    /// <summary>
    /// A client that tries to retrieve information about the existing games
    /// on the local area network via the broadcast address.
    /// 
    /// Allows players to join games that are happening on the LAN without knowing the IP.
    /// </summary>
    internal class ServiceDiscoveryClient
    {
        /// <summary>
        /// The udp client bound to the broadcast.
        /// </summary>
        private readonly UdpClient _udpClient;

        /// <summary>
        /// The current list of services.
        /// </summary>
        private readonly List<ServiceInfo> _services;

        /// <summary>
        /// The thread we're receiving on.
        /// </summary>
        private Thread? _receiveThread;

        /// <summary>
        /// The currently existing services.
        /// </summary>
        public IReadOnlyList<ServiceInfo> Services => _services;

        /// <summary>
        /// Are we currently receiving the services?
        /// </summary>
        public bool IsReceiving { get; private set; } = false;

        /// <summary>
        /// Creates a new service discovery server and binds it to the broadcast address.
        /// </summary>
        public ServiceDiscoveryClient()
        {
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 2023));
            _udpClient.Client.ReceiveTimeout = 500;
            _services = new List<ServiceInfo>();
        }

        /// <summary>
        /// Start broadcasting a service.
        /// </summary>
        public void StartListeningForServices()
        {
            if (IsReceiving)
                return;

            IsReceiving = true;

            _receiveThread = new Thread(ReceiveLoop);
            _receiveThread.Start();
        }

        /// <summary>
        /// Stop broadcasting this service.
        /// </summary>
        public void StopListeningForServices()
        {
            if (!IsReceiving)
                return;

            IsReceiving = false;
            _receiveThread?.Join();
        }

        /// <summary>
        /// The loop that receives game data.
        /// </summary>
        private void ReceiveLoop()
        {
            Span<byte> recvBuffer = stackalloc byte[1024];

            while (IsReceiving)
            {
                // Try to read the data.
                try
                {
                    var read = _udpClient.Client.Receive(recvBuffer);
                    if (read <= 0)
                        continue;

                    // Decode the service info.
                    var reader = new NetworkReader(recvBuffer);
                    var serviceInfo = new ServiceInfo(ref reader);

                    // Add this to the list if we don't have it.
                    lock (_services)
                    {
                        if (!_services.Any(info => info.Hostname == serviceInfo.Hostname))
                            _services.Add(serviceInfo);
                    }
                }
                catch (SocketException)
                {
                    continue;
                }
            }

            _udpClient.Close();
            _udpClient.Dispose();
        }
    }
}
