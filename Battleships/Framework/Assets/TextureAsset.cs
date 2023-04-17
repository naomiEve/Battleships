using Raylib_cs;

namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A texture.
    /// </summary>
    internal class TextureAsset : Asset
    {
        /// <summary>
        /// The texture.
        /// </summary>
        public Texture2D? Texture { get; private set; }

        public override void LoadFromFile(string path)
        {
            Path = path;

            Texture = Raylib.LoadTexture(path);
        }
    }
}
