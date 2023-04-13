namespace Battleships.Framework.Objects
{
    /// <summary>
    /// Inherited by all objects that want to draw UI.
    /// </summary>
    internal interface IUIObject
    {
        /// <summary>
        /// Draws the UI.
        /// </summary>
        void DrawUI();
    }
}
