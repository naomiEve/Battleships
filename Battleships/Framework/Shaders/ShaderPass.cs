using Raylib_cs;

namespace Battleships.Framework.Shaders
{
    /// <summary>
    /// A single shader pass.
    /// </summary>
    internal abstract class ShaderPass
    {
        /// <summary>
        /// The default vertex shader.
        /// </summary>
        private const string DEFAULT_VERTEX = @"
#version 330

// Input vertex attributes
in vec3 vertexPosition;
in vec2 vertexTexCoord;
in vec3 vertexNormal;
in vec4 vertexColor;

// Input uniform values
uniform mat4 mvp;

// Output vertex attributes (to fragment shader)
out vec2 fragTexCoord;
out vec4 fragColor;

// NOTE: Add here your custom variables

void main()
{
    // Send vertex attributes to fragment shader
    fragTexCoord = vertexTexCoord;
    fragColor = vertexColor;

    // Calculate final vertex position
    gl_Position = mvp*vec4(vertexPosition, 1.0);
}
";

        /// <summary>
        /// The default fragment shader.
        /// </summary>
        private const string DEFAULT_FRAGMENT = @"
#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Input uniform values
uniform sampler2D texture0;
uniform vec4 colDiffuse;

// Output fragment color
out vec4 finalColor;

void main()
{
    // Texel color fetching from texture sampler
    vec4 texelColor = texture(texture0, fragTexCoord);

    finalColor = texelColor*colDiffuse;
}
";
        /// <summary>
        /// The shader.
        /// </summary>
        private Shader _shader;

        /// <summary>
        /// Constructs a new shader pass from the given shaders.
        /// </summary>
        /// <param name="vertexShader">The vertex shader.</param>
        /// <param name="fragmentShader">The fragment shader.</param>
        public ShaderPass(string? vertexShader, string? fragmentShader)
        {
            _shader = Raylib.LoadShaderFromMemory(vertexShader ?? DEFAULT_VERTEX, fragmentShader ?? DEFAULT_FRAGMENT);
        }

        /// <summary>
        /// Begins the drawing mode for this shader.
        /// </summary>
        public void Begin()
        {
            Raylib.BeginShaderMode(_shader);
        }

        /// <summary>
        /// Ends the drawing mode for this shader.
        /// </summary>
        public void End()
        {
            Raylib.EndShaderMode();
        }
    }
}
