#version 330 core

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec3 inNormal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat3 normalMatrix;

out vec3 FragPos;
out vec3 Normal;

void main(void)
{
    gl_Position = vec4(inPosition, 1.0f) * model * view * projection;
    FragPos = vec3(model * vec4(inPosition, 1.0f));
    Normal = inNormal * normalMatrix; // Eliminate normal distortion
}