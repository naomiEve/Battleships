using Raylib_cs;

namespace Battleships.Framework.Objects
{
    /// <summary>
    /// Inherited by all of the objects that also can be a raycast target.
    /// </summary>
    internal interface IRaycastTargettableObject
    {
        /// <summary>
        /// Checks if a ray collides with this object.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <returns>The collision data.</returns>
        RayCollision Collide(Ray ray);
    }
}
