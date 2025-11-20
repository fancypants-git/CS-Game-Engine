#version 330 core
out vec4 FragColor;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;

uniform vec3 cameraPos;

struct Material
{
    vec3 color;
    sampler2D texture0;
    sampler2D specularMap;
    float specularExponent;
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
    Light light = Light(vec3(1), vec3(-1, -1, 1));
    
    vec3 diffuseMapTexel = texture(material.texture0, texCoord).rgb;
    vec3 color = light.color * material.color;
    
    // ambient light
    float ambientStrength = 0.5;
    vec3 ambient = ambientStrength * color * diffuseMapTexel;
    
    // diffuse light
    float diffuseStrength = 0.8;
    float diffuseDot = max(dot(light.direction, -normal), 0);
    vec3 diffuse = diffuseStrength * diffuseDot * color * diffuseMapTexel;
    
    // specular light
    float specularStrength = 0.6;
    vec3 viewDir = normalize(cameraPos - fragPos);
    vec3 reflectDir = reflect(light.direction, -normal);
    float specularReflection = pow(max(dot(viewDir, reflectDir), 0), material.specularExponent);
    vec3 specular = specularStrength * specularReflection * color * texture(material.specularMap, texCoord).rgb;
    
    FragColor = vec4(ambient + diffuse + specular, material.transparency);
}
