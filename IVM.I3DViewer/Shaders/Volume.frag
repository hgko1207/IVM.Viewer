#version 130
in vec3 pNormal;
in vec3 pTexCoord;
in vec4 pPosition;
//out vec4 FragColor;

uniform sampler3D inTex3D;
uniform vec3 BG_COLOR;
uniform vec4 THRESHOLD_INTENSITY_MIN;
uniform vec4 THRESHOLD_INTENSITY_MAX;
uniform vec4 ALPHA_WEIGHT;
uniform vec4 BAND_ORDER;
uniform vec4 BAND_VISIBLE;
uniform float PER_PIXEl_ITERATION;
uniform float BOX_HEIGHT;
uniform float RENDER_MODE;
uniform float IS_COLOCALIZATION;

uniform mat4 invModelView;
uniform mat4 invProj;

void main()
{
    float w = pPosition.w;
    vec4 scrFgPos = vec4(pPosition.x/w, pPosition.y/w, pPosition.z/w, 1);
    vec4 scrBgPos = (scrFgPos + vec4(0,0,-1,0)) * w;

    vec4 orgFrPos = invModelView * invProj * pPosition;
    vec4 orgBgPos = invModelView * invProj * scrBgPos;

    float ofw = orgFrPos.w;
    float obw = orgBgPos.w;
    vec3 locFr = vec3(orgFrPos.x/ofw, orgFrPos.y/ofw, orgFrPos.z/ofw);
    vec3 locBg = vec3(orgBgPos.x/obw, orgBgPos.y/obw, orgBgPos.z/obw);
    vec3 dir = locFr - locBg;

    dir = normalize(dir);
    //dir.x = -dir.x;
    //dir.y = -dir.y;
    dir.z = -dir.z;

    float iter = PER_PIXEl_ITERATION;
    dir = dir / (iter - 1);
    vec4 bcol = vec4(BG_COLOR.x, BG_COLOR.y, BG_COLOR.z, 0.0f);
    
    vec4 acc = vec4(-1, -1, -1, -1);
    if (RENDER_MODE == 1)
        acc = vec4(0, 0, 0, 0);

    if (BOX_HEIGHT > 1)
        iter *= BOX_HEIGHT;

    vec4 wa = vec4(1.0 / iter * ALPHA_WEIGHT);

    bool mark = false;

    for (int i = 0; i < iter; ++i)
    {
        float tx = pTexCoord.x + dir.x * (iter-i-1);
        float ty = pTexCoord.y + dir.y * (iter-i-1);
        float tz = pTexCoord.z + dir.z * (iter-i-1) / BOX_HEIGHT;

        if (tx < 0 || tx > 1 || ty < 0 || ty > 1 || tz < 0 || tz > 1)
            continue;
        
        vec4 col = texture(inTex3D, vec3(tx, ty, tz));        
        col *= 255.0;

        // if one band is exist at least, show voxel
        if (IS_COLOCALIZATION == 0)
        {
            if(!((THRESHOLD_INTENSITY_MIN.x <= col.x && col.x <= THRESHOLD_INTENSITY_MAX.x) || 
                (THRESHOLD_INTENSITY_MIN.y <= col.y && col.y <= THRESHOLD_INTENSITY_MAX.y) ||
                (THRESHOLD_INTENSITY_MIN.z <= col.z && col.z <= THRESHOLD_INTENSITY_MAX.z) ||
                (THRESHOLD_INTENSITY_MIN.w <= col.w && col.w <= THRESHOLD_INTENSITY_MAX.w)))
                continue;
        }
        else
        {
            if(!((THRESHOLD_INTENSITY_MIN.x <= col.x && col.x <= THRESHOLD_INTENSITY_MAX.x) &&
                (THRESHOLD_INTENSITY_MIN.y <= col.y && col.y <= THRESHOLD_INTENSITY_MAX.y) &&
                (THRESHOLD_INTENSITY_MIN.z <= col.z && col.z <= THRESHOLD_INTENSITY_MAX.z) &&
                (THRESHOLD_INTENSITY_MIN.w <= col.w && col.w <= THRESHOLD_INTENSITY_MAX.w)))
                continue;
        }

        if (RENDER_MODE == 0)
        {
            acc = col / 255.0;
        }
        else if (RENDER_MODE == 1)
        {
            // if below intensity, that band will be hide
            if (THRESHOLD_INTENSITY_MIN.x <= col.x && col.x <= THRESHOLD_INTENSITY_MAX.x)
                acc.x += col.x / 255.0 * wa.x;

            if (THRESHOLD_INTENSITY_MIN.y <= col.y && col.y <= THRESHOLD_INTENSITY_MAX.y)
                acc.y += col.y / 255.0 * wa.y;

            if (THRESHOLD_INTENSITY_MIN.z <= col.z && col.z <= THRESHOLD_INTENSITY_MAX.z)
                acc.z += col.z / 255.0 * wa.z;

            if (THRESHOLD_INTENSITY_MIN.w <= col.w && col.w <= THRESHOLD_INTENSITY_MAX.w)
                acc.w += col.w / 255.0 * wa.z;
        }

        mark = true;
    }

    vec4 ocol = acc;

    if (!mark) discard;

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
