using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A simplistic particle effect.
    /// </summary>
    internal partial class ParticleEffect : GameObject,
        IDrawableGameObject,
        IPositionedObject
    {
        /// <summary>
        /// The atlas sheet associated with this particle effect.
        /// </summary>
        public TextureAsset? Atlas { get; set; }

        /// <summary>
        /// Is this particle effect playing?
        /// </summary>
        public bool Playing { get; private set; } = false;

        /// <summary>
        /// The duration of this particle effect.
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// The position.
        /// </summary>
        public Vector3 Position { get; private set; }

        /// <summary>
        /// Is this particle effect looping?
        /// </summary>
        public bool Looping { get; private set; } = false;

        /// <summary>
        /// Followed object.
        /// </summary>
        public IPositionedObject? FollowedObject { get; private set; }

        /// <summary>
        /// The offset from the followed object.
        /// </summary>
        public Vector3 FollowedObjectOffset { get; private set; }

        /// <summary>
        /// The camera.
        /// </summary>
        private Camera? _camera;

        /// <summary>
        /// The current atlas index.
        /// </summary>
        public int _atlasIndex = 0;

        /// <summary>
        /// The starting time.
        /// </summary>
        public float _elapsed;

        /// <inheritdoc/>
        public override void Start()
        {
            _camera = GetGameObjectFromGame<Camera>();
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            if (!Playing)
                return;

            if (FollowedObject is not null)
                Position = FollowedObject.Position + FollowedObjectOffset;

            _elapsed += dt;

            if (_elapsed > Duration)
            {
                if (!Looping)
                    Playing = false;
                else
                    _elapsed = 0f;
            }
        }

        /// <inheritdoc/>
        public void Draw()
        {
            if (!Playing || Atlas == null)
                return;

            var index = (int)(Mathematics.LinearInterpolation(0, Atlas.FrameCount, _elapsed / Duration));
            var grid = Atlas.FrameIndexToGridIndex(index);
            if (grid == null)
                return;

            var rect = Atlas.GetAtlasRectForGridIndex(grid.Value);
            if (rect == null)
                return;

            Raylib.DrawBillboardRec(_camera!.BackingCamera, Atlas.Texture!.Value, rect.Value, Position, new Vector2(1f, 2f), Color.WHITE);
        }
    }
}
