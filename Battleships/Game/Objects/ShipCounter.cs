using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Objects;
using Raylib_cs;

namespace Battleships.Game.Objects;

/// <summary>
/// Life counter for the ships.
/// </summary>
internal class ShipCounter : GameObject,
    IUIObject
{
    /// <summary>
    /// The padding of the counter.
    /// </summary>
    private const float XY_PADDING = 10f;

    /// <summary>
    /// The playfield.
    /// </summary>
    public ShipPlayfield? Playfield { get; set; }

    /// <summary>
    /// The amount of ships left.
    /// </summary>
    public int ShipsLeft { get; set; } = 5;

    /// <summary>
    /// The ship texture.
    /// </summary>
    private TextureAsset? _ship;

    /// <inheritdoc/>
    public override void Start()
    {
        _ship = ThisGame!.AssetDatabase
            .Get<TextureAsset>("galleon");

        ShipsLeft = GameCoordinator.ShipCount;
    }

    /// <inheritdoc/>
    public void DrawUI()
    {
        const float SCALE = 0.075f;

        if (_ship is null || Playfield is null)
            return;

        var tex = _ship.Texture!.Value;

        var ourField = Playfield.Owner == Peer?.PeerId;
        var nextShipOffset = tex.width * SCALE;
        var position = new Vector2
        {
            Y = Raylib.GetScreenHeight() - XY_PADDING - (tex.height * SCALE)
        };

        var color = Color.RED;

        if (ourField)
        {
            color = Color.BLUE;
            position.X = XY_PADDING;
        }
        else
        {
            nextShipOffset *= -1;
            position.X = Raylib.GetScreenWidth() - XY_PADDING + nextShipOffset;
        }

        color.a = 128;

        for (var i = 0; i < ShipsLeft; i++)
        {
            Raylib.DrawTextureEx(_ship.Texture!.Value, position, 0f, SCALE, color);
            position.X += nextShipOffset;
        }
    }
}
