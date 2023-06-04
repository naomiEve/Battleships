using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships.Game.Objects
{
    /// <summary>
    /// A part of a battleship.
    /// </summary>
    internal class ShipPart : GameObject,
        IDrawableGameObject,
        IPositionedObject
    {
        /// <summary>
        /// The type of the ship part.
        /// </summary>
        public enum PartType
        {
            Head,
            Body,
            Tail
        }

        /// <summary>
        /// The ship part's position.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// The initial position of this ship part.
        /// </summary>
        public Vector3 InitialPosition { get; set; }

        /// <summary>
        /// The type of this ship part.
        /// </summary>
        public PartType Type { get; private set; }

        /// <summary>
        /// The parent ship.
        /// </summary>
        public Ship? Ship { get; set; }

        /// <summary>
        /// Is this part sunk?
        /// </summary>
        public bool Sunk { get; set; } = false;

        /// <summary>
        /// The bobbing offset.
        /// </summary>
        public float BobOffset { get; set; } = 0f;

        /// <summary>
        /// The model this ship has.
        /// </summary>
        private ModelAsset? _model;

        /// <summary>
        /// Set this part's type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void SetType(PartType type)
        {
            Type = type;

            switch (type)
            {
                case PartType.Head:
                    _model = ThisGame?.AssetDatabase.Get<ModelAsset>("ship_head");
                    break;

                case PartType.Body:
                    _model = ThisGame?.AssetDatabase.Get<ModelAsset>("ship_body");
                    break;

                case PartType.Tail:
                    _model = ThisGame?.AssetDatabase.Get<ModelAsset>("ship_tail");
                    break;
            }
        }

        /// <summary>
        /// Sink this piece.
        /// </summary>
        public void Sink()
        {
            if (Sunk)
                return;

            Sunk = true;
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            Position = InitialPosition + new Vector3(0f, 0.2f, 0f) * BobOffset;
        }

        /// <inheritdoc/>
        public void Draw()
        {
            if (_model == null)
            {
                Raylib.DrawCube(Position, 1f, 1f, 1f, Color.RED);
                return;
            }

            if (Ship!.ShipFacing == Ship.Facing.Right)
                Raylib.DrawModelEx(_model.Model, Position, Vector3.UnitY, 90f, new Vector3(0.5f), Color.WHITE);
            else
                Raylib.DrawModel(_model.Model, Position, 0.5f, Color.WHITE);
        }
    }
}
