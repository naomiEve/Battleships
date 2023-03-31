using Battleships.Data;
using Raylib_cs;

namespace Battleships.Framework
{
    /// <summary>
    /// The game.
    /// </summary>
    internal class Game
    {
        private IGameLogic _logic;

        /// <summary>
        /// Construct a new game window.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="name">The name of the window.</param>
        public Game(Vector2Int dimensions, string name, IGameLogic logic)
        {
            _logic = logic;

            Raylib.InitWindow(dimensions.X, dimensions.Y, name);
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
            _logic.Preinitialize();

            while (!Raylib.WindowShouldClose())
            {
                _logic.Update(Raylib.GetFrameTime());

                Raylib.BeginDrawing();
                _logic.Draw();
                Raylib.EndDrawing();
            }

            _logic.Destroy();
            Raylib.CloseWindow();
        }
    }
}
