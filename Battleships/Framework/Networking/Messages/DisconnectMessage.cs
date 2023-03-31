using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking.Messages
{
    /// <summary>
    /// A message sent by a party when they're closing their game, to ensure the other party doesn't keep waiting forever.
    /// </summary>
    internal struct DisconnectMessage : INetworkMessage
    {
        public void Deserialize(ref NetworkReader reader)
        {
        }

        public void Serialize(ref NetworkWriter writer)
        {
        }
    }
}
