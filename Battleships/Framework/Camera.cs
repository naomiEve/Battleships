using System.Numerics;
using Raylib_cs;

namespace Battleships.Framework
{
    /// <summary>
    /// A camera.
    /// </summary>
    internal class Camera
    {
        /// <summary>
        /// The camera we're using.
        /// </summary>
        private Camera3D _camera;

        /// <summary>
        /// Construct a new camera.
        /// </summary>
        public Camera()
        {
            _camera = new Camera3D(new Vector3(0f, 10f, 10f), Vector3.Zero, new Vector3(0f, 1f, 0f), 10f, CameraProjection.CAMERA_ORTHOGRAPHIC);
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

        public void Begin()
        {
            Raylib.BeginMode3D(_camera);
        }

        public void End()
        {
            Raylib.EndMode3D();
        }
    }
}
