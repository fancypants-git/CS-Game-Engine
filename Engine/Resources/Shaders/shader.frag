#version 330 core
out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;

uniform vec3 color;
uniform sampler2D texture0;

void main()
{
    FragColor = texture(texture0, texCoord) * vec4(color, 1.0f);
//    FragColor = vec4(texCoord, 0.0f, 1.0f);
}
