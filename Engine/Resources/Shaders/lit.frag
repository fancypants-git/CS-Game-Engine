#version 330 core
in vec2 texCoord;

out vec4 FragColor;

struct Material
{
    vec3 color;
    
    bool useAmbientMap;
    sampler2D ambientMap;
    
    float transparency;
};

uniform Material material;

void main() {
    if (material.useAmbientMap)
        FragColor = texture(material.ambientMap, texCoord) * vec4(material.color, material.transparency);
    else
        FragColor = vec4(material.color, material.transparency);
}