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

        /// <summary>
        /// The materials of this model.
        /// </summary>
        public IReadOnlyList<Rendering.Material>? Materials => _materials;

        /// <summary>
        /// The material list.
        /// </summary>
        private List<Rendering.Material>? _materials;

        /// <inheritdoc/>
        public override void LoadFromFile(string path)
        {
            Path = path;
            Model = Raylib.LoadModel(path);

            _materials = new List<Rendering.Material>();
            for (var i = 0; i < Model.materialCount; i++)
                _materials.Add(new Rendering.Material(this, i));
        }
    }
}
