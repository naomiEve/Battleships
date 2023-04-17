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
        public int x, y;
        public int field;

        public void Deserialize(ref NetworkReader reader)
        {
            hit = reader.Read<bool>();
            x = reader.Read<int>();
            y = reader.Read<int>();
            field = reader.Read<int>();
        }

        public void Serialize(ref NetworkWriter writer)
        {
            writer.Write(hit);
            writer.Write(x);
            writer.Write(y);
            writer.Write(field);
        }
    }
}
