#version 330 core

layout(location = 0) in vec3 inPosition;
uniform mat4 transform;

void main(void)
{
    gl_Position = vec4(inPosition, 1.0f) * transform;
}