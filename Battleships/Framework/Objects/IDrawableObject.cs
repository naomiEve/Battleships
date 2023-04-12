namespace Battleships.Framework.Objects
{
    /// <summary>
    /// An interface inherited by all of the objects that want to be drawn.
    /// </summary>
    internal interface IDrawableGameObject
    {
        /// <summary>
        /// Draw this object.
        /// </summary>
        void Draw();
    }
}
