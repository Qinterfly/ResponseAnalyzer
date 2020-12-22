#version 330 core

layout(location = 0) in vec3 inPosition;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
    gl_Position = vec4(inPosition, 1.0f) * model * view * projection;
}