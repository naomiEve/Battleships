using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Tweening;
using Raylib_cs;

namespace Battleships.Game.Objects;

/// <summary>
/// The ship debris.
/// </summary>
internal class ShipDebris : GameObject,
    IDrawableGameObject,
    IPositionedObject
{
    /// <inheritdoc/>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The facing of this part.
    /// </summary>
    public Ship.Facing Facing { get; set; }

    /// <summary>
    /// The playfield this debris belongs to.
    /// </summary>
    public ShipPlayfield? Playfield { get; set; }

    /// <summary>
    /// The model.
    /// </summary>
    private ModelAsset? _model;

    /// <summary>
    /// The bob offset.
    /// </summary>
    private float _offset;

    /// <summary>
    /// The color.
    /// </summary>
    private Color _color;

    /// <inheritdoc/>
    public override void Start()
    {
        _model = ThisGame!
            .AssetDatabase.Get<ModelAsset>("ship_body");

        _color = Color.GRAY;
        _color.a = 200;
    }

    /// <inheritdoc/>
    public override void Update(float dt)
    {
        _offset += dt;
    }

    /// <summary>
    /// Makes the debris float up.
    /// </summary>
    public void FloatUp()
    {
        var playedSplash = false;

        new Tween<Vector3>()
            .WithBeginningValue(Position - new Vector3(0, 1.4f, 0))
            .WithEndingValue(Position)
            .WithTime(3f)
            .WithEasing(TimeEasing.OutElastic)
            .WithIncrementer((a, b, t) =>
            {
                if (t >= 1 && !playedSplash)
                {
                    Playfield!.SpawnSplashAt(Playfield.PositionToFieldCoordinates(Position)!.Value);
                    playedSplash = true;
                }

                return a.LinearInterpolation(b, t);
            })
            .WithUpdateCallback(pos => Position = pos)
            .WithFinishedCallback(pos => Position = pos)
            .BindTo(GetGameObjectFromGame<TweenEngine>()!);
    }

    /// <inheritdoc/>
    public void Draw()
    {
        if (_model is null)
            return;

        var pos = Position;
        pos.Y += MathF.Sin(_offset * 3f) * 0.1f;

        // Scaling tricks to invert the model.
        var scale = new Vector3(ShipPart.SHIP_PART_SCALE, -ShipPart.SHIP_PART_SCALE, ShipPart.SHIP_PART_SCALE);

        if (Facing == Ship.Facing.Right)
        {
            pos.X -= 0.5f;
            Raylib.DrawModelEx(_model.Model, pos, Vector3.UnitY, 90f, scale, _color);
        }
        else
        {
            pos.Z -= 0.5f;
            Raylib.DrawModelEx(_model.Model, pos, Vector3.Zero, 0f, scale, _color);
        }
    }
}
