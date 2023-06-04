using System.Runtime.CompilerServices;
using Raylib_cs;

namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A model asset.
    /// </summary>
    internal class ModelAsset : Asset,
        IAssetCanBeRef<Model>
    {
        /// <summary>
        /// The model.
        /// </summary>
        public Model Model => _model;

        /// <summary>
        /// The internal model.
        /// </summary>
        private Model _model;

        /// <summary>
        /// The materials of this model.
        /// </summary>
        public IReadOnlyList<Rendering.Material>? Materials => _materials;

        /// <summary>
        /// The material list.
        /// </summary>
        private List<Rendering.Material>? _materials;

        /// <inheritdoc/>
        public ref Model AsRef() => ref _model;

        /// <inheritdoc/>
        public override void LoadFromFile(string path)
        {
            Path = path;
            _model = Raylib.LoadModel(path);

            _materials = new List<Rendering.Material>();
            for (var i = 0; i < Model.materialCount; i++)
                _materials.Add(new Rendering.Material(this, i));
        }
    }
}
