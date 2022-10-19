#ifndef INPUT_LUXLWRP_BASE_INCLUDED
#define INPUT_LUXLWRP_BASE_INCLUDED


    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
//  defines a bunch of helper functions (like lerpwhiteto)
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"  
//  defines SurfaceData, textures and the functions Alpha, SampleAlbedoAlpha, SampleNormal, SampleEmission
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

//  Must be declared before we can include Lighting.hlsl
    struct AdditionalSurfaceData
    {
        half fuzzMask;
        half translucency;
    };

//  defines e.g. "DECLARE_LIGHTMAP_OR_SH"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
 
    #include "../Includes/Lux URP Simple Fuzz Lighting.hlsl"

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

//  Material Inputs
    CBUFFER_START(UnityPerMaterial)

        half4   _BaseColor;

        half    _Cutoff;

        float4  _BaseMap_ST;
        half    _Smoothness;
        half3   _SpecColor;

    //  Simple
        half    _FuzzStrength;
        half    _FuzzAmbient;
        half    _FuzzWrap;
        half    _FuzzPower;        
        half    _FuzzBias;
        
        half    _BumpScale;

        float4  _MaskMap_ST;
        half    _OcclusionStrength;

        half    _TranslucencyPower;
        half    _TranslucencyStrength;
        half    _ShadowStrength;
        half    _Distortion;
        half    _DecalTransmission;

        half    _ShadowOffset;

        half4   _RimColor;
        half    _RimPower;
        half    _RimMinPower;
        half    _RimFrequency;
        half    _RimPerPositionFrequency;
    CBUFFER_END

//  Additional textures
    #if defined(_MASKMAP)
        TEXTURE2D(_MaskMap); SAMPLER(sampler_MaskMap);
    #endif

//  Global Inputs

#endif