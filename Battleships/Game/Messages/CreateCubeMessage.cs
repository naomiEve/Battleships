using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Game.Messages
{
    /// <summary>
    /// Create a cube at the playfield.
    /// </summary>
    internal struct CreateCubeMessage : INetworkMessage
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
