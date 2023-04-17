using System.Numerics;
using Battleships.Framework;
using Battleships.Framework.Assets;
using Battleships.Framework.Data;
using Battleships.Framework.Rendering;
using Battleships.Framework.Tweening;
using Battleships.Game.Objects;
using Raylib_cs;

namespace Battleships.Game
{
    /// <summary>
    /// The game logic for battleships.
    /// </summary>
    internal class BattleshipsGame : NetworkedGame
    {
        /// <summary>
        /// The camera.
        /// </summary>
        private Camera? _camera;

        /// <summary>
        /// Construct a new battleship logic with the given launch options.
        /// </summary>
        /// <param name="opts">The launch options.</param>
        public BattleshipsGame(LaunchOptions opts)
            : base(new Vector2Int(800, 600), "Battleships", opts)
        {

        }

        /// <inheritdoc/>
        protected override void RegisterMessages()
        {
            
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            AssetDatabase.Load<SoundAsset>("explosion", "./assets/explosion.wav");
            AssetDatabase.Load<SoundAsset>("seagulls", "./assets/seagulls.wav");

            AssetDatabase.Load<ModelAsset>("ship_head", "./assets/ship_head.obj");
            AssetDatabase.Load<ModelAsset>("ship_body", "./assets/ship_body.obj");
            AssetDatabase.Load<ModelAsset>("ship_tail", "./assets/ship_tail.obj");
            AssetDatabase.Load<MusicAsset>("waves", "./assets/waves.ogg");
            AssetDatabase.Load<TextureAsset>("crosshair", "./assets/crosshair.png");

            _camera = AddGameObject<Camera>()
                .WithPosition(new Vector3(0, 10f, 10f))
                .WithFOV(10f)
                .WithProjectionType(CameraProjection.CAMERA_ORTHOGRAPHIC);
            _camera.Rotate(new Vector3(45, 0, 0));
            CurrentRenderer = _camera;

            AddGameObject<GameCoordinator>();
            var ambience = AddGameObject<AmbienceController>();
            ambience.SetMusic(AssetDatabase.Get<MusicAsset>("waves")!);
            ambience.CreateAmbientNoise(AssetDatabase.Get<SoundAsset>("seagulls")!, new Vector2(5f, 25f));

            AddGameObject<DebugObject>();
        }
    }
}
