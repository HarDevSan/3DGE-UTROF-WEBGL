//  Structs
struct Attributes
{
    float3 positionOS                   : POSITION;
    float3 normalOS                     : NORMAL;
    float4 tangentOS                    : TANGENT;
    #ifdef LIGHTMAP_ON
        float2 staticLightmapUV         : TEXCOORD1;
    #endif
    #ifdef DYNAMICLIGHTMAP_ON
        float2 dynamicLightmapUV        : TEXCOORD2;
    #endif
    float2 texcoord                     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS                   : SV_POSITION;
    #if defined(_MASKMAP)
        float4 uv                       : TEXCOORD0;
    #else
        float2 uv                       : TEXCOORD0;
    #endif
    half3 normalWS                      : TEXCOORD3;
    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 1);
    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        float3 positionWS               : TEXCOORD2;
    #endif
    #if defined(_NORMALMAP) || !defined(_COTTONWOOL)
        half4 tangentWS                 : TEXCOORD5;
    #endif
    half4 fogFactorAndVertexLight       : TEXCOORD6;
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord              : TEXCOORD7;
    #endif
    #ifdef DYNAMICLIGHTMAP_ON
        float2  dynamicLightmapUV       : TEXCOORD8;
    #endif

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

//--------------------------------------
//  Vertex shader

Varyings LitPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    VertexPositionInputs vertexInput; // 
    vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

//  As we use half3 here we have to normalize!
    // half3 viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    output.uv.xy = TRANSFORM_TEX(input.texcoord, _BaseMap);
    #if defined(_MASKMAP)
        output.uv.zw = TRANSFORM_TEX(input.texcoord, _MaskMap);
    #endif

    // already normalized from normal transform to WS.
    output.normalWS = normalInput.normalWS;
    // output.viewDirWS = viewDirWS;
//  GGX needs the tangent even if no normal is assigned!
    #if defined(_NORMALMAP) || !defined(_COTTONWOOL)
        real sign = input.tangentOS.w * GetOddNegativeScale();
        output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
    #endif

    OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
    #ifdef DYNAMICLIGHTMAP_ON
        output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
    OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
    
    output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        output.positionWS = vertexInput.positionWS;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(vertexInput);
    #endif
    output.positionCS = vertexInput.positionCS;

    return output;
}

//--------------------------------------
//  Fragment shader and functions

inline void InitializeSurfaceData(
    #if defined(_MASKMAP)
        float4 uv,
    #else
        float2 uv,
    #endif
    out SurfaceData outSurfaceData, out AdditionalSurfaceData additionalSurfaceData)
{
    outSurfaceData = (SurfaceData)0;

    half4 albedoAlpha = SampleAlbedoAlpha(uv.xy, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    #if defined(_ALPHATEST_ON)
        outSurfaceData.alpha = Alpha(albedoAlpha.a, 1, _Cutoff);
    #else
        outSurfaceData.alpha = 1;
    #endif

    #if defined(_MASKMAP)
        half4 maskSample = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv.zw);
        additionalSurfaceData.translucency = maskSample.b;
        outSurfaceData.occlusion = lerp(1.0h, maskSample.g, _OcclusionStrength);
        outSurfaceData.smoothness = maskSample.a * _Smoothness;
    #else
        additionalSurfaceData.translucency = 1;
        outSurfaceData.occlusion = 1;
        outSurfaceData.smoothness = _Smoothness;
    #endif

    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;
    outSurfaceData.metallic = 0;
    outSurfaceData.specular = _SpecColor.rgb;
    
//  Normal Map
    #if defined (_NORMALMAP)
        outSurfaceData.normalTS = SampleNormal(uv.xy, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    #else
        outSurfaceData.normalTS = half3(0,0,1);
    #endif

    outSurfaceData.emission = 0;
}

void InitializeInputData(Varyings input, half3 normalTS, half facing, out InputData inputData)
{
    inputData = (InputData)0;
    
    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        inputData.positionWS = input.positionWS;
    #endif
    
    //half3 viewDirWS = SafeNormalize(input.viewDirWS);
    half3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
    #if defined(_NORMALMAP) || !defined(_COTTONWOOL)
        normalTS.z *= facing;
        float sgn = input.tangentWS.w;      // should be either +1 or -1
        float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
        inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent, input.normalWS.xyz));
    #else
        inputData.normalWS = input.normalWS * facing;
    #endif

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = viewDirWS;

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif
    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    #if defined(DYNAMICLIGHTMAP_ON)
        inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
    #else
        inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
    #endif
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
    inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);

    #if defined(DEBUG_DISPLAY)
    #if defined(DYNAMICLIGHTMAP_ON)
    inputData.dynamicLightmapUV = input.dynamicLightmapUV;
    #endif
    #if defined(LIGHTMAP_ON)
    inputData.staticLightmapUV = input.staticLightmapUV;
    #else
    inputData.vertexSH = input.vertexSH;
    #endif
    #endif

}

half4 LitPassFragment(Varyings input, half facing : VFACE) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

//  Get the surface description
    SurfaceData surfaceData;
    AdditionalSurfaceData additionalSurfaceData;
    InitializeSurfaceData(input.uv, surfaceData, additionalSurfaceData);

//  Prepare surface data (like bring normal into world space and get missing inputs like gi)
    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, facing, inputData);

    #if defined(_RIMLIGHTING)
        half rim = saturate(1.0h - saturate( dot(inputData.normalWS, inputData.viewDirectionWS) ) );
        half power = _RimPower;
        if(_RimFrequency > 0 ) {
            half perPosition = lerp(0.0h, 1.0h, dot(1.0h, frac(UNITY_MATRIX_M._m03_m13_m23) * 2.0h - 1.0h ) * _RimPerPositionFrequency ) * 3.1416h;
            power = lerp(power, _RimMinPower, (1.0h + sin(_Time.y * _RimFrequency + perPosition) ) * 0.5h );
        }
        surfaceData.emission += pow(rim, power) * _RimColor.rgb * _RimColor.a;
    #endif

    #if !defined(_COTTONWOOL)
        float sgn = input.tangentWS.w;      // should be either +1 or -1
        float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
    #endif

    #ifdef _DBUFFER
        #if defined(_RECEIVEDECALS)
            ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
        #endif
    #endif

//  Apply lighting
    half4 color = LuxURPClothFragmentPBR(
        inputData,
        surfaceData,
        #if !defined(_COTTONWOOL)
            input.tangentWS.xyz,
        #else
            half3(0,0,0),
        #endif
        _Anisotropy,
        _SheenColor.rgb,
        #if defined(_SCATTERING)
            half4(additionalSurfaceData.translucency * _TranslucencyStrength, _TranslucencyPower, _ShadowStrength, _Distortion)
        #else
            half4(0,0,0,0)
        #endif
    );    
//  Add fog
    color.rgb = MixFog(color.rgb, inputData.fogCoord);

    return color;
}