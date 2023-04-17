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

        /// <summary>
        /// Is this asset playing?
        /// </summary>
        public bool IsPlaying { get; private set; } = false;

        /// <inheritdoc/>
        public override void LoadFromFile(string path)
        {
            Path = path;

            _music = Raylib.LoadMusicStream(path);
        }

        /// <summary>
        /// Updates the music stream.
        /// </summary>
        public void UpdateStream()
        {
            if (_music == null)
                return;

            Raylib.UpdateMusicStream(_music.Value);
        }

        /// <summary>
        /// Plays this music asset.
        /// </summary>
        public void Play()
        {
            if (_music == null)
                return;

            Raylib.PlayMusicStream(_music.Value);
            IsPlaying = true;
        }

        /// <summary>
        /// Pauses this music asset.
        /// </summary>
        public void Pause()
        {
            if ( _music == null)
                return;

            Raylib.PauseMusicStream(_music.Value);
            IsPlaying = false;
        }

        /// <summary>
        /// Stops this music asset.
        /// </summary>
        public void Stop()
        {
            if (_music == null)
                return;

            Raylib.StopMusicStream(_music.Value);
            IsPlaying = false;
        }
    }
}
