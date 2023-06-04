using System.Numerics;

namespace Battleships.Framework.Objects;

/// <summary>
/// Implemented by anything that has a position.
/// </summary>
internal interface IPositionedObject
{
    /// <summary>
    /// The position.
    /// </summary>
    Vector3 Position { get; }
}
