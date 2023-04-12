using System.Reflection.Metadata.Ecma335;
using Battleships.Framework.Data;

namespace Battleships.Game.Objects
{
    internal partial class Ship
    {
        /// <summary>
        /// Sets the playfield of this ship.
        /// </summary>
        /// <param name="playfield">The ship's playfield.</param>
        public Ship ForPlayfield(ShipPlayfield playfield)
        {
            _playfield = playfield;
            return this;
        }

        /// <summary>
        /// The position of the first ship element.
        /// </summary>
        /// <param name="position">Position.</param>
        public Ship AtPosition(Vector2Int position)
        {
            Position = position;
            return this;
        }

        /// <summary>
        /// Sets the length of this ship.
        /// </summary>
        /// <param name="length">The length of the ship.</param>
        public Ship WithLength(int length)
        {
            Length = length;
            return this;
        }

        /// <summary>
        /// Sets the facing of this ship.
        /// </summary>
        /// <param name="facing">The facing.</param>
        public Ship WithFacing(Facing facing)
        {
            ShipFacing = facing;
            return this;
        }

        /// <summary>
        /// Builds this ship.
        /// </summary>
        public void BuildShip()
        {
            for (var i = 0; i < Length; i++)
            {
                switch (ShipFacing)
                {
                    case Facing.Down:
                        _parts.Add(_playfield!.CreateShipPart(Position.X, Position.Y + i, this));
                        break;

                    case Facing.Right:
                        _parts.Add(_playfield!.CreateShipPart(Position.X + i, Position.Y, this));
                        break;
                }
            }
        }
    }
}
