using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking.ServiceDiscovery
{
    /// <summary>
    /// Broadcasted information about a service.
    /// </summary>
    internal class ServiceInfo : INetworkMessage
    {
        /// <summary>
        /// The IP of this service.
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// The port of this service.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The name of the host.
        /// </summary>
        public string? Hostname { get; set; }

        /// <summary>
        /// Constructs a new ServiceInfo from the given parameters.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <param name="port">The port.</param>
        /// <param name="hostname">The hostname of this machine.</param>
        public ServiceInfo(string ip, int port, string hostname)
        {
            Ip = ip;
            Port = port;
            Hostname = hostname;
        }

        /// <summary>
        /// Constructs a new ServiceInfo and deserializes it from a reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public ServiceInfo(ref NetworkReader reader)
        {
            Deserialize(ref reader);
        }

        /// <inheritdoc/>
        public void Serialize(ref NetworkWriter writer)
        {
            writer.WriteString(Ip!);
            writer.Write(Port);
            writer.WriteString(Hostname!);
        }

        /// <inheritdoc/>
        public void Deserialize(ref NetworkReader reader)
        {
            Ip = reader.ReadString();
            Port = reader.Read<int>();
            Hostname = reader.ReadString();
        }
    }
}
