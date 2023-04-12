using System.Numerics;
using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships
{
    /// <summary>
    /// A cube renderer.
    /// </summary>
    internal class CubeRenderer : GameObject, IDrawableGameObject
    {
        /// <summary>
        /// The position of this cube.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <inheritdoc/>
        public void Draw()
        {
            Raylib.DrawCube(Position, 1f, 1f, 1f, Color.RED);
        }
    }
}
