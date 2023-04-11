using System.Numerics;
using Battleships.Framework.Shaders;
using Raylib_cs;

namespace Battleships.Framework.Rendering
{
    /// <summary>
    /// A camera.
    /// </summary>
    internal class Camera : IGameRenderer
    {
        /// <summary>
        /// The camera we're using.
        /// </summary>
        private Camera3D _camera;

        /// <summary>
        /// The list of shader passes.
        /// </summary>
        private List<ShaderPass>? _shaderPasses;

        /// <summary>
        /// The backing render texture for this camera.
        /// </summary>
        private RenderTexture2D? _backingTexture;

        /// <summary>
        /// Construct a new camera.
        /// </summary>
        public Camera()
        {
            _camera = new Camera3D(
                new Vector3(0f, 10f, 10f),
                Vector3.Zero,
                new Vector3(0f, 1f, 0f),
                10f,
                CameraProjection.CAMERA_ORTHOGRAPHIC
            );
        }

        /// <summary>
        /// Adds a new shader pass to this camera's shader pass stack.
        /// </summary>
        /// <typeparam name="TPass">The shader pass.</typeparam>
        public void AddShaderPass<TPass>()
            where TPass : ShaderPass, new()
        {
            _shaderPasses ??= new List<ShaderPass>();
            _shaderPasses.Add(new TPass());

            // Construct a backing render texture in case one doesn't exist.
            _backingTexture ??= Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }

        public Vector3 GetForward()
        {
            return Raymath.Vector3Normalize(Raymath.Vector3Subtract(_camera.target, _camera.position));
        }

        public Ray MouseRay(Vector2 mousePosition)
        {
            return Raylib.GetMouseRay(mousePosition, _camera);
        }

        public void RotateY(float angle)
        {
            var mtx = Raymath.MatrixRotate(Raymath.Vector3Normalize(_camera.up), angle * (MathF.PI / 180f));
            var view = Raymath.Vector3Subtract(_camera.position, _camera.target);
            view = Raymath.Vector3Transform(view, mtx);
            _camera.position = Raymath.Vector3Add(_camera.target, view);
        }

        public void Rotate(Vector3 angles)
        {
            /*var mtx = Raymath.MatrixRotate(Raymath.Vector3Normalize(_camera.up), angle * (MathF.PI / 180f));
            var view = Raymath.Vector3Subtract(_camera.position, _camera.target);
            view = Raymath.Vector3Transform(view, mtx);
            _camera.position = Raymath.Vector3Add(_camera.target, view);*/
        }

        private void Blit()
        {
            // This only makes sense if we're using any backing textures.
            if (_backingTexture == null)
                return;

            Raylib.BeginDrawing();

            for (var i = 0; i < _shaderPasses!.Count; i++)
                _shaderPasses[i].Begin();

            var rect = new Rectangle(0, 0, _backingTexture.Value.texture.width, -_backingTexture.Value.texture.height);
            Raylib.DrawTextureRec(_backingTexture.Value.texture, rect, Vector2.Zero, Color.WHITE);

            for (var i = _shaderPasses!.Count - 1; i >= 0; i--)
                _shaderPasses[i].End();

            Raylib.EndDrawing();
        }

        public void Begin()
        {
            if (_backingTexture != null)
                Raylib.BeginTextureMode(_backingTexture.Value);
            else
                Raylib.BeginDrawing();

            Raylib.BeginMode3D(_camera);
            Raylib.ClearBackground(Color.WHITE);
        }

        public void End()
        {
            Raylib.EndMode3D();

            if (_backingTexture != null)
                Raylib.EndTextureMode();
            else
                Raylib.EndDrawing();

            Blit();
        }
    }
}
