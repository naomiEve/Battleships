using System.Net;
using System.Net.Sockets;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking.ServiceDiscovery
{
    /// <summary>
    /// A server that broadcasts information about the current game on the local area network
    /// via the broadcast address.
    /// 
    /// Allows players to join games that are happening on the LAN without knowing the IP.
    /// </summary>
    internal class ServiceDiscoveryServer
    {
        /// <summary>
        /// The udp client bound to the broadcast.
        /// </summary>
        private readonly UdpClient _udpClient;

        /// <summary>
        /// The endpoint we're broadcasting on.
        /// </summary>
        private readonly IPEndPoint _broadcastEndpoint;

        /// <summary>
        /// The thread responsible for broadcasting the data.
        /// </summary>
        private Thread? _broadcastThread;

        /// <summary>
        /// The service we're broadcasting.
        /// </summary>
        private ServiceInfo? _serviceInfo;

        /// <summary>
        /// Are we broadcasting a service right now?
        /// </summary>
        public bool IsBroadcasting { get; private set; } = false;

        /// <summary>
        /// Creates a new service discovery server and binds it to the broadcast address.
        /// </summary>
        public ServiceDiscoveryServer()
        {
            _udpClient = new UdpClient
            {
                EnableBroadcast = true
            };

            _broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, 2023);
        }

        /// <summary>
        /// Start broadcasting a service.
        /// </summary>
        public void BroadcastService(int port)
        {
            if (IsBroadcasting)
                return;

            IsBroadcasting = true;

            _serviceInfo = new ServiceInfo(IPHelper.GetLocalIpAddress().ToString(), port, Dns.GetHostName());
            _broadcastThread = new Thread(BroadcastLoop);
            _broadcastThread.Start();
        }

        /// <summary>
        /// Stop broadcasting this service.
        /// </summary>
        public void StopBroadcastingService()
        {
            if (!IsBroadcasting)
                return;

            IsBroadcasting = false;
            _broadcastThread?.Join();
        }

        /// <summary>
        /// Continuously broadcasts this service.
        /// </summary>
        private void BroadcastLoop()
        {
            const int mtu = 1200;

            // Prepare the data to send already, so we don't allocate all the time.
            // We can safely allocate this on the stack, we shouldn't exceed the MTU either way.
            Span<byte> buffer = stackalloc byte[mtu];
            var writer = new NetworkWriter(buffer);
            
            _serviceInfo!.Serialize(ref writer);
            var slice = buffer[..writer.Written];

            while (IsBroadcasting)
            {
                _udpClient.Send(slice, _broadcastEndpoint);
                Thread.Sleep(500);
            }
        }
    }
}
