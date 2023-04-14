using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Game.Messages
{
    /// <summary>
    /// Sent by the opposite party when bombing.
    /// </summary>
    internal struct BombingResultMessage : INetworkMessage
    {
        public bool hit;

        public void Deserialize(ref NetworkReader reader)
        {
            hit = reader.Read<bool>(); 
        }

        public void Serialize(ref NetworkWriter writer)
        {
            writer.Write(hit);
        }
    }
}
