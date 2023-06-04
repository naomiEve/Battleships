using System.Numerics;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering.ShaderPasses;
using Raylib_cs;

namespace Battleships.Framework.Rendering
{
    /// <summary>
    /// A camera.
    /// </summary>
    internal partial class Camera : GameObject, 
        IGameRenderer
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
        private RenderTexture2D _backingTexture;

        /// <summary>
        /// Forward vector.
        /// </summary>
        public Vector3 Forward => Raymath.Vector3Normalize(Raymath.Vector3Subtract(_camera.target, _camera.position));

        /// <summary>
        /// Up vector.
        /// </summary>
        public Vector3 Up => Raymath.Vector3Normalize(_camera.up);

        /// <summary>
        /// Right vector.
        /// </summary>
        public Vector3 Right => Raymath.Vector3CrossProduct(Forward, Up);

        /// <summary>
        /// The camera's position.
        /// </summary>
        public Vector3 Position => _camera.position;

        /// <summary>
        /// The current rotation vector.
        /// </summary>
        public Vector3 Rotation { get; private set; }

        /// <summary>
        /// The target.
        /// </summary>
        public Vector3 Target => _camera.target;

        /// <summary>
        /// The backing camera.
        /// </summary>
        public Camera3D BackingCamera => _camera;

        /// <summary>
        /// Construct a new camera.
        /// </summary>
        public Camera()
        {
            _camera = new Camera3D(
                Vector3.Zero,
                Vector3.Zero,
                Vector3.UnitY,
                90f,
                CameraProjection.CAMERA_PERSPECTIVE
            );

            Rotation = Vector3.Zero;

            _backingTexture = Raylib.LoadRenderTexture(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
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
        }

        /// <summary>
        /// Constructs a ray from the screen space mouse position facing the camera's forward.
        /// </summary>
        /// <param name="mousePosition">The screen space mouse position</param>
        /// <returns>The constructed ray.</returns>
        public Ray MouseRay(Vector2 mousePosition)
        {
            return Raylib.GetMouseRay(mousePosition, _camera);
        }

        /// <summary>
        /// Sets the camera's target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void SetTarget(Vector3 target)
        {
            _camera.target = target;
        }

        /// <summary>
        /// Moves the camera a set distance forward.
        /// </summary>
        /// <param name="distance">The distance.</param>
        /// <param name="moveInWorldPlane">Should we move in the world plane?</param>
        public void MoveForward(float distance, bool moveInWorldPlane)
        {
            var forward = Forward;

            if (moveInWorldPlane)
            {
                forward.Y = 0;
                forward = Raymath.Vector3Normalize(forward);
            }

            forward = Raymath.Vector3Scale(forward, distance);
            _camera.position = Raymath.Vector3Add(_camera.position, forward);
            _camera.target = Raymath.Vector3Add(_camera.target, forward);
        }

        /// <summary>
        /// Moves the camera up a certain distance.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        public void MoveUp(float distance)
        {
            var up = Up;
            up = Raymath.Vector3Scale(up, distance);
            _camera.position = Raymath.Vector3Add(_camera.position, up);
            _camera.target = Raymath.Vector3Add(_camera.target, up);
        }

        /// <summary>
        /// Moves the camera right a certain distance.
        /// </summary>
        /// <param name="distance">The distance to move.</param>
        /// <param name="moveInWorldPlane">Should we move in the world plane?</param>
        public void MoveRight(float distance, bool moveInWorldPlane)
        {
            var right = Right;

            if (moveInWorldPlane)
            {
                right.Y = 0;
                right = Raymath.Vector3Normalize(right);
            }

            right = Raymath.Vector3Scale(right, distance);
            _camera.position = Raymath.Vector3Add(_camera.position, right);
            _camera.target = Raymath.Vector3Add(_camera.target, right);
        }

        /// <summary>
        /// Moves the camera.
        /// </summary>
        /// <param name="delta">The delta movement vector.</param>
        public void Move(Vector3 delta)
        {
            MoveForward(delta.Z, true);
            MoveUp(delta.Y);
            MoveRight(delta.X, true);
        }


        /// <summary>
        /// Snaps to a position.
        /// </summary>
        /// <param name="target">The position.</param>
        public void SnapTo(Vector3 target)
        {
            var fwd = Forward;
            fwd.Y = 0;
            fwd = Raymath.Vector3Normalize(fwd);

            var right = Right;
            right.Y = 0;
            right = Raymath.Vector3Normalize(right);

            var l = target - Target;
            l.X /= fwd.X;
            l.Z /= fwd.Z;

            var l2 = target - Target;
            l2.X /= right.X;
            l2.Z /= right.Z;

            Move(new Vector3(l.X / 2f, 0f, l2.X / 2f));

            //Console.WriteLine($"Should snap to: {target}, actual target: {Target}");
        }

        /// <summary>
        /// Sets the camera's yaw.
        /// </summary>
        /// <param name="angle">The angle to rotate by.</param>
        /// <param name="rotateAroundTarget">Should we rotate around the camera's target?</param>
        public void RotateYaw(float angle, bool rotateAroundTarget)
        {
            var up = Up;
            var targetPosition = Raymath.Vector3Subtract(_camera.target, _camera.position);
            targetPosition = Raymath.Vector3RotateByAxisAngle(targetPosition, up, angle);

            if (rotateAroundTarget)
                _camera.position = Raymath.Vector3Subtract(_camera.target, targetPosition);
            else
                _camera.target = Raymath.Vector3Add(_camera.position, targetPosition);
        }

        /// <summary>
        /// Gets the angle between two vector3s.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The angle between them.</returns>
        private float Vector3Angle(Vector3 v1, Vector3 v2)
        {
            return MathF.Acos(Raymath.Vector3DotProduct(v1, v2) / (Raymath.Vector3Length(v1) * Raymath.Vector3Length(v2)));
        }

        /// <summary>
        /// Sets the camera's pitch.
        /// </summary>
        /// <param name="angle">The new angle.</param>
        /// <param name="rotateAroundTarget">Should we rotate around the target?</param>
        public void RotatePitch(float angle, bool lockView, bool rotateAroundTarget, bool rotateUp)
        {
            var up = Up;
            var targetPosition = Raymath.Vector3Subtract(_camera.target, _camera.position);

            if (lockView)
            {
                var maxAngleUp = Vector3Angle(up, targetPosition);
                maxAngleUp -= 0.001f;
                if (angle > maxAngleUp)
                    angle = maxAngleUp;

                var maxAngleDown = Vector3Angle(Raymath.Vector3Negate(up), targetPosition);
                maxAngleDown *= -1.0f;
                maxAngleDown += 0.001f;

                if (angle < maxAngleDown)
                    angle = maxAngleDown;
            }

            var right = Right;
            targetPosition = Raymath.Vector3RotateByAxisAngle(targetPosition, right, angle);

            if (rotateAroundTarget)
                _camera.position = Raymath.Vector3Subtract(_camera.target, targetPosition);
            else
                _camera.target = Raymath.Vector3Add(_camera.position, targetPosition);

            if (rotateUp)
                _camera.up = Raymath.Vector3RotateByAxisAngle(_camera.up, right, angle);
        }

        /// <summary>
        /// Sets the camera's roll.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void RotateRoll(float angle)
        {
            var forward = Forward;
            _camera.up = Raymath.Vector3RotateByAxisAngle(_camera.up, forward, angle);
        }

        /// <summary>
        /// Rotates the camera by the given angular vector.
        /// </summary>
        /// <param name="angles">The vector.</param>
        public void Rotate(Vector3 angles)
        {
            const float DEG2RAD = MathF.PI / 180f;

            RotatePitch(-angles.Y * DEG2RAD, true, true, false);
            RotateYaw(-angles.X * DEG2RAD, true);
            RotateRoll(-angles.Z * DEG2RAD);

            Rotation = angles;
        }

        /// <inheritdoc/>
        public void Blit()
        {
            if (_shaderPasses != null)
            {
                for (var i = 0; i < _shaderPasses!.Count; i++)
                    _shaderPasses[i].Begin();
            }

            var rect = new Rectangle(0, 0, _backingTexture.texture.width, -_backingTexture.texture.height);
            Raylib.DrawTextureRec(_backingTexture.texture, rect, Vector2.Zero, Color.WHITE);

            if (_shaderPasses != null)
            {
                for (var i = _shaderPasses!.Count - 1; i >= 0; i--)
                    _shaderPasses[i].End();
            }
        }

        /// <summary>
        /// Begins drawing.
        /// </summary>
        public void Begin()
        {
            Raylib.BeginTextureMode(_backingTexture);

            Raylib.BeginMode3D(_camera);
            Rlgl.rlEnableDepthTest();
            Raylib.ClearBackground(Color.WHITE);
        }

        /// <summary>
        /// Ends drawing.
        /// </summary>
        public void End()
        {
            Raylib.EndMode3D();
            Raylib.EndTextureMode();
        }
    }
}
