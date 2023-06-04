using System.Numerics;
using Battleships.Framework.Assets;
using Battleships.Framework.Objects;
using Battleships.Framework.Rendering.Shaders;
using Raylib_cs;

namespace Battleships.Game.Objects;

/// <summary>
/// A renderer for the water.
/// </summary>
internal class WaterRenderer : GameObject,
    IDrawableGameObject
{
    /// <summary>
    /// The water model.
    /// </summary>
    private ModelAsset? _model;

    /// <summary>
    /// The time location.
    /// </summary>
    private ShaderLocation<float>? _time;

    /// <summary>
    /// The vertex time location.
    /// </summary>
    private ShaderLocation<float>? _vertexTime;

    /// <inheritdoc/>
    public override void Start()
    {
        _model = ThisGame!
            .AssetDatabase.Get<ModelAsset>("water_quad")!;

        _time = ThisGame!
            .AssetDatabase.Get<ShaderAsset>("water_shader")!
            .GetShaderLocation<float>("time", ShaderUniformDataType.SHADER_UNIFORM_FLOAT);

        _vertexTime = ThisGame!
            .AssetDatabase.Get<ShaderAsset>("water_shader")!
            .GetShaderLocation<float>("vtxTime", ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
    }

    /// <inheritdoc/>
    public void Draw()
    {
        _time?.SetValue((float)Raylib.GetTime() / 1000f);
        _vertexTime?.SetValue((float)Raylib.GetTime());

        Raylib.DrawModel(_model!.Model, Vector3.UnitY * -0.1f, 1f, Color.WHITE);
    }
}
