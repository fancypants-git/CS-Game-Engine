#version 330 core
in vec2 texCoord;

uniform vec3 color;
uniform float transparency;

uniform sampler2D texture0;
uniform bool useTexture;

out vec4 FragColor;

void main() {
    if (useTexture)
        FragColor = texture(texture0, texCoord) * vec4(color, 1.0f);
    else
        FragColor = vec4(color, transparency);
}