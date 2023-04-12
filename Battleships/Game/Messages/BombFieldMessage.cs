using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Game.Messages
{
    /// <summary>
    /// Sent when the opposite client tries to bomb a field.
    /// </summary>
    internal struct BombFieldMessage : INetworkMessage
    {
        public int x, y;

        public void Deserialize(ref NetworkReader reader)
        {
            x = reader.Read<int>();
            y = reader.Read<int>();
        }

        public void Serialize(ref NetworkWriter writer)
        {
            writer.Write(x);
            writer.Write(y);
        }
    }
}
