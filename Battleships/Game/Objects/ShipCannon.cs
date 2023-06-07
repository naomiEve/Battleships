using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Tweening;
using Raylib_cs;

namespace Battleships.Game.Objects;

/// <summary>
/// A cannon for a ship.
/// </summary>
internal class ShipCannon : GameObject,
    IDrawableGameObject,
    IPositionedObject
{
    /// <summary>
    /// The position of this object.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The part we're attached to.
    /// </summary>
    public ShipPart? Part { get; set; }

    /// <summary>
    /// The scale.
    /// </summary>
    private float _scale = .5f;

    /// <summary>
    /// The model.
    /// </summary>
    private ModelAsset? _cannonModel;

    /// <inheritdoc/>
    public override void Start()
    {
        _cannonModel = ThisGame!.AssetDatabase
            .Get<ModelAsset>("ship_cannon");
    }

    /// <summary>
    /// Plays the shooting.
    /// </summary>
    public void PlayShooting()
    {
        var pos = Position;
        pos.Y += 0.5f;
        if (Part!.Ship!.ShipFacing == Ship.Facing.Right)
            pos.X -= 0.5f;
        else
            pos.Z -= 0.5f;

        ThisGame!.AddGameObject<ParticleEffect>()
            .WithPosition(pos)
            .WithAtlas(ThisGame.AssetDatabase.Get<TextureAsset>("explosion_atlas")!)
            .WithDuration(1f)
            .Fire();

        ThisGame!.AssetDatabase
            .Get<SoundAsset>("cannon_shoot")?
            .Play();

        new Tween<float>()
            .WithBeginningValue(.6f)
            .WithEndingValue(.5f)
            .WithTime(1f)
            .WithEasing(TimeEasing.OutElastic)
            .WithIncrementer(Mathematics.LinearInterpolation)
            .WithUpdateCallback(f => _scale = f)
            .WithFinishedCallback(f => _scale = f)
            .BindTo(GetGameObjectFromGame<TweenEngine>()!);
    }

    /// <inheritdoc/>
    public void Draw()
    {
        if (_cannonModel is null || Part is null)
            return;

        var pos = Position;

        if (Part!.Ship!.ShipFacing == Ship.Facing.Right)
        {
            pos.X -= 0.5f;
            Raylib.DrawModelEx(_cannonModel!.Model, pos, Vector3.UnitY, 90f, new(_scale), Color.WHITE);
        }
        else
        {
            pos.Z -= 0.5f;
            Raylib.DrawModelEx(_cannonModel!.Model, pos, Vector3.UnitY, 0f, new(_scale), Color.WHITE);
        }
    }
}
