using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Messages
{
    internal struct TestMessage : INetworkMessage
    {
        public void Serialize(NetworkWriter writer)
        {
            writer.Write(12345);
        }

        public void Deserialize(NetworkReader reader)
        {
        }
    }
}
