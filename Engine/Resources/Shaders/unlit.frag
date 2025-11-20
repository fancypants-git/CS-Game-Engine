#version 330 core
out vec4 FragColor;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;

uniform sampler2D texture0;
uniform bool useTexture;

uniform vec3 color;
uniform vec3 diffuseColor;
uniform vec3 specularColor;
uniform float specularExponent;

uniform vec3 cameraPos;


void main()
{
    // default light data
    vec3 lightDirection = normalize(vec3(1, -1, -1));
    vec3 lightColor = vec3(1, 1, 1);
    
    // Ambient Lighting
    const float ambientConstant = 0.4f;
    vec3 ambient = color * lightColor * ambientConstant;
    
    // Diffuse Lighting
    const float diffuseConstant = 0.7;
    float diffuseDot = max(dot(normal, lightDirection), 0);
    vec3 diffuse = diffuseColor * lightColor * diffuseDot * diffuseConstant;
    
    // Specular Lighting
    const float specularConstant = 0.5f;
    vec3 reflectDir = reflect(-lightDirection, normal);
    float specularDot = max(dot(normalize(fragPos - cameraPos), reflectDir), 0);
    vec3 specular = specularColor * lightColor * pow(specularDot, specularExponent) * specularConstant;
    
    if (useTexture)
        FragColor = texture(texture0, texCoord) * vec4(ambient + diffuse + specular, 1.0f);
    else
        FragColor = vec4(ambient + diffuse + specular, 1.0f);
}
