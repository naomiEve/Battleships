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
        public string Path { get; private set; }

        /// <summary>
        /// Construct a new asset from the file path.
        /// </summary>
        /// <param name="path">The file path.</param>
        public Asset(string path)
        {
            Path = path;
            LoadFromFile(path);
        }

        /// <summary>
        /// Load an asset from the path.
        /// </summary>
        /// <param name="path">The path.</param>
        protected abstract void LoadFromFile(string path);
    }
}
