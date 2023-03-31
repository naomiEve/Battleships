namespace Battleships.Framework
{
    /// <summary>
    /// Interface that all logic has to implement.
    /// </summary>
    internal interface IGameLogic
    {
        /// <summary>
        /// Called before the game window gets initialized.
        /// </summary>
        void Preinitialize();

        /// <summary>
        /// Called when updating all of the logic.
        /// </summary>
        /// <param name="dt">The delta time.</param>
        /// <returns>Whether we should close after this frame.</returns>
        bool Update(float dt);

        /// <summary>
        /// Called when the game window gets drawn.
        /// </summary>
        void Draw();

        /// <summary>
        /// Drawn when the game window is exitting.
        /// </summary>
        void Destroy();
    }
}
