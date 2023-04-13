using Battleships.Framework.Assets;
using Battleships.Framework.Data;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering;
using Raylib_cs;

namespace Battleships.Framework
{
    /// <summary>
    /// The game.
    /// </summary>
    internal abstract class Game
    {
        /// <summary>
        /// The launch options of this game.
        /// </summary>
        protected LaunchOptions _launchOptions;

        /// <summary>
        /// The dimensions of the game window.
        /// </summary>
        public Vector2Int Dimensions { get; private set; }

        /// <summary>
        /// The title of the game window.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Should the game window close after the next update cycle finishes?
        /// </summary>
        public bool ShouldClose { get; protected set; }

        /// <summary>
        /// The game renderer.
        /// </summary>
        public IGameRenderer? CurrentRenderer { get; protected set; }

        /// <summary>
        /// This game's asset database.
        /// </summary>
        public AssetDatabase AssetDatabase { get; private set; }

        /// <summary>
        /// A list of all the game objects.
        /// </summary>
        private List<GameObject> _gameObjects;

        /// <summary>
        /// Construct a new game window.
        /// </summary>
        /// <param name="dimensions">The dimensions.</param>
        /// <param name="name">The name of the window.</param>
        /// <param name="opts">The launch options.</param>
        public Game(Vector2Int dimensions, string name, LaunchOptions opts)
        {
            Dimensions = dimensions;
            Title = name;
            _launchOptions = opts;

            _gameObjects = new List<GameObject>();
            AssetDatabase = new AssetDatabase();
        }

        /// <summary>
        /// Sets the framerate limit of this game window.
        /// </summary>
        /// <param name="limit">The limit.</param>
        public void SetFramerateLimit(int limit)
        {
            Raylib.SetTargetFPS(limit);
        }

        /// <summary>
        /// Runs the game loop.
        /// </summary>
        public void Run()
        {
            Preinitialize();

            Raylib.InitWindow(Dimensions.X, Dimensions.Y, Title);
            Raylib.InitAudioDevice();

            Start();

            while (!Raylib.WindowShouldClose())
            {
                Update(Raylib.GetFrameTime());
                if (ShouldClose)
                    break;

                CurrentRenderer!.Begin();
                Draw();
                CurrentRenderer!.End();

                Raylib.BeginDrawing();
                CurrentRenderer!.Blit();
                DrawUI();
                Raylib.EndDrawing();
            }

            Destroy();
            Raylib.CloseWindow();
        }

        /// <summary>
        /// Adds a new game object.
        /// </summary>
        /// <typeparam name="TGameObject">The game object type.</typeparam>
        /// <returns>The newly created game object.</returns>
        public TGameObject AddGameObject<TGameObject>()
            where TGameObject : GameObject, new()
        {
            var obj = new TGameObject();
            
            // If this object is a singleton, find if we perhaps already have one instantiated.
            if (obj is ISingletonObject)
            {
                var singleton = _gameObjects.Find(obj => obj is ISingletonObject && obj is TGameObject);
                if (singleton != null)
                    return (singleton as TGameObject)!;
            }

            obj.SetGame(this);
            _gameObjects.Add(obj);

            obj.Start();

            return obj;
        }

        /// <summary>
        /// Get a game object by its type.
        /// </summary>
        /// <typeparam name="TGameObject">The game object.</typeparam>
        /// <returns>The game object, or nothing.</returns>
        public TGameObject? GetGameObjectOfType<TGameObject>()
            where TGameObject : GameObject
        {
            return _gameObjects.FirstOrDefault(obj => obj is TGameObject) as TGameObject;
        }

        /// <summary>
        /// Cast a ray within the scene.
        /// </summary>
        /// <param name="ray">The ray.</param>
        /// <returns>A ray collision, or nothing.</returns>
        public RayCollision? CastRay(Ray ray)
        {
            // Try to collide with all of the gameobjects that are raycast targets.
            foreach (var obj in _gameObjects)
            {
                if (!obj.Enabled)
                    continue;

                if (obj is not IRaycastTargettableObject rto)
                    continue;
                
                var collision = rto.Collide(ray);
                if (collision.hit)
                    return collision;
            }

            return null;
        }

        /// <summary>
        /// Ran before we initialize anything.
        /// </summary>
        protected virtual void Preinitialize() { }

        /// <summary>
        /// Called right after the Raylib window has been initialized.
        /// </summary>
        protected virtual void Start() { }

        /// <summary>
        /// Runs a single update tick.
        /// </summary>
        /// <param name="dt">The time since the last frame.</param>
        protected virtual void Update(float dt)
        {
            for (var i = 0; i < _gameObjects.Count; i++)
            {
                if (!_gameObjects[i].Enabled)
                    continue;
                
                _gameObjects[i].Update(dt);
            }
        }

        /// <summary>
        /// Draws the game.
        /// </summary>
        protected virtual void Draw()
        {
            foreach (var go in _gameObjects)
            {
                if (!go.Enabled)
                    continue;

                if (go is IDrawableGameObject dgo)
                    dgo.Draw();
            }
        }

        /// <summary>
        /// Draws any UI elements. This needs to be in two passes, as the UI has to be drawn over the game.
        /// </summary>
        protected virtual void DrawUI()
        {
            foreach (var go in _gameObjects)
            {
                if (!go.Enabled)
                    continue;

                if (go is IUIObject uio)
                    uio.DrawUI();
            }
        }

        /// <summary>
        /// Destroys all assets related to the game.
        /// </summary>
        protected virtual void Destroy()
        {
            foreach (var go in _gameObjects)
                go.Destroy();   
        }
    }
}
