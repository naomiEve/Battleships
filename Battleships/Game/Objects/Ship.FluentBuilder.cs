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
                var part = default(ShipPart?);
                switch (ShipFacing)
                {
                    case Facing.Down:
                        part = _playfield!.CreateShipPart(Position.X, Position.Y + i, this);
                        break;

                    case Facing.Right:
                        part = _playfield!.CreateShipPart(Position.X + i, Position.Y, this);
                        break;
                }

                if (i == 0)
                    part?.SetType(ShipPart.PartType.Head);
                else if (i == Length - 1)
                    part?.SetType(ShipPart.PartType.Tail);
                else
                    part?.SetType(ShipPart.PartType.Body);

                // Add a cannon to this part if either:
                //  - This part is the ending part of a length 2 ship.
                //  - This part is the 2nd part of a length 2+ ship.
                if ((Length == 2 && i == Length - 1) ||
                    (Length > 2 && i == 1))
                {
                    part?.AddCannon();
                }

                _parts.Add(part!);
            }
        }
    }
}
