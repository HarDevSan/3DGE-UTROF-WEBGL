#ifndef LIGHTWEIGHT_TRANSPARENTLIGHTING_INCLUDED
#define LIGHTWEIGHT_TRANSPARENTLIGHTING_INCLUDED

half4 LuxURPTransparentFragmentPBR(InputData inputData, half3 albedo, half metallic, half3 specular,
    half smoothness, half occlusion, half3 emission, half alpha
)
{
    BRDFData brdfData;
    InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);

    half4 shadowMask = CalculateShadowMask(inputData);
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
//  Dummy AO
    AmbientOcclusionFactor aoFactor;
    aoFactor.indirectAmbientOcclusion = 1;
    aoFactor.directAmbientOcclusion = 1;

    Light mainLight = GetMainLight(inputData.shadowCoord);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

    half3 color = GlobalIllumination(brdfData, inputData.bakedGI, occlusion, inputData.normalWS, inputData.viewDirectionWS);

    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
    //  GetMainLight will return screen space shadows.
        #if defined(_MAIN_LIGHT_SHADOWS)
            ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
            half shadowStrength = GetMainLightShadowStrength();
            mainLight.shadowAttenuation = SampleShadowmap(inputData.shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowSamplingData, shadowStrength, false);
        #endif
        color += LightingPhysicallyBased(brdfData, mainLight, inputData.normalWS, inputData.viewDirectionWS);
    }

    #ifdef _ADDITIONAL_LIGHTS
        uint pixelLightCount = GetAdditionalLightsCount();
        for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
        {
            Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);
            if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
            {
                color += LightingPhysicallyBased(brdfData, light, inputData.normalWS, inputData.viewDirectionWS);
            }
        }
    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        color += inputData.vertexLighting * brdfData.diffuse;
    #endif

    color += emission;

    return half4(color, alpha);
}
#endif