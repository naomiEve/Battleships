using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Framework.Networking.ServiceDiscovery
{
    /// <summary>
    /// Helpers for interfacing with some IP concepts.
    /// </summary>
    internal static class IPHelper
    {
        /// <summary>
        /// Gets the local host's ip address.
        /// </summary>
        /// <returns>The IP address.</returns>
        public static IPAddress GetLocalIpAddress()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            return hostEntry!.AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)!;
        }
    }
}
