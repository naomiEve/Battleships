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

        /// <inheritdoc/>
        public static bool operator==(Vector2Int a, Vector2Int b)
        {
            return a.X == b.X 
                && a.Y == b.Y;
        }

        /// <inheritdoc/>
        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Vector2Int vec && 
                this == vec;
        }

        /// <inheritdoc
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
