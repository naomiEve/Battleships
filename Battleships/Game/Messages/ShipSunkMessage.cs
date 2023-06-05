using Battleships.Framework.Networking;
using Battleships.Framework.Networking.Serialization;
using Battleships.Game.Objects;

namespace Battleships.Game.Messages;

/// <summary>
/// Sent whenever we've sunk a ship.
/// </summary>
internal struct ShipSunkMessage : INetworkMessage
{
    public int playfield;
    public int x, y;
    public int length;
    public Ship.Facing facing;

    /// <inheritdoc/>
    public void Deserialize(ref NetworkReader reader)
    {
        playfield = reader.Read<int>();
        x = reader.Read<int>();
        y = reader.Read<int>();
        length = reader.Read<int>();
        facing = reader.Read<Ship.Facing>();
    }

    /// <inheritdoc/>
    public void Serialize(ref NetworkWriter writer)
    {
        writer.Write(playfield);
        writer.Write(x);
        writer.Write(y);
        writer.Write(length);
        writer.Write(facing);
    }
}
