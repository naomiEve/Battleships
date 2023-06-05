using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Objects;

namespace Battleships.Game.Objects
{
    internal partial class ParticleEffect
    {
        /// <summary>
        /// Sets the particle atlas for this texture.
        /// </summary>
        /// <param name="asset">The texture atlas.</param>
        public ParticleEffect WithAtlas(TextureAsset asset)
        {
            Atlas = asset;
            return this;
        }

        /// <summary>
        /// Sets the duration of this particle effect.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public ParticleEffect WithDuration(float duration)
        {
            Duration = duration;
            return this;
        }

        /// <summary>
        /// Sets the position of this particle effect.
        /// </summary>
        /// <param name="position">The position.</param>
        public ParticleEffect WithPosition(Vector3 position)
        {
            Position = position;
            return this;
        }

        /// <summary>
        /// Sets the looping of this particle effect.
        /// </summary>
        /// <param name="looping">Should it be looping?</param>
        public ParticleEffect WithLooping(bool looping)
        {
            Looping = looping;
            return this;
        }

        /// <summary>
        /// Makes this particle effect follow an object.
        /// </summary>
        /// <param name="object">The object.</param>
        public ParticleEffect Following(IPositionedObject @object, Vector3 offset)
        {
            FollowedObject = @object;
            FollowedObjectOffset = offset;
            return this;
        }

        /// <summary>
        /// Plays this particle effect.
        /// </summary>
        public ParticleEffect Fire()
        {
            Playing = true;
            return this;
        }
    }
}
