#version 130
in vec3 pTexCoord;
in vec4 pPosition;

uniform sampler3D inTex3D;
uniform vec3 BG_COLOR;

void main()
{
    float tx = pTexCoord.x;
    float ty = pTexCoord.y;
    float tz = pTexCoord.z;

    vec4 col = vec4(BG_COLOR.x, BG_COLOR.y, BG_COLOR.z, 1.0f);
    
    if (tx >= 0 && tx <= 1 && ty >= 0 && ty <= 1 && tz >= 0 && tz <= 1)
    {
        col = texture(inTex3D, vec3(tx, ty, tz));
    }

    gl_FragColor = col;
}
