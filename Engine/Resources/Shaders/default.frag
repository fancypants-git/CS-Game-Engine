#version 330 core
out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;

uniform vec3 color;
uniform sampler2D texture0;
uniform bool useTexture;

void main()
{
    if (useTexture)
        FragColor = texture(texture0, texCoord) * vec4(color, 1.0f);
    else
        FragColor = vec4(color, 1.0f);
}
