using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Data;
using Battleships.Framework.Math;
using Battleships.Framework.Objects;
using Battleships.Framework.Tweening;
using Battleships.Game.Text;
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
    /// The text renderer.
    /// </summary>
    private AtlasFontTextRenderer? _renderer;

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
        _renderer ??= new(ThisGame!.AssetDatabase.Get<TextureAsset>("smallfont")!);
        _renderer.Scale = 2f;
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
            .WithEndingValue(-_renderer!.CharacterHeight * _renderer!.Scale)
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

    /// <inheritdoc/>
    public void DrawUI()
    {
        if (_messages is null || _renderer is null)
            return;

        var offset = _offset;
        foreach (var message in _messages!)
        {
            _renderer.DrawTextRun(new Vector2(0, offset), message);
            offset += _renderer.CharacterHeight * _renderer.Scale;
        }
    }
}
