using Battleships.Framework.Data;
using Battleships.Framework.Objects;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A ship.
    /// </summary>
    internal partial class Ship : GameObject
    {
        /// <summary>
        /// The ship's facing.
        /// </summary>
        public enum Facing
        {
            Down,
            Right
        }

        /// <summary>
        /// Ship parts.
        /// </summary>
        private readonly List<ShipPart> _parts;

        /// <summary>
        /// The playfield this ship is associated with.
        /// </summary>
        private ShipPlayfield? _playfield;

        /// <summary>
        /// The ship parts.
        /// </summary>
        public IReadOnlyList<ShipPart> Parts => _parts;

        /// <summary>
        /// The ship's facing.
        /// </summary>
        public Facing ShipFacing { get; private set; } = Facing.Down;

        /// <summary>
        /// The ship's length.
        /// </summary>
        public int Length { get; private set; } = 1;

        /// <summary>
        /// The position of the first element of the ship on the playfield.
        /// </summary>
        public Vector2Int Position { get; private set; }

        /// <summary>
        /// Constructs a new ship.
        /// </summary>
        public Ship()
        {
            _parts = new List<ShipPart>();
        }
    }
}
