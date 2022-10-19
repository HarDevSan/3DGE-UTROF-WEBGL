struct Attributes
{
    float3 positionOS               : POSITION;
    float3 normalOS                 : NORMAL;
    #if defined(_ALPHATEST_ON) && defined(_MASKMAP)
        float2 texcoord             : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS               : SV_POSITION;
    #if defined(_ALPHATEST_ON) && defined(_MASKMAP)
        float2 uv                   : TEXCOORD0;
    #endif
};

//  Shadow caster specific input
float3 _LightDirection;
float3 _LightPosition;

Varyings ShadowPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);

    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldDir(input.normalOS);

    #if _CASTING_PUNCTUAL_LIGHT_SHADOW
        float3 lightDirectionWS = normalize(_LightPosition - positionWS);
    #else
        float3 lightDirectionWS = _LightDirection;
    #endif

    output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS * _ShadowOffset, lightDirectionWS));
    #if UNITY_REVERSED_Z
        output.positionCS.z = min(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #else
        output.positionCS.z = max(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
    #endif

    #if defined(_ALPHATEST_ON) && defined(_MASKMAP)
        output.uv = TRANSFORM_TEX(input.texcoord, _MaskMap);
    #endif

    return output;
}

half4 ShadowPassFragment(Varyings input) : SV_TARGET
{
    #if defined(_ALPHATEST_ON) && defined(_MASKMAP)
        half mask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, input.uv).a;
        clip (mask - _Cutoff);
    #endif

    return 0;
}