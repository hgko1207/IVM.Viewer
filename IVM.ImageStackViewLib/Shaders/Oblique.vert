#version 130
in vec4 vPosition;
in vec3 vTexCoord;

uniform mat4 matModelView;
uniform mat4 matProj;

out vec3 pTexCoord;

void main()
{
    gl_Position = matProj * matModelView * vPosition;
    pTexCoord = vTexCoord;
}