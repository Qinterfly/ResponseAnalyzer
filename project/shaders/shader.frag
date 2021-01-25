#version 330 core

in vec3 Normal;
in vec3 FragPos;

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float constant;
    float linear;
    float quadratic;
};

uniform vec3 objectColor;
uniform vec3 viewPos;
uniform Light light;
uniform Material material;
uniform int isLighting;

out vec4 FragColor;


void main()
{
    if (isLighting > 0)
    {
        // Ambient color
        vec3 ambient = light.ambient * material.ambient;
        // Light direction
        vec3 norm = normalize(Normal);
        vec3 lightDir = normalize(light.position - FragPos); 
        // The diffuse part of Phong lighting model.
        float diff = max(dot(norm, lightDir), 0.0); // Always nonegative
        vec3 diffuse = light.diffuse * (diff * material.diffuse);
        // The specular light 
        vec3 viewDir = normalize(viewPos - FragPos);
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = light.specular * (spec * material.specular);
        // Attenuation 
        float dist = length(light.position - FragPos);
        float intensity = light.constant + light.linear * dist + light.quadratic * (dist * dist);
        // Adding all the components
        vec3 result = (ambient + diffuse + specular) * intensity * objectColor;
        FragColor = vec4(result, 1.0f);
    }
    else 
    {
        FragColor = vec4(objectColor, 1.0f);
    }
}