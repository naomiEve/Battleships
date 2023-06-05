using System.Numerics;
using Battleships.Framework;
using Battleships.Framework.Assets;
using Battleships.Framework.Data;
using Battleships.Framework.Rendering;
using Battleships.Game.Objects;
using Battleships.Game.ShaderPasses;
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

        /// <summary>
        /// Sets up the ship models.
        /// </summary>
        private void SetupShipModels()
        {
            AssetDatabase.Load<TextureAsset>("battleship_red", "./assets/Battleship_Red.png");
            AssetDatabase.Load<TextureAsset>("battleship_blue", "./assets/Battleship_Blue.png");

            void SetBattleshipTexture(ModelAsset asset)
            {
                asset.Materials![0]
                    .SetTexture(MaterialMapIndex.MATERIAL_MAP_ALBEDO, AssetDatabase.Get<TextureAsset>("battleship_blue")!);
            }

            SetBattleshipTexture(AssetDatabase.Load<ModelAsset>("ship_tail", "./assets/back.obj"));
            SetBattleshipTexture(AssetDatabase.Load<ModelAsset>("ship_head", "./assets/front.obj"));
            SetBattleshipTexture(AssetDatabase.Load<ModelAsset>("ship_body", "./assets/middle.obj"));
            SetBattleshipTexture(AssetDatabase.Load<ModelAsset>("ship_cannon", "./assets/cannon.obj"));
        }

        /// <summary>
        /// Sets up the water mesh.
        /// </summary>
        private void SetupWater()
        {
            var waterTexture = AssetDatabase.Load<TextureAsset>("water_tex", "./assets/caustics.png");
            var waterModel = AssetDatabase.Load<ModelAsset>("water_quad", "./assets/water.obj");
            var waterShader = AssetDatabase.Load<ShaderAsset>("water_shader", "./assets/water.sha");

            waterModel.Materials![0]
                .SetTexture(MaterialMapIndex.MATERIAL_MAP_ALBEDO, waterTexture);

            waterModel.Materials![0]
                .SetShader(waterShader);

            AddGameObject<WaterRenderer>();
        }

        /// <summary>
        /// Sets up the announcement controller and the texture it uses.
        /// </summary>
        private void SetupAnnouncements()
        {
            AssetDatabase.Load<SoundAsset>("announcement", "./assets/bowomp.wav");

            AssetDatabase.Load<TextureAsset>("build_your_fleet", "./assets/build_your_fleet.png");
            AssetDatabase.Load<TextureAsset>("your_round", "./assets/your_round.png");
            AssetDatabase.Load<TextureAsset>("opponents_round", "./assets/opponents_round.png");
            AssetDatabase.Load<TextureAsset>("player_1_wins", "./assets/player_1_wins.png");
            AssetDatabase.Load<TextureAsset>("player_2_wins", "./assets/player_2_wins.png");

            AddGameObject<AnnouncementController>();
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            SetupShipModels();
            SetupWater();
            SetupAnnouncements();

            AssetDatabase.Load<ModelAsset>("buoy", "./assets/buoy.obj");

            AssetDatabase.Load<SoundAsset>("explosion", "./assets/explosion.wav");
            AssetDatabase.Load<SoundAsset>("seagulls", "./assets/seagulls.wav");
            AssetDatabase.Load<SoundAsset>("splash", "./assets/splash.wav");

            AssetDatabase.Load<MusicAsset>("waves", "./assets/waves.ogg");
            AssetDatabase.Load<TextureAsset>("crosshair", "./assets/crosshair.png");

            AssetDatabase.Load<TextureAsset>("explosion_atlas", "./assets/explosion_atlas.png")
                .MakeAtlas(new Vector2Int(4, 4), 16);

            AssetDatabase.Load<TextureAsset>("fire_atlas", "./assets/fire_atlas.png")
                .MakeAtlas(new Vector2Int(10, 6), 60);

            AssetDatabase.Load<TextureAsset>("splash_atlas", "./assets/splash_atlas.png")
                .MakeAtlas(new Vector2Int(4, 1), 4);

            AssetDatabase.Load<ModelAsset>("quad", "./assets/quad.obj")
                .Materials![0]
                .SetTexture(MaterialMapIndex.MATERIAL_MAP_ALBEDO, AssetDatabase.Get<TextureAsset>("crosshair")!);

            _camera = AddGameObject<Camera>()
                .WithPosition(new Vector3(0, 10f, 10f))
                .WithFOV(10f)
                .WithProjectionType(CameraProjection.CAMERA_ORTHOGRAPHIC)
                .WithShaderPass<PixelizationShaderPass>();
            _camera.Rotate(new Vector3(45, 0, 0));
            CurrentRenderer = _camera;

            var ambience = AddGameObject<AmbienceController>();
            ambience.SetMusic(AssetDatabase.Get<MusicAsset>("waves")!);
            ambience.CreateAmbientNoise(AssetDatabase.Get<SoundAsset>("seagulls")!, new Vector2(5f, 25f));

            AddGameObject<GameCoordinator>();
            //AddGameObject<ShipCannon>();
            //AddGameObject<DebugObject>();
        }
    }
}
