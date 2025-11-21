#version 330 core
out vec4 FragColor;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;

uniform vec3 cameraPos;

struct Material
{
    vec3 color;
    vec3 diffuseColor;
    vec3 specularColor;

    bool useAmbientMap;
    bool useSpecularPowerMap;

    sampler2D ambientMap;
    sampler2D specularPowerMap;

    float specularPower;
    float transparency;
};

struct Light
{
    vec3 color;
    vec3 direction;
};

uniform Material material;

void main()
{
    Light light = Light(vec3(1), normalize(vec3(-1, -1, 1)));
    
    vec3 texColor = vec3(1);
    if (material.useAmbientMap)
        texColor = texture(material.ambientMap, texCoord).rgb;

    // Ambient Light //
    float ambientStrength = 0.6;
    vec3 ambient = ambientStrength * light.color * material.color * texColor;


    // Diffuse Light //
    float diffuseStrength = 0.7;
    float diffuseDot = max(dot(-light.direction, normal), 0);
    vec3 diffuse = diffuseStrength * diffuseDot * light.color * material.diffuseColor * texColor;


    // Specular Light //
    float specularStrength = 0.5;

    float specularPower;
    if (material.useSpecularPowerMap)
        specularPower = texture(material.specularPowerMap, texCoord).r;
    else
        specularPower = material.specularPower;

    vec3 specReflect = reflect(-light.direction, normal);
    vec3 viewDir = normalize(fragPos - cameraPos);
    float specularDot = pow(max(dot(specReflect, viewDir), 0), max(specularPower, 0.0001));
    vec3 specular = specularStrength * specularDot * light.color * material.specularColor * texColor;

    // Total
    FragColor = vec4(ambient + diffuse + specular, material.transparency);
}
