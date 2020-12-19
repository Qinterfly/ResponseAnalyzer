#version 450 core

uniform vec4 definedColor;
out vec4 outputColor;

void main()
{
    outputColor = definedColor;
}