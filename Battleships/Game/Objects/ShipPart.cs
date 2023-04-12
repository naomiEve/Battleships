using System.Numerics;
using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A part of a battleship.
    /// </summary>
    internal class ShipPart : GameObject,
        IDrawableGameObject,
        IRaycastTargettableObject
    {
        /// <summary>
        /// The type of the ship part.
        /// </summary>
        public enum PartType
        {
            Head,
            Body,
            Tail
        }

        /// <summary>
        /// The ship part's position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The type of this ship part.
        /// </summary>
        public PartType Type { get; set; }

        /// <summary>
        /// The parent ship.
        /// </summary>
        public Ship? Ship { get; set; }

        /// <summary>
        /// Is this part sunk?
        /// </summary>
        public bool Sunk { get; set; } = false;

        /// <inheritdoc/>
        public RayCollision Collide(Ray ray)
        {
            return Raylib.GetRayCollisionBox(ray, new BoundingBox());
        }

        /// <inheritdoc/>
        public void Draw()
        {
            if (Sunk)
                return;

            Raylib.DrawCube(Position, 1f, 1f, 1f, Color.RED);
        }
    }
}
