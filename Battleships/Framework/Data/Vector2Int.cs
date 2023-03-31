namespace Battleships.Framework.Data
{
    /// <summary>
    /// A 2d vector of integers.
    /// </summary>
    internal struct Vector2Int
    {
        public int X, Y;

        /// <summary>
        /// Constructs a new 2d vector of integers.
        /// </summary>
        /// <param name="x">The x part.</param>
        /// <param name="y">The y part.</param>
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
