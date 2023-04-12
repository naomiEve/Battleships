using Raylib_cs;

namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A model asset.
    /// </summary>
    internal class ModelAsset : Asset
    {
        /// <summary>
        /// The model.
        /// </summary>
        public Model Model { get; private set; }

        /// <inheritdoc/>
        public override void LoadFromFile(string path)
        {
            Path = path;
            Model = Raylib.LoadModel(path);
        }
    }
}
