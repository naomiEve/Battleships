using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking.Messages
{
    /// <summary>
    /// The message sent whenever we're passing the current lockstep simulation ownership.
    /// </summary>
    internal struct LockstepPassingMessage : INetworkMessage
    {
        public void Serialize(ref NetworkWriter writer)
        {

        }

        public void Deserialize(ref NetworkReader reader)
        {

        }
    }
}
