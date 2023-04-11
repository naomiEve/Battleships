using Battleships.Framework.Data;
using Raylib_cs;

namespace Battleships.Framework
{
    /// <summary>
    /// The game.
    /// </summary>
    internal abstract class Game
    {
        /// <summary>
        /// The launch options of this game.
        /// </summary>
        protected LaunchOptions _launchOptions;

        /// <summary>
        /// The dimensions of the game window.
        /// </summary>
        public Vector2Int Dimensions { get; private set; }

        /// <summary>
        /// The title of the game window.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Should the game window close after the next update cycle finishes?
        /// </summary>
        public bool ShouldClose { get; protected set; }

        /// <summary>
        /// Construct a new game window.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="name">The name of the window.</param>
        /// <param name="opts">The launch options.</param>
        public Game(Vector2Int dimensions, string name, LaunchOptions opts)
        {
            Dimensions = dimensions;
            Title = name;
            _launchOptions = opts;
        }

        /// <summary>
        /// Sets the framerate limit of this game window.
        /// </summary>
        /// <param name="limit">The limit.</param>
        public void SetFramerateLimit(int limit)
        {
            Raylib.SetTargetFPS(limit);
        }

        /// <summary>
        /// Runs the game loop.
        /// </summary>
        public void Run()
        {
            Preinitialize();

            Raylib.InitWindow(Dimensions.X, Dimensions.Y, Title);
            while (!Raylib.WindowShouldClose())
            {
                Update(Raylib.GetFrameTime());
                if (ShouldClose)
                    break;

                Raylib.BeginDrawing();
                Draw();
                Raylib.EndDrawing();
            }

            Destroy();
            Raylib.CloseWindow();
        }

        /// <summary>
        /// Ran before we initialize anything.
        /// </summary>
        protected virtual void Preinitialize() { }

        /// <summary>
        /// Runs a single update tick.
        /// </summary>
        /// <param name="dt">The time since the last frame.</param>
        protected abstract void Update(float dt);

        /// <summary>
        /// Draws the game.
        /// </summary>
        protected abstract void Draw();

        /// <summary>
        /// Destroys all assets related to the game.
        /// </summary>
        protected virtual void Destroy() { }
    }
}
