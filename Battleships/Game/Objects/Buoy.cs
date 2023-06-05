using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships.Game.Objects;

/// <summary>
/// A bobbing buoy.
/// </summary>
internal class Buoy : GameObject,
    IDrawableGameObject,
    IPositionedObject
{
    /// <inheritdoc/>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The model of this buoy.
    /// </summary>
    public ModelAsset? Model { get; private set; }

    /// <summary>
    /// The time we've begun at.
    /// </summary>
    private float _beginningTime;

    /// <inheritdoc/>
    public override void Start()
    {
        Model = ThisGame!
            .AssetDatabase.Get<ModelAsset>("buoy");

        _beginningTime = (float)Raylib.GetTime() + (Random.Shared.NextSingle() * 10f);
    }

    /// <inheritdoc/>
    public void Draw()
    {
        if (Model is null)
            return;

        Raylib.DrawModelEx(
            Model!.Model, 
            Position, 
            Vector3.UnitX + Vector3.UnitZ, 
            MathF.Sin((float)(Raylib.GetTime() - _beginningTime) * 3f) * 10f,
            new Vector3(0.3f), 
            Color.WHITE
        );
    }
}
