using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Data;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Tweening;
using Raylib_cs;

namespace Battleships.Game.Objects;

/// <summary>
/// The log of what's happening in the game.
/// </summary>
internal class GameLog : GameObject,
    IUIObject
{
    /// <summary>
    /// The max amount of messages.
    /// </summary>
    private const int MAX_MESSAGES = 3;

    /// <summary>
    /// The map from characters to positions within the font.
    /// </summary>
    private static readonly IReadOnlyDictionary<char, Vector2Int> CHARACTER_MAP = new Dictionary<char, Vector2Int>()
    {
        { '!',  new Vector2Int(0, 0) },
        { '"',  new Vector2Int(1, 0) },
        { '#',  new Vector2Int(2, 0) },
        { '$',  new Vector2Int(3, 0) },
        { '%',  new Vector2Int(4, 0) },
        { '&',  new Vector2Int(5, 0) },
        { '\'', new Vector2Int(6, 0) },
        { '(',  new Vector2Int(7, 0) },
        { ')',  new Vector2Int(0, 1) },
        { '*',  new Vector2Int(1, 1) },
        { '+',  new Vector2Int(2, 1) },
        { '`',  new Vector2Int(3, 1) },
        { '-',  new Vector2Int(4, 1) },
        //{ '????',  new Vector2Int(5, 1) },
        { '/',  new Vector2Int(6, 1) },
        { '0',  new Vector2Int(7, 1) },
        { '1',  new Vector2Int(0, 2) },
        { '2',  new Vector2Int(1, 2) },
        { '3',  new Vector2Int(2, 2) },
        { '4',  new Vector2Int(3, 2) },
        { '5',  new Vector2Int(4, 2) },
        { '6',  new Vector2Int(5, 2) },
        { '7',  new Vector2Int(6, 2) },
        { '8',  new Vector2Int(7, 2) },
        { '9',  new Vector2Int(0, 3) },
        { ':',  new Vector2Int(1, 3) },
        { ';',  new Vector2Int(2, 3) },
        { '<',  new Vector2Int(3, 3) },
        { '=',  new Vector2Int(4, 3) },
        { '>',  new Vector2Int(5, 3) },
        { '?',  new Vector2Int(6, 3) },
        { '@',  new Vector2Int(7, 3) },
        { 'A',  new Vector2Int(0, 4) },
        { 'B',  new Vector2Int(1, 4) },
        { 'C',  new Vector2Int(2, 4) },
        { 'D',  new Vector2Int(3, 4) },
        { 'E',  new Vector2Int(4, 4) },
        { 'F',  new Vector2Int(5, 4) },
        { 'G',  new Vector2Int(6, 4) },
        { 'H',  new Vector2Int(7, 4) },
        { 'I',  new Vector2Int(0, 5) },
        { 'J',  new Vector2Int(1, 5) },
        { 'K',  new Vector2Int(2, 5) },
        { 'L',  new Vector2Int(3, 5) },
        { 'M',  new Vector2Int(4, 5) },
        { 'N',  new Vector2Int(5, 5) },
        { 'O',  new Vector2Int(6, 5) },
        { 'P',  new Vector2Int(7, 5) },
        { 'Q',  new Vector2Int(0, 6) },
        { 'R',  new Vector2Int(1, 6) },
        { 'S',  new Vector2Int(2, 6) },
        { 'T',  new Vector2Int(3, 6) },
        { 'U',  new Vector2Int(4, 6) },
        { 'V',  new Vector2Int(5, 6) },
        { 'W',  new Vector2Int(6, 6) },
        { 'X',  new Vector2Int(7, 6) },
        { 'Y',  new Vector2Int(0, 7) },
        { 'Z',  new Vector2Int(1, 7) },
        { '[',  new Vector2Int(2, 7) },
        { '\\', new Vector2Int(3, 7) },
        { ']',  new Vector2Int(4, 7) },
        { '^',  new Vector2Int(5, 7) },
        { '_',  new Vector2Int(6, 7) },
        { '|',  new Vector2Int(7, 7) },
    };

    /// <summary>
    /// The scale of the renderer.
    /// </summary>
    public float Scale { get; set; } = 2f;

    /// <summary>
    /// The width of a single character.
    /// </summary>
    private int _characterWidth;

    /// <summary>
    /// The height of a single character.
    /// </summary>
    private int _characterHeight;

    /// <summary>
    /// The font we're using.
    /// </summary>
    private TextureAsset? _font;

    /// <summary>
    /// The message list.
    /// </summary>
    private List<string>? _messages;

    /// <summary>
    /// The current offset from the top.
    /// </summary>
    private float _offset = 0f;

    /// <summary>
    /// The tween of the message offset.
    /// </summary>
    private Tween<float>? _offsetTween;

    public override void Start()
    {
        _font = ThisGame!.AssetDatabase
            .Get<TextureAsset>("smallfont");

        _characterWidth = _font!.Texture!.Value.width / 8;
        _characterHeight = _font!.Texture!.Value.height / 8;
    }

    /// <summary>
    /// Adds a message to the log.
    /// </summary>
    /// <param name="message">The message.</param>
    public void AddMessageToLog(string message)
    {
        _messages ??= new();
        _messages.Add(message.ToUpper());

        if (_messages.Count > MAX_MESSAGES)
            StartMessageRemovalTweener();
    }

    /// <summary>
    /// Starts the message removal tweener.
    /// </summary>
    private void StartMessageRemovalTweener()
    {
        _offsetTween?.Kill();
        _offsetTween = new Tween<float>()
            .WithBeginningValue(0f)
            .WithEndingValue(-_characterHeight * Scale)
            .WithTime(0.5f)
            .WithEasing(TimeEasing.Linear)
            .WithIncrementer(Mathematics.LinearInterpolation)
            .WithUpdateCallback(off => _offset = off)
            .WithFinishedCallback(off =>
            {
                _messages?.RemoveAt(0);
                _offset = 0f;
            })
            .BindTo(GetGameObjectFromGame<TweenEngine>()!);
    }

    /// <summary>
    /// Draws text at the given position.
    /// </summary>
    /// <param name="uiPos">The position.</param>
    /// <param name="text">The text.</param>
    private void DrawTextRun(Vector2 uiPos, string text)
    {
        foreach (var chr in text)
        {
            if (CHARACTER_MAP.TryGetValue(chr, out var gridPos))
            {
                var p = _font!.GetAtlasRectForGridIndex(gridPos);
                if (p.HasValue)
                {
                    Raylib.DrawTexturePro(
                        _font!.Texture!.Value,
                        p.Value,
                        new Rectangle(uiPos.X, uiPos.Y, _characterWidth * Scale, _characterHeight * Scale),
                        Vector2.Zero,
                        0f,
                        Color.WHITE
                    );
                }
            }

            uiPos.X += _characterWidth * Scale;
        }
    }

    /// <inheritdoc/>
    public void DrawUI()
    {
        if (_messages is null)
            return;

        var offset = _offset;
        foreach (var message in _messages!)
        {
            DrawTextRun(new Vector2(0, offset), message);
            offset += _characterHeight * Scale;
        }
    }
}
