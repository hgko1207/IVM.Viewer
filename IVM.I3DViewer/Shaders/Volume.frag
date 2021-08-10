#version 130
in vec3 pNormal;
in vec3 pTexCoord;
in vec4 pPosition;
//out vec4 FragColor;

uniform sampler3D inTex3D;
uniform vec3 BG_COLOR;
uniform vec3 THRESHOLD_INTENSITY;
uniform vec3 ALPHA_WEIGHT;
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
    vec4 acc = vec4(0, 0, 0, 0);

    if (RENDER_MODE == 0)
        acc = bcol;

    if (BOX_HEIGHT > 1)
        iter *= BOX_HEIGHT;

    float wcr = 0;
    float wcg = 0;
    float wcb = 0;
    float war = 1.0 / iter * ALPHA_WEIGHT.x;
    float wag = 1.0 / iter * ALPHA_WEIGHT.y;
    float wab = 1.0 / iter * ALPHA_WEIGHT.z;
    bool mark = false;

    for (int i = 0; i < iter; ++i)
    {
        float tx = pTexCoord.x + dir.x * (iter-i-1);
        float ty = pTexCoord.y + dir.y * (iter-i-1);
        float tz = pTexCoord.z + dir.z * (iter-i-1) / BOX_HEIGHT;

        if (tx < 0 || tx > 1 || ty < 0 || ty > 1 || tz < 0 || tz > 1)
            continue;
        
        vec4 col = texture(inTex3D, vec3(tx, ty, tz));

        if (RENDER_MODE == 0)
        {
            // if one band is exist at least, show voxel
            if (IS_COLOCALIZATION == 0)
            {
                if (col.x < THRESHOLD_INTENSITY.x && col.y < THRESHOLD_INTENSITY.y && col.z < THRESHOLD_INTENSITY.z)
                    continue;
            }
            else
            {
                if (col.x < THRESHOLD_INTENSITY.x || col.y < THRESHOLD_INTENSITY.y || col.z < THRESHOLD_INTENSITY.z)
                    continue;
            }

            acc = (acc * 0.5f + col * 0.5f);
            mark = true;
        }
        else if (RENDER_MODE == 1)
        {
            vec4 col2 = col;

            // if below intensity, that band will be hide
            if (col.x <= THRESHOLD_INTENSITY.x)
                col2.x = 0;

            if (col.y <= THRESHOLD_INTENSITY.y)
                col2.y = 0;

            if (col.z <= THRESHOLD_INTENSITY.z)
                col2.z = 0;

            // if one band is exist at least, show voxel
            if (IS_COLOCALIZATION == 0)
            {
                if (col.x < THRESHOLD_INTENSITY.x && col.y < THRESHOLD_INTENSITY.y && col.z < THRESHOLD_INTENSITY.z)
                    continue;
            }
            else
            {
                if (col.x < THRESHOLD_INTENSITY.x || col.y < THRESHOLD_INTENSITY.y || col.z < THRESHOLD_INTENSITY.z)
                    continue;
            }

            wcr += war;
            wcg += wag;
            wcb += wab;
            acc.x = acc.x + col2.x * war;
            acc.y = acc.y + col2.y * wag;
            acc.z = acc.z + col2.z * wab;

            mark = true;
        }
    }

    vec4 ocol = vec4(0, 0, 0, 0);

    if (RENDER_MODE == 0)
    {
        ocol = acc;
    }
    else if (RENDER_MODE == 1)
    {
        if (wcr > 1) wcr = 1;
        if (wcg > 1) wcg = 1;
        if (wcb > 1) wcb = 1;

        ocol.x = bcol.x * (1 - wcr) + acc.x * wcr;
        ocol.y = bcol.y * (1 - wcg) + acc.y * wcg;
        ocol.z = bcol.z * (1 - wcb) + acc.z * wcb;
    }

    if (ocol.x > 1) ocol.x = 1;
    if (ocol.y > 1) ocol.y = 1;
    if (ocol.z > 1) ocol.z = 1;
    ocol.w = 0;

    if (!mark) discard;

    gl_FragColor = ocol;
}
