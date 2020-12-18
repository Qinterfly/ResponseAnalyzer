#version 450

uniform vec4 definedColor;
out vec4 outputColor;

void main()
{
    outputColor = definedColor;
}