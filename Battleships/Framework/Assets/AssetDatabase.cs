namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A database for easy lookups of all loaded assets.
    /// </summary>
    internal sealed class AssetDatabase
    {
        /// <summary>
        /// The database.
        /// </summary>
        private Dictionary<string, Asset> _assets;

        /// <summary>
        /// Creates a new asset database.
        /// </summary>
        public AssetDatabase()
        {
            _assets = new Dictionary<string, Asset>();
        }

        /// <summary>
        /// Load an asset and add it to the database via a given name.
        /// </summary>
        /// <typeparam name="TAsset">The type of the asset.</typeparam>
        /// <param name="name">The name of the asset.</param>
        /// <param name="path">The path to the asset.</param>
        /// <returns>The loaded asset.</returns>
        public TAsset Load<TAsset>(string name, string path)
            where TAsset : Asset, new()
        {
            var asset = new TAsset();
            asset.LoadFromFile(path);
            _assets.Add(name, asset);

            return asset;
        }

        /// <summary>
        /// Gets an asset of a type by its name.
        /// </summary>
        /// <typeparam name="TAsset">The asset type.</typeparam>
        /// <param name="name">The asset name.</param>
        /// <returns>The asset, or nothing.</returns>
        public TAsset? Get<TAsset>(string name)
            where TAsset : Asset
        {
            if (!_assets.TryGetValue(name, out var asset))
                return default;

            if (asset is not TAsset typedAsset)
                return default;

            return typedAsset;
        }
    }
}
