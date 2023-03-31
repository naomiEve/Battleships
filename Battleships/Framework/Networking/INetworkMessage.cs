namespace Battleships.Framework.Networking
{
    /// <summary>
    /// Base network message that will be a part of the network packet.
    /// </summary>
    internal interface INetworkMessage
    {
        /// <summary>
        /// Serializes this message.
        /// </summary>
        void Serialize();

        /// <summary>
        /// Deserializes this message.
        /// </summary>
        void Deserialize();
    }
}
