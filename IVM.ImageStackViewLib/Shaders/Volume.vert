#version 130
in vec4 vPosition;
in vec3 vNormal;
in vec3 vTexCoord;

uniform mat4 matModelView;
uniform mat4 matProj;

out vec3 pNormal;
out vec3 pTexCoord;
out vec4 pPosition;

void main()
{
    gl_Position = matProj * matModelView * vPosition;
    pPosition = matProj * matModelView * vPosition;

    pTexCoord = vTexCoord;
    pNormal = vNormal;
}