using Raylib_cs;

namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A sound file.
    /// </summary>
    internal class Sound : Asset
    {
        /// <summary>
        /// The raylib sound struct.
        /// </summary>
        private Raylib_cs.Sound _sound;

        /// <summary>
        /// Create a sound from the file path.
        /// </summary>
        /// <param name="name">The file path.</param>
        public Sound(string path) 
            : base(path) 
        {
        
        }

        /// <summary>
        /// Load a sound from the file.
        /// </summary>
        /// <param name="path">The file.</param>
        protected override void LoadFromFile(string path)
        {
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
