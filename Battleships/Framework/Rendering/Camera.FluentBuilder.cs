using System.Numerics;
using Battleships.Framework.Shaders;
using Raylib_cs;

namespace Battleships.Framework.Rendering
{
    internal partial class Camera
    {
        /// <summary>
        /// Sets this camera's position.
        /// </summary>
        /// <param name="position">The position.</param>
        public Camera WithPosition(Vector3 position)
        {
            _camera.position = position;
            return this;
        }

        /// <summary>
        /// Sets this camera's fov.
        /// </summary>
        /// <param name="fov">The fov.</param>
        public Camera WithFOV(float fov)
        {
            _camera.fovy = fov;
            return this;
        }

        /// <summary>
        /// Sets the projection type.
        /// </summary>
        /// <param name="projection">The projection type.</param>
        public Camera WithProjectionType(CameraProjection projection)
        {
            _camera.projection = projection;
            return this;
        }

        /// <summary>
        /// Adds a shader pass.
        /// </summary>
        /// <typeparam name="TShaderPass">The type of the shader pass.</typeparam>
        public Camera WithShaderPass<TShaderPass>()
            where TShaderPass : ShaderPass, new()
        {
            AddShaderPass<TShaderPass>();
            return this;
        }
    }
}
