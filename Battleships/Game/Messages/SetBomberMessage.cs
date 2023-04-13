using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Game.Messages
{
    /// <summary>
    /// Set the player who starts bombing.
    /// </summary>
    internal struct SetBomberMessage : INetworkMessage
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
