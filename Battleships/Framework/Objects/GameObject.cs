namespace Battleships.Framework.Objects
{
    /// <summary>
    /// An object within the game.
    /// </summary>
    internal class GameObject
    {
        /// <summary>
        /// The game this game object is tied to.
        /// </summary>
        protected Game? ThisGame { get; private set; }

        /// <summary>
        /// Set the game this game object belongs to.
        /// </summary>
        /// <param name="game">The game.</param>
        internal void SetGame(Game game)
        {
            ThisGame = game;
        }

        /// <summary>
        /// Called when the object is starting.
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Updates the object.
        /// </summary>
        /// <param name="dt">The delta time.</param>
        public virtual void Update(float dt) { }

        /// <summary>
        /// Called before the object is destroyed.
        /// </summary>
        public virtual void Destroy() { }
    }
}
