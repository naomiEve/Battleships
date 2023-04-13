using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Objects;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A controller for any in-game ambience.
    /// </summary>
    internal class AmbienceController : GameObject
    {
        /// <summary>
        /// The currently playing music.
        /// </summary>
        public MusicAsset? CurrentMusic { get; private set; }

        /// <summary>
        /// The ambient noise list.
        /// </summary>
        private readonly List<AmbientNoise> _ambientNoiseList;

        /// <summary>
        /// Creates a new ambient controller.
        /// </summary>
        public AmbienceController()
        {
            _ambientNoiseList = new List<AmbientNoise>();
        }

        /// <summary>
        /// Sets the currently playing music.
        /// </summary>
        /// <param name="asset">The new music asset.</param>
        public void SetMusic(MusicAsset asset)
        {
            CurrentMusic?.Stop();

            CurrentMusic = asset;
            CurrentMusic?.Play();
        }

        /// <summary>
        /// Creates an ambient noise.
        /// </summary>
        /// <param name="asset">The sound asset.</param>
        /// <param name="delayBounds">The bounds for the delay.</param>
        public void CreateAmbientNoise(SoundAsset asset, Vector2 delayBounds)
        {
            var noise = ThisGame!.AddGameObject<AmbientNoise>();
            noise.RandomBounds = delayBounds;
            noise.Sound = asset;
            noise.ResetDelay();

            _ambientNoiseList.Add(noise);
        }
    }
}
