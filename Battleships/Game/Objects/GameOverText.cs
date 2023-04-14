using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// The text displayed after a game over.
    /// </summary>
    internal class GameOverText : GameObject,
        IUIObject
    {
        /// <summary>
        /// The winner.
        /// </summary>
        public int Winner { get; set; }

        /// <inheritdoc/>
        public void DrawUI()
        {
            var text = $"Player {Winner + 1} wins!";
            Raylib.DrawText(text,
                (Raylib.GetScreenHeight() - 30) / 2,
                (Raylib.GetScreenWidth() - Raylib.MeasureText(text, 30)) / 2,
                30,
                Color.RED
            );
        }
    }
}
