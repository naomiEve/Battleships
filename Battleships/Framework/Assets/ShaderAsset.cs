using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Battleships.Framework.Rendering.Shaders;
using Raylib_cs;

namespace Battleships.Framework.Assets;

/// <summary>
/// A shader to be applied onto a material.
/// </summary>
internal class ShaderAsset : Asset,
    IAssetCanBeRef<Shader>
{
    /// <summary>
    /// The paths to shaders.
    /// </summary>
    private struct ShaderPaths
    {
        /// <summary>
        /// The fragment shader path.
        /// </summary>
        public string? Fragment { get; set; }

        /// <summary>
        /// The vertex shader path.
        /// </summary>
        public string? Vertex { get; set; }
    }

    /// <summary>
    /// The shader.
    /// </summary>
    public Shader Shader => _shader;

    /// <summary>
    /// The internal shader.
    /// </summary>
    public Shader _shader;

    /// <inheritdoc/>
    public ref Shader AsRef() => ref _shader;

    /// <inheritdoc/>
    public override void LoadFromFile(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs);

        var paths = new ShaderPaths();

        // Process the sha file line-by-line
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine()!;
            var split = line.Split('=');
            if (split is null)
                continue;

            var name = split[0];
            switch (name)
            {
                case "fs":
                    paths.Fragment= split[1];
                    break;

                case "vs":
                    paths.Vertex = split[1];
                    break;
            }
        }

        _shader = Raylib.LoadShader(paths.Vertex, paths.Fragment);
    }

    /// <summary>
    /// Get a shader location by its name.
    /// </summary>
    /// <param name="name">The name</param>
    /// <param name="type">The uniform data type.</param>
    /// <returns>The location id, or nothing if it doesn't exist.</returns>
    public ShaderLocation<T>? GetShaderLocation<T>(string name, ShaderUniformDataType type)
        where T: unmanaged
    {
        var loc = Raylib.GetShaderLocation(_shader, name);
        if (loc < 0)
            return null;

        return new ShaderLocation<T>(loc, type, this);
    }
}
