using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Tweening;
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
        /// The scale of a ship part.
        /// </summary>
        public const float SHIP_PART_SCALE = 0.5f;

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
        /// The cannon attached to this part.
        /// </summary>
        public ShipCannon? Cannon { get; set; }

        /// <summary>
        /// Is this part hit?
        /// </summary>
        public bool Hit { get; set; } = false;

        /// <summary>
        /// Is this part underwater?
        /// </summary>
        public bool Underwater { get; set; } = false;

        /// <summary>
        /// The bobbing offset.
        /// </summary>
        public float BobOffset { get; set; } = 0f;

        /// <summary>
        /// The model this ship has.
        /// </summary>
        private ModelAsset? _model;

        /// <summary>
        /// The rotation while sinking.
        /// </summary>
        private float _sinkRotation;

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
        /// Adds a cannon to this ship part.
        /// </summary>
        public void AddCannon()
        {
            if (Cannon is not null)
                return;

            Cannon = ThisGame!.AddGameObject<ShipCannon>();
            Cannon.Part = this;
        }

        /// <summary>
        /// Sink this piece.
        /// </summary>
        public void Sink()
        {
            if (Underwater)
                return;

            new Tween<float>()
                .WithBeginningValue(InitialPosition.Y)
                .WithEndingValue(InitialPosition.Y - 10)
                .WithTime(5f)
                .WithEasing(TimeEasing.Linear)
                .WithIncrementer(Mathematics.LinearInterpolation)
                .WithUpdateCallback(y => InitialPosition = new Vector3(InitialPosition.X, y, InitialPosition.Z))
                .BindTo(GetGameObjectFromGame<TweenEngine>()!);

            new Tween<float>()
                .WithBeginningValue(0f)
                .WithEndingValue(Random.Shared.NextSingle() * 10f)
                .WithTime(3f)
                .WithEasing(TimeEasing.Linear)
                .WithIncrementer(Mathematics.LinearInterpolation)
                .WithUpdateCallback(rot => _sinkRotation = rot)
                .BindTo(GetGameObjectFromGame<TweenEngine>()!);

            ThisGame!.AddGameObject<ParticleEffect>()
                .WithPosition(Position)
                .WithAtlas(ThisGame.AssetDatabase.Get<TextureAsset>("explosion_atlas")!)
                .WithDuration(1f)
                .Fire();

            Underwater = true;
        }

        /// <inheritdoc/>
        public override void Update(float dt)
        {
            Position = InitialPosition + new Vector3(0f, 0.2f, 0f) * BobOffset;

            if (Cannon is not null)
                Cannon!.Position = Position;
        }

        /// <inheritdoc/>
        public void Draw()
        {
            if (_model == null)
            {
                Raylib.DrawCube(Position, 1f, 1f, 1f, Color.RED);
                return;
            }

            var color = Color.WHITE;
            if (Hit)
                color = Color.GRAY;

            var pos = Position;
            var axis = Vector3.UnitY;
            if (_sinkRotation > float.Epsilon)
                axis += Vector3.UnitX;
            
            if (Ship!.ShipFacing == Ship.Facing.Right)
            {
                pos.X -= 0.5f;
                Raylib.DrawModelEx(_model.Model, pos, axis, 90f + _sinkRotation, new(SHIP_PART_SCALE), color);
            }
            else
            {
                pos.Z -= 0.5f;
                Raylib.DrawModelEx(_model.Model, pos, axis, 0f + _sinkRotation, new(SHIP_PART_SCALE), color);
            }
        }
    }
}
