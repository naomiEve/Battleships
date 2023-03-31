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
        /// Constructs and reads a network packet from the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public NetworkPacket(ref NetworkReader reader, MessageRegistry registry)
        {
            Mode = reader.Read<SendMode>();
            MessageType = reader.Read<int>();

            // Try to construct the inner message if we know of it.
            if (registry.TryGetMessageConstructor(MessageType, out var constructor))
                Message = constructor!();

            Message?.Deserialize(ref reader);

            Console.WriteLine($"Got a message of type {MessageType} with mode {Mode}. Inner message exists? {Message != null}");
        }

        /// <summary>
        /// Serializes this packet into the network.
        /// </summary>
        public int Serialize(ref NetworkWriter writer)
        {
            writer.Write(Mode);
            writer.Write(MessageType);
            Message?.Serialize(ref writer);
            return writer.Written;
        }
    }
}
