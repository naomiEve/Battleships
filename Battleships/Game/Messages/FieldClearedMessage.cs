using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Game.Messages
{
    /// <summary>
    /// Sent by a player whenever their field is cleared.
    /// </summary>
    internal struct FieldClearedMessage : INetworkMessage
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
