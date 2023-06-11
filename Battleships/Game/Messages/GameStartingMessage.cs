using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Game.Messages;

/// <summary>
/// Sent whenever we're starting the game again.
/// </summary>
internal struct GameStartingMessage : INetworkMessage
{
    /// <inheritdoc/>
    public void Deserialize(ref NetworkReader reader)
    {
    }

    /// <inheritdoc/>
    public void Serialize(ref NetworkWriter writer)
    {
    }
}
