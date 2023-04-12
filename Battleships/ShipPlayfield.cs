using System.Numerics;
using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships
{
    /// <summary>
    /// A playfield for battleships.
    /// </summary>
    internal class ShipPlayfield : GameObject, 
        IDrawableGameObject, 
        IRaycastTargettableObject
    {
        /// <summary>
        /// The field.
        /// </summary>
        private readonly CubeRenderer[,] _field;

        /// <summary>
        /// The preview cube.
        /// </summary>
        private CubeRenderer? _previewCube;

        /// <summary>
        /// Construct a new ship playfield.
        /// </summary>
        public ShipPlayfield()
        {
            _field = new CubeRenderer[10, 10];
        }

        /// <inheritdoc/>
        public override void Start()
        {
            _previewCube = ThisGame!.AddGameObject<CubeRenderer>();    
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            
        }

        /// <inheritdoc/>
        public void Draw()
        {
            Raylib.DrawGrid(10, 1.0f);
        }

        /// <inheritdoc/>
        public RayCollision Collide(Ray ray)
        {
            return Raylib.GetRayCollisionQuad(ray, new Vector3(-5f, 0f, -5f), new Vector3(-5f, 0f, 5f), new Vector3(5f, 0f, 5f), new Vector3(5f, 0f, -5f)); ;
        }
    }
}
