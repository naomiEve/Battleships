using Raylib_cs;

namespace Battleships.Framework.Assets
{
    /// <summary>
    /// A music asset.
    /// </summary>
    internal class MusicAsset : Asset
    {
        /// <summary>
        /// The music.
        /// </summary>
        private Music? _music;

        /// <inheritdoc/>
        public override void LoadFromFile(string path)
        {
            Path = path;

            _music = Raylib.LoadMusicStream(path);
        }

        /// <summary>
        /// Plays this music asset.
        /// </summary>
        public void Play()
        {
            if (_music == null)
                return;

            Raylib.PlayMusicStream(_music.Value);
        }

        /// <summary>
        /// Pauses this music asset.
        /// </summary>
        public void Pause()
        {
            if ( _music == null)
                return;

            Raylib.PauseMusicStream(_music.Value);
        }

        /// <summary>
        /// Stops this music asset.
        /// </summary>
        public void Stop()
        {
            if (_music == null)
                return;

            Raylib.StopMusicStream(_music.Value);
        }
    }
}
