using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships
{
    /// <summary>
    /// A grid renderer.
    /// </summary>
    internal class GridRenderer : GameObject, IDrawableGameObject
    {
        public void Draw()
        {
            Raylib.DrawGrid(10, 1.0f);
        }
    }
}
