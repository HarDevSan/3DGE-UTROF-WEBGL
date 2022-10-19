CBUFFER_START(UnityPerMaterial)
    half4 _Color;
    half _Border;
    half _Cutoff;
CBUFFER_END

#if defined(_ALPHATEST_ON)
    TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap); float4 _BaseMap_ST;
#endif

struct Attributes
{
    float4 vertex           : POSITION;
    float3 normal           : NORMAL;
    #if defined(_ALPHATEST_ON)
        float2 texcoord     : TEXCOORD0;
    #endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 position : POSITION;
    #if defined(LITPASS)
        half fogCoord : TEXCOORD0;
    #endif
    #if defined(_ALPHATEST_ON)
        float2 uv     : TEXCOORD1;
    #endif
    UNITY_VERTEX_OUTPUT_STEREO
};


//--------------------------------------
// Shared vertex shader

Varyings vert (Attributes input)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    #if !defined(_ALPHATEST_ON)
    //  Extrude
        #if !defined(_OUTLINEINSCREENSPACE)
            #if defined(_COMPENSATESCALE)
                float3 scale;
                scale.x = length(float3(UNITY_MATRIX_M[0].x, UNITY_MATRIX_M[1].x, UNITY_MATRIX_M[2].x));
                scale.y = length(float3(UNITY_MATRIX_M[0].y, UNITY_MATRIX_M[1].y, UNITY_MATRIX_M[2].y));
                scale.z = length(float3(UNITY_MATRIX_M[0].z, UNITY_MATRIX_M[1].z, UNITY_MATRIX_M[2].z));
            #endif
                input.vertex.xyz += input.normal * 0.001 * _Border
            #if defined(_COMPENSATESCALE) 
                / scale
            #endif
            ;
        #endif
    #endif

    o.position = TransformObjectToHClip(input.vertex.xyz);

    #if defined(LITPASS)
        o.fogCoord = ComputeFogFactor(o.position.z);
    #endif

    #if !defined(_ALPHATEST_ON)
    //  Extrude
        #if defined(_OUTLINEINSCREENSPACE)
            if (_Border > 0.0h) {
                //float3 normal = mul(UNITY_MATRIX_MVP, float4(v.normal, 0)).xyz; // to clip space
                float3 normal = mul(GetWorldToHClipMatrix(), mul(GetObjectToWorldMatrix(), float4(v.normal, 0.0))).xyz;
                float2 offset = normalize(normal.xy);
                float2 ndc = _ScreenParams.xy * 0.5;
                o.position.xy += ((offset * _Border) / ndc * o.position.w);
            }
        #endif
    #endif

//  Alpha testing
    #if defined(_ALPHATEST_ON)
       o.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    #endif

    return o;
}

//--------------------------------------
//  Shared fragment shader

//  Helper
inline float2 shufflefast (float2 offset, float2 shift) {
    return offset * shift;
}

half4 frag (Varyings input) : SV_Target
{
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if defined(_ALPHATEST_ON)
        
        half innerAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a;

    //  Outline
        float2 uv = input.uv;

        float2 offset = float2(1,1);
        float2 shift = fwidth(uv) * _Border * 0.5f;

        float2 sampleCoord = uv + shufflefast(offset, shift); 
        half shuffleAlpha = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, sampleCoord).a;

        offset = float2(-1,1);
        sampleCoord = uv + shufflefast(offset, shift);
        shuffleAlpha += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, sampleCoord).a;

        offset = float2(1,-1);
        sampleCoord = uv + shufflefast(offset, shift);
        shuffleAlpha += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, sampleCoord).a;

        offset = float2(-1,-1);
        sampleCoord = uv + shufflefast(offset, shift);
        shuffleAlpha += SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, sampleCoord).a;
    
    //  Mask inner parts - which is not really needed when using the stencil buffer. Let's do it anyway, just in case.
        shuffleAlpha = lerp(shuffleAlpha, 0, step(_Cutoff, innerAlpha) );
    //  Apply clip
        //outSurfaceData.alpha = Alpha(shuffleAlpha, 1, _Cutoff);
        clip(shuffleAlpha - _Cutoff);
    #endif

    #if defined(LITPASS)
        _Color.rgb = MixFog(_Color.rgb, input.fogCoord);
        return half4(_Color);
    #else
        return 0;
    #endif
}