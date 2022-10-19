#ifndef INPUT_BASETOPDOWN_INCLUDED
#define INPUT_BASETOPDOWN_INCLUDED

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//  defines a bunch of helper functions (like lerpwhiteto)
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"  
//  defines SurfaceData, textures and the functions Alpha, SampleAlbedoAlpha, SampleNormal, SampleEmission
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

//  Must be declared before we can include Lighting.hlsl
    struct AdditionalSurfaceData
    {
        half fuzzMask;
    };

//  defines e.g. "DECLARE_LIGHTMAP_OR_SH"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 
    #include "../Includes/Lux URP Simple Fuzz Lighting.hlsl"

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"


    CBUFFER_START(UnityPerMaterial)
        float4  _BaseMap_ST;
        half4   _BaseColor;
        half4   _SpecColor;
        half    _BumpScale;
        half    _GlossMapScale;
        half    _GlossMapScaleDyn;
        half4   _EmissionColor;
        half    _Occlusion;
        half    _BumpScaleDyn;
        half    _NormalFactor;
        half    _NormalLimit;
        half    _TopDownTiling;
        float3  _TerrainPosition;
        half    _LowerNormalMinStrength;
        half    _LowerNormalInfluence;
        half    _HeightBlendSharpness;
    //  Simple Fuzz
        half    _FuzzStrength;
        half    _FuzzAmbient;
        half    _FuzzWrap;
        half    _FuzzPower;        
        half    _FuzzBias;

        half    _Cutoff;    //HDRP 10.1. DepthNormal pass
    CBUFFER_END
    
    #if defined(_COMBINEDTEXTURE)
        TEXTURE2D(_MaskMap); SAMPLER(sampler_MaskMap);
    #endif
    TEXTURE2D(_TopDownBaseMap); SAMPLER(sampler_TopDownBaseMap);
    TEXTURE2D(_TopDownNormalMap); SAMPLER(sampler_TopDownNormalMap);
#endif