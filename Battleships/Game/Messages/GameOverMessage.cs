using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Game.Messages
{
    /// <summary>
    /// Sent whenever we've reached a game over.
    /// </summary>
    internal struct GameOverMessage : INetworkMessage
    {
        public int winner;

        public void Deserialize(ref NetworkReader reader)
        {
            winner = reader.Read<int>();
        }

        public void Serialize(ref NetworkWriter writer)
        {
            writer.Write(winner);
        }
    }
}
