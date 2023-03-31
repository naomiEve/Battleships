namespace Battleships.Framework.Networking
{
    /// <summary>
    /// A single network packet.
    /// </summary>
    internal class NetworkPacket
    {
        /// <summary>
        /// The mode this packet was sent in.
        /// </summary>
        public SendMode Mode { get; set; } = SendMode.Lockstep;

        /// <summary>
        /// The type of the message that was sent.
        /// </summary>
        public int MessageType { get; set; }

        /// <summary>
        /// The inner message that was serialized.
        /// </summary>
        public INetworkMessage? Message { get; set; }

        /// <summary>
        /// Constructs a new network message with a packet.
        /// </summary>
        /// <param name="message">The message.</param>
        public NetworkPacket(INetworkMessage message)
        {
            Message = message;
        }
    }
}
