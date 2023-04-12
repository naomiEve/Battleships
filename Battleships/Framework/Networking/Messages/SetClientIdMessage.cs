using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking.Messages
{
    /// <summary>
    /// Sent when setting the id for a client from a server.
    /// </summary>
    internal struct SetClientIdMessage : INetworkMessage
    {
        public int id;

        public void Deserialize(ref NetworkReader reader)
        {
            id = reader.Read<int>();
        }

        public void Serialize(ref NetworkWriter writer)
        {
            writer.Write(id);
        }
    }
}
