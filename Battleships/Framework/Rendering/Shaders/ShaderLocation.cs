using Battleships.Framework.Assets;
using Raylib_cs;

namespace Battleships.Framework.Rendering.Shaders;

/// <summary>
/// A shader location.
/// </summary>
/// <typeparam name="T">The type it holds.</typeparam>
internal class ShaderLocation<T>
    where T: unmanaged
{
    /// <summary>
    /// The id of the location.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The shader itself.
    /// </summary>
    public ShaderAsset Shader { get; init; }

    /// <summary>
    /// The uniform data type.
    /// </summary>
    public ShaderUniformDataType DataType { get; init; }

    /// <summary>
    /// Construct a new shader location.
    /// </summary>
    /// <param name="loc">The location.</param>
    /// <param name="shader">The shader asset.</param>
    public ShaderLocation(int loc, ShaderUniformDataType type, ShaderAsset shader)
    {
        Id = loc;
        DataType = type;
        Shader = shader;
    }

    /// <summary>
    /// Set a value of this shader location.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetValue(T value)
    {
        Raylib.SetShaderValue(
            Shader.Shader,
            Id,
            value,
            DataType
        );
    }
}
