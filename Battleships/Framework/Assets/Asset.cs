namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A generic asset file.
    /// </summary>
    internal abstract class Asset
    {
        /// <summary>
        /// The path to this asset.
        /// </summary>
        public string? Path { get; protected set; }

        /// <summary>
        /// Load an asset from the path.
        /// </summary>
        /// <param name="path">The path.</param>
        public abstract void LoadFromFile(string path);
    }
}
