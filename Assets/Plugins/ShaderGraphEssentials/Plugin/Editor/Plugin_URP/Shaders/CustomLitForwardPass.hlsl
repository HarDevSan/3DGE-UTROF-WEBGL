void BuildInputData(Varyings input, float3 normal, out InputData inputData)
{
    inputData.positionWS = input.positionWS;
#ifdef _NORMALMAP

     #if _NORMAL_DROPOFF_TS
        // IMPORTANT! If we ever support Flip on double sided materials ensure bitangent and tangent are NOT flipped.
        float crossSign = (input.tangentWS.w > 0.0 ? 1.0 : -1.0) * GetOddNegativeScale();
        float3 bitangent = crossSign * cross(input.normalWS.xyz, input.tangentWS.xyz);
        inputData.normalWS = TransformTangentToWorld(normal, half3x3(input.tangentWS.xyz, bitangent, input.normalWS.xyz));
    #elif _NORMAL_DROPOFF_OS
        inputData.normalWS = TransformObjectToWorldNormal(normal);
    #elif _NORMAL_DROPOFF_WS
        inputData.normalWS = normal;
    #endif
    
#else
    inputData.normalWS = input.normalWS;
#endif
    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = SafeNormalize(input.viewDirectionWS);
    
    #if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif

    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.sh, inputData.normalWS);
}

    PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;
    output = BuildVaryings(input);
    PackedVaryings packedOutput = (PackedVaryings)0;
    packedOutput = PackVaryings(output);
    return packedOutput;
}

half4 SGE_UniversalFragmentCustom(InputData inputData, SGECustomLightingData data)
{
    Light mainLight = GetMainLight(inputData.shadowCoord);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

    half3 diffuseColor = inputData.bakedGI + SGE_DiffuseLightingCustom(mainLight, data);
    half3 specularColor = SGE_SpecularLightingCustom(mainLight, inputData.viewDirectionWS, data);

#ifdef _ADDITIONAL_LIGHTS
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, inputData.positionWS);
        diffuseColor += SGE_DiffuseLightingCustom(light, data);
        specularColor += SGE_SpecularLightingCustom(light, inputData.viewDirectionWS, data);
    }
#endif

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    diffuseColor += inputData.vertexLighting;
#endif

    half3 finalColor = diffuseColor * data.albedo + data.emission;

#if defined(_SPECGLOSSMAP) || defined(_SPECULAR_COLOR)
    finalColor += specularColor;
#endif

    return half4(finalColor, data.alpha);
}

half4 frag(PackedVaryings packedInput) : SV_TARGET 
{    
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

    #if _AlphaClip
        clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
    #endif

    InputData inputData;
    BuildInputData(unpacked, surfaceDescription.Normal, inputData);
    
    // SGECustomLightingData remap
	SGECustomLightingData data;
	data.albedo = surfaceDescription.Albedo;
	data.normal = inputData.normalWS;
	data.specular = surfaceDescription.Specular;
	data.glossiness = surfaceDescription.Glossiness;
	data.smoothness = surfaceDescription.Shininess * 128.0;
	data.emission = surfaceDescription.Emission;
	data.alpha = surfaceDescription.Alpha;
	data.customLightingData1 = surfaceDescription.CustomLightingData1;
	data.customLightingData2 = surfaceDescription.CustomLightingData2;

	// custom lit style
	half4 color = SGE_UniversalFragmentCustom(inputData, data);

    color.rgb = MixFog(color.rgb, inputData.fogCoord); 
    return color;
}
