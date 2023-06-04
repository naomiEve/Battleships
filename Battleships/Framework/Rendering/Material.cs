using Battleships.Framework.Assets;
using Raylib_cs;

namespace Battleships.Framework.Rendering
{
    /// <summary>
    /// Wrapper class for model materials.
    /// </summary>
    internal class Material
    {
        /// <summary>
        /// The index of this material within the 
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The model this material is bound to.
        /// </summary>
        public ModelAsset Model { get; private set; }

        /// <summary>
        /// Construct a new material wrapper for the model and index.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="index">The material index within the model.</param>
        public Material(ModelAsset model, int index)
        {
            Model = model;
            Index = index;
        }

        /// <summary>
        /// Set a texture for this material.
        /// </summary>
        /// <param name="asset">The material.</param>
        public unsafe void SetTexture(MaterialMapIndex mapIndex, TextureAsset asset)
        {
            if (asset?.Texture == null)
                return;

            var model = Model.Model;
            Raylib.SetMaterialTexture(ref model.materials[Index], 
                mapIndex, 
                asset.Texture.Value
            );
        }

        /// <summary>
        /// Set a shader for this model.
        /// </summary>
        /// <param name="shader">The shader.</param>
        public unsafe void SetShader(ShaderAsset shader)
        {
            if (shader?.Shader == null)
                return;

            Raylib.SetMaterialShader(
                ref Model.AsRef(),
                Index,
                ref shader.AsRef()
            );
        }
    }
}
