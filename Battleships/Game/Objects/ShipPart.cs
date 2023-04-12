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
        /// The ship part's position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <inheritdoc/>
        public RayCollision Collide(Ray ray)
        {
            return Raylib.GetRayCollisionBox(ray, new BoundingBox());
        }

        /// <inheritdoc/>
        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
