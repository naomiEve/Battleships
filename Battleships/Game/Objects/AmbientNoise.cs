using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// An ambient noise that plays at random.
    /// </summary>
    internal class AmbientNoise : GameObject
    {
        /// <summary>
        /// The sound associated with this noise.
        /// </summary>
        public SoundAsset? Sound { get; set; }

        /// <summary>
        /// The random boundaries for when to play this noise. (between X and Y seconds).
        /// </summary>
        public Vector2 RandomBounds { get; set; }

        /// <summary>
        /// The last time this sound has played.
        /// </summary>
        public float _lastSoundTime;

        /// <summary>
        /// The current delay.
        /// </summary>
        public float _lastRandomDelay;

        /// <summary>
        /// Reset this noise.
        /// </summary>
        public void ResetDelay()
        {
            _lastRandomDelay = Random.Shared.NextSingle() * (RandomBounds.Y - RandomBounds.X) + RandomBounds.X;
            _lastSoundTime = (float)Raylib.GetTime();
        }

        /// <summary>
        /// Play this sound.
        /// </summary>
        private void PlaySound()
        {
            Sound?.Play();
            ResetDelay();
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            if (Raylib.GetTime() > _lastRandomDelay + _lastSoundTime)
                PlaySound();
        }
    }
}
