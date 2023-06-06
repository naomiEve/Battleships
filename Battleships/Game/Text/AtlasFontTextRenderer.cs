using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Battleships.Framework.Assets;
using Battleships.Framework.Data;
using Raylib_cs;

namespace Battleships.Game.Text;

/// <summary>
/// Class used for rendering fonts inside of an atlas.
/// </summary>
internal class AtlasFontTextRenderer
{
    /// <summary>
    /// The info for a single character.
    /// </summary>
    private readonly struct CharacterInfo
    {
        /// <summary>
        /// Its position on the grid.
        /// </summary>
        public Vector2Int GridPosition { get; init; }

        /// <summary>
        /// The overriden width.
        /// </summary>
        public int? OverrideWidth { get; init; }

        /// <summary>
        /// Constructs a new character info.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="width">The width.</param>
        public CharacterInfo(Vector2Int pos, int? width)
        {
            GridPosition = pos;
            OverrideWidth = width;
        }
    }

    /// <summary>
    /// The map from characters to positions within the font.
    /// </summary>
    private static readonly IReadOnlyDictionary<char, CharacterInfo> CHARACTER_MAP = new Dictionary<char, CharacterInfo>()
    {
        { '!',  new(new(0, 0), null) },
        { '"',  new(new(1, 0), null) },
        { '#',  new(new(2, 0), null) },
        { '$',  new(new(3, 0), null) },
        { '%',  new(new(4, 0), null) },
        { '&',  new(new(5, 0), null) },
        { '\'', new(new(6, 0),    6) },
        { '(',  new(new(7, 0), null) },
        { ')',  new(new(0, 1), null) },
        { '*',  new(new(1, 1), null) },
        { '+',  new(new(2, 1), null) },
        { '`',  new(new(3, 1), null) },
        { '-',  new(new(4, 1), null) },
        { '/',  new(new(6, 1), null) },
        { '0',  new(new(7, 1), null) },
        { '1',  new(new(0, 2), null) },
        { '2',  new(new(1, 2), null) },
        { '3',  new(new(2, 2), null) },
        { '4',  new(new(3, 2), null) },
        { '5',  new(new(4, 2), null) },
        { '6',  new(new(5, 2), null) },
        { '7',  new(new(6, 2), null) },
        { '8',  new(new(7, 2), null) },
        { '9',  new(new(0, 3), null) },
        { ':',  new(new(1, 3), null) },
        { ';',  new(new(2, 3), null) },
        { '<',  new(new(3, 3), null) },
        { '=',  new(new(4, 3), null) },
        { '>',  new(new(5, 3), null) },
        { '?',  new(new(6, 3), null) },
        { '@',  new(new(7, 3), null) },
        { 'A',  new(new(0, 4), null) },
        { 'B',  new(new(1, 4), null) },
        { 'C',  new(new(2, 4), null) },
        { 'D',  new(new(3, 4), null) },
        { 'E',  new(new(4, 4), null) },
        { 'F',  new(new(5, 4), null) },
        { 'G',  new(new(6, 4), null) },
        { 'H',  new(new(7, 4), null) },
        { 'I',  new(new(0, 5),    6) },
        { 'J',  new(new(1, 5), null) },
        { 'K',  new(new(2, 5), null) },
        { 'L',  new(new(3, 5), null) },
        { 'M',  new(new(4, 5), null) },
        { 'N',  new(new(5, 5), null) },
        { 'O',  new(new(6, 5), null) },
        { 'P',  new(new(7, 5), null) },
        { 'Q',  new(new(0, 6), null) },
        { 'R',  new(new(1, 6), null) },
        { 'S',  new(new(2, 6), null) },
        { 'T',  new(new(3, 6), null) },
        { 'U',  new(new(4, 6), null) },
        { 'V',  new(new(5, 6), null) },
        { 'W',  new(new(6, 6), null) },
        { 'X',  new(new(7, 6), null) },
        { 'Y',  new(new(0, 7), null) },
        { 'Z',  new(new(1, 7), null) },
        { '[',  new(new(2, 7), null) },
        { '\\', new(new(3, 7), null) },
        { ']',  new(new(4, 7), null) },
        { '^',  new(new(5, 7), null) },
        { '_',  new(new(6, 7), null) },
        { '|',  new(new(7, 7), null) },
    };

    /// <summary>
    /// The scale.
    /// </summary>
    public float Scale { get; set; } = 1f;

    /// <summary>
    /// The used font.
    /// </summary>
    private TextureAsset _font;

    /// <summary>
    /// The width of a single character.
    /// </summary>
    public int CharacterWidth { get; private set; }

    /// <summary>
    /// The height of a single character.
    /// </summary>
    public int CharacterHeight { get; private set; }

    /// <summary>
    /// Creates a new text renderer.
    /// </summary>
    /// <param name="font">The used font.</param>
    public AtlasFontTextRenderer(TextureAsset font)
    {
        _font = font;

        CharacterWidth = _font.Texture!.Value.width / 8;
        CharacterHeight = _font.Texture!.Value.height / 8;
    }

    /// <summary>
    /// Draws text at the given position.
    /// </summary>
    /// <param name="uiPos">The position.</param>
    /// <param name="text">The text.</param>
    public void DrawTextRun(Vector2 uiPos, string text)
    {
        foreach (var chr in text)
        {
            var width = CharacterWidth;

            if (CHARACTER_MAP.TryGetValue(chr, out var info))
            {
                var p = _font!.GetAtlasRectForGridIndex(info.GridPosition);
                if (p.HasValue)
                {
                    if (info.OverrideWidth.HasValue)
                        width = info.OverrideWidth.Value;

                    Raylib.DrawTexturePro(
                        _font!.Texture!.Value,
                        p.Value,
                        new Rectangle(uiPos.X, uiPos.Y, CharacterWidth * Scale, CharacterHeight * Scale),
                        Vector2.Zero,
                        0f,
                        Color.WHITE
                    );
                }
            }

            uiPos.X += width * Scale;
        }
    }

}
