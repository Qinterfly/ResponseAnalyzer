#version 450 core

out vec4 outputColor;
uniform vec4 definedColor;


void main()
{
    outputColor = definedColor;
}