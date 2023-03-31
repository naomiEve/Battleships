using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking
{
    /// <summary>
    /// A single network packet.
    /// </summary>
    internal readonly ref struct NetworkPacket
    {
        /// <summary>
        /// The mode this packet was sent in.
        /// </summary>
        public SendMode Mode { get; init; } = SendMode.Lockstep;

        /// <summary>
        /// The type of the message that was sent.
        /// </summary>
        public int MessageType { get; init; }

        /// <summary>
        /// The inner message that was serialized.
        /// </summary>
        public INetworkMessage? Message { get; init; }

        /// <summary>
        /// Constructs a new network message with a packet.
        /// </summary>
        /// <param name="message">The message.</param>
        public NetworkPacket(INetworkMessage message, SendMode mode)
        {
            Mode = mode;
            MessageType = message.GetType().GetHashCode();
            Message = message;
        }

        /// <summary>
        /// Serializes this packet into the network.
        /// </summary>
        public int Serialize(NetworkWriter writer)
        {
            writer.Write(Mode);
            writer.Write(MessageType);
            Message?.Serialize(writer);
            return writer.Written;
        }
    }
}
