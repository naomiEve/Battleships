using Battleships.Framework.Networking;

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
        /// The network peer of this game.
        /// </summary>
        protected NetworkPeer? Peer => (ThisGame as NetworkedGame)?.Peer;

        /// <summary>
        /// Is this game object enabled?
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Set the game this game object belongs to.
        /// </summary>
        /// <param name="game">The game.</param>
        internal void SetGame(Game game)
        {
            ThisGame = game;
        }

        /// <summary>
        /// Tries to get a game object by its type from the game.
        /// </summary>
        /// <typeparam name="TGameObject">The type of the game object.</typeparam>
        /// <returns>The game object, or nothing.</returns>
        protected TGameObject? GetGameObjectFromGame<TGameObject>()
            where TGameObject : GameObject
        {
            return ThisGame?.GetGameObjectOfType<TGameObject>();
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
