#ifndef DRAW_COLOR
#define DRAW_COLOR 1
#endif

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct PrepassInput
{
    float4 vertex   : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct PrepassOutput
{
    float4 vertex        : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

PrepassOutput PrepassVert(PrepassInput IN)
{
    PrepassOutput OUT;
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
    OUT.vertex = TransformObjectToHClip(IN.vertex.xyz);
    return OUT;
}

float4 PrepassFrag(PrepassOutput IN) : SV_TARGET
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
    return DRAW_COLOR;
}

