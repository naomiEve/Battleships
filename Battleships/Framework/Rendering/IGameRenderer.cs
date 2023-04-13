namespace Battleships.Framework.Rendering
{
    internal interface IGameRenderer
    {
        /// <summary>
        /// Begin the rendering.
        /// </summary>
        void Begin();

        /// <summary>
        /// End the rendering.
        /// </summary>
        void End();

        /// <summary>
        /// Blit the camera to the screen.
        /// </summary>
        void Blit();
    }
}
