namespace Battleships.Framework.Tweening
{
    /// <summary>
    /// Inherited by all tweens.
    /// </summary>
    internal interface ITween
    {
        /// <summary>
        /// Updates the tween.
        /// </summary>
        void UpdateTween(float dt);

        /// <summary>
        /// Is the tween finished?
        /// </summary>
        bool Finished { get; }
    }
}
