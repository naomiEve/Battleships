using Battleships.Framework.Data;
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

        /// <summary>
        /// Is this texture an atlas?
        /// </summary>
        public bool IsAtlas { get; private set; } = false;

        /// <summary>
        /// The frame count of this atlas.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// The atlas grid size.
        /// </summary>
        public Vector2Int GridSize { get; private set; }

        /// <inheritdoc/>
        public override void LoadFromFile(string path)
        {
            Path = path;
            Texture = Raylib.LoadTexture(path);

            Raylib.SetTextureWrap(Texture!.Value, TextureWrap.TEXTURE_WRAP_REPEAT);
            Raylib.SetTextureFilter(Texture!.Value, TextureFilter.TEXTURE_FILTER_TRILINEAR);
        }

        /// <summary>
        /// Makes this texture asset into an atlas.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="frameCount">The frame count of this atlas.</param>
        public void MakeAtlas(Vector2Int dimensions, int frameCount)
        {
            GridSize = dimensions;
            FrameCount = frameCount;
            IsAtlas = true;
        }

        /// <summary>
        /// Converts a frame index into an atlas grid index.
        /// </summary>
        /// <param name="frameIndex">The frame index.</param>
        /// <returns>The atlas grid index, or nothing.</returns>
        public Vector2Int? FrameIndexToGridIndex(int frameIndex)
        {
            if (!IsAtlas)
                return null;

            if (frameIndex < 0 || frameIndex > FrameCount)
                return null;

            return new Vector2Int(frameIndex % GridSize.X, frameIndex / GridSize.X);
        }

        /// <summary>
        /// Gets the rectangle for this grid index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Either the rectangle for this index's area, or nothing.</returns>
        public Rectangle? GetAtlasRectForGridIndex(Vector2Int index)
        {
            if (!IsAtlas || Texture == null)
                return null;

            var hSplit = Texture.Value.height / GridSize.Y;
            var wSplit = Texture.Value.width / GridSize.X;

            return new Rectangle(
                wSplit * index.X,
                hSplit * index.Y,
                wSplit,
                hSplit);
        }
    }
}
