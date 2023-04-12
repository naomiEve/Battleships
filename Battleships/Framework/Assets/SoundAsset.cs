using Raylib_cs;

namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A sound file.
    /// </summary>
    internal class SoundAsset : Asset
    {
        /// <summary>
        /// The raylib sound struct.
        /// </summary>
        private Raylib_cs.Sound _sound;

        /// <summary>
        /// Load a sound from the file.
        /// </summary>
        /// <param name="path">The file.</param>
        public override void LoadFromFile(string path)
        {
            Path = path;
            _sound = Raylib.LoadSound(path);
        }

        /// <summary>
        /// Plays this sound.
        /// </summary>
        public void Play()
        {
            Raylib.PlaySoundMulti(_sound);
        }
    }
}
