using Battleships.Framework.Assets;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Tweening;
using Raylib_cs;

namespace Battleships.Game.Objects;

/// <summary>
/// The object responsible for drawing game announcements.
/// </summary>
internal class AnnouncementController : GameObject,
    IUIObject
{
    /// <summary>
    /// The type of the announcement to show.
    /// </summary>
    public enum AnnouncementType
    {
        None,
        BuildYourFleet,
        YourTurn,
        OpponentsTurn,
        Player1Won,
        Player2Won
    }

    /// <summary>
    /// The map from an announcement type to the asset it represents.
    /// </summary>
    private Dictionary<AnnouncementType, TextureAsset>? _map;

    /// <summary>
    /// The current announcement.
    /// </summary>
    private AnnouncementType _currentAnnouncement = AnnouncementType.None;

    /// <summary>
    /// The opacity tween.
    /// </summary>
    private Tween<int>? _opacityTween;

    /// <summary>
    /// The opacity of this announcement.
    /// </summary>
    private int _opacity = 255;

    /// <inheritdoc/>
    public override void Start()
    {
        SetTextureFor(AnnouncementType.BuildYourFleet, ThisGame!.AssetDatabase.Get<TextureAsset>("build_your_fleet")!);
        SetTextureFor(AnnouncementType.YourTurn, ThisGame!.AssetDatabase.Get<TextureAsset>("your_round")!);
        SetTextureFor(AnnouncementType.OpponentsTurn, ThisGame!.AssetDatabase.Get<TextureAsset>("opponents_round")!);
        SetTextureFor(AnnouncementType.Player1Won, ThisGame!.AssetDatabase.Get<TextureAsset>("player_1_wins")!);
        SetTextureFor(AnnouncementType.Player2Won, ThisGame!.AssetDatabase.Get<TextureAsset>("player_2_wins")!);
    }

    /// <summary>
    /// Sets a texture for a given announcement type.
    /// </summary>
    /// <param name="type">The announcement type.</param>
    /// <param name="asset">The texture.</param>
    private void SetTextureFor(AnnouncementType type, TextureAsset asset)
    {
        _map ??= new();
        _map[type] = asset;
    }

    /// <summary>
    /// Show an announcement.
    /// </summary>
    /// <param name="type">The type.</param>
    public void DisplayAnnouncement(AnnouncementType type)
    {
        _currentAnnouncement = type;

        _opacityTween?.Kill();
        _opacityTween = new Tween<int>()
            .WithBeginningValue(255)
            .WithEndingValue(0)
            .WithEasing(TimeEasing.Linear)
            .WithTime(2f)
            .WithIncrementer((a, b, t) => (int)Mathematics.LinearInterpolation(a, b, t))
            .WithUpdateCallback(opacity => _opacity = opacity)
            .WithFinishedCallback(fin =>
            {
                _opacityTween = null;
                _currentAnnouncement = AnnouncementType.None;
            })
            .BindTo(GetGameObjectFromGame<TweenEngine>()!);
    }

    /// <inheritdoc/>
    public void DrawUI()
    {
        if (_currentAnnouncement == AnnouncementType.None)
            return;

        var tex = _map![_currentAnnouncement].Texture!.Value;
        Raylib.DrawTexture(
            tex,
            (Raylib.GetScreenWidth() / 2) - (tex.width / 2),
            (Raylib.GetScreenHeight() / 2) - (tex.height / 2),
            new Color(255, 255, 255, _opacity)
        );
    }
}
