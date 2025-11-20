#version 330 core
in vec2 texCoord;

uniform vec3 color;

uniform sampler2D texture0;
uniform bool useTexture;

out vec4 FragColor;

void main() {
    if (useTexture)
        FragColor = texture(texture0, texCoord) * color;
    else
        FragColor = color;
}