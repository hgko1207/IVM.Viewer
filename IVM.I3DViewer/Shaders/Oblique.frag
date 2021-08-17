#version 130
in vec3 pTexCoord;
in vec4 pPosition;

uniform sampler3D inTex3D;
uniform vec3 BG_COLOR;
uniform vec4 BAND_ORDER;
uniform vec4 BAND_VISIBLE;

void main()
{
    float tx = pTexCoord.x;
    float ty = pTexCoord.y;
    float tz = pTexCoord.z;

    vec4 ocol = vec4(BG_COLOR.x, BG_COLOR.y, BG_COLOR.z, 1.0f);
    
    if (tx >= 0 && tx <= 1 && ty >= 0 && ty <= 1 && tz >= 0 && tz <= 1)
    {
        ocol = texture(inTex3D, vec3(tx, ty, tz));
    }

    if (ocol.x > 1) ocol.x = 1;
    if (ocol.y > 1) ocol.y = 1;
    if (ocol.z > 1) ocol.z = 1;
    if (ocol.w > 1) ocol.w = 1;

    if (BAND_VISIBLE.x == 1)
    {
        if (BAND_ORDER.x == 0)
            gl_FragColor.x = ocol.x;
        else if (BAND_ORDER.x == 1)
            gl_FragColor.y = ocol.x;
        else if (BAND_ORDER.x == 2)
            gl_FragColor.z = ocol.x;
    }

    if (BAND_VISIBLE.y == 1)
    {
        if (BAND_ORDER.y == 0)
            gl_FragColor.x = ocol.y;
        else if (BAND_ORDER.y == 1)
            gl_FragColor.y = ocol.y;
        else if (BAND_ORDER.y == 2)
            gl_FragColor.z = ocol.y;
    }

    if (BAND_VISIBLE.z == 1)
    {
        if (BAND_ORDER.z == 0)
            gl_FragColor.x = ocol.z;
        else if (BAND_ORDER.z == 1)
            gl_FragColor.y = ocol.z;
        else if (BAND_ORDER.z == 2)
            gl_FragColor.z = ocol.z;
    }

    if (BAND_VISIBLE.w == 1)
    {
        if (BAND_ORDER.w == 0)
            gl_FragColor.x = ocol.w;
        else if (BAND_ORDER.w == 1)
            gl_FragColor.y = ocol.w;
        else if (BAND_ORDER.w == 2)
            gl_FragColor.z = ocol.w;
    }

    gl_FragColor.w = 1.0;
}
