#ifndef UNIVERSAL_CLEARCOATLIGHTING_INCLUDED
#define UNIVERSAL_CLEARCOATLIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"



// ---------

struct AdditionalData {
    half coatThickness;
    half3 coatSpecular;
    half3 normalWS;
    half perceptualRoughness;
    half roughness;
    half roughness2;
    half normalizationTerm;
    half roughness2MinusOne;    // roughness² - 1.0
    half reflectivity;
    half grazingTerm;
    half specOcclusion;
};

half3 DirectBDRF_LuxClearCoat(BRDFData brdfData, AdditionalData addData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS, half NdotL)
{
#ifndef _SPECULARHIGHLIGHTS_OFF
    float3 lightDirectionWSFloat3 = float3(lightDirectionWS);
    float3 halfDir = SafeNormalize(lightDirectionWSFloat3 + float3(viewDirectionWS));

    half LoH = half(saturate(dot(lightDirectionWSFloat3, halfDir)));

//  Base Lobe
    float NoH = saturate(dot(float3(normalWS), halfDir));
    float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;
    half d2 = half(d * d);

    half LoH2 = LoH * LoH;
    LoH2 = max(0.1h, LoH2);
    half specularTerm = brdfData.roughness2 / (d2 * max(half(0.1), LoH2) * brdfData.normalizationTerm);

    #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
        specularTerm = specularTerm - HALF_MIN;
        specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
    #endif

    half3 spec = specularTerm * brdfData.specular * NdotL;

//  Coat Lobe
//  From HDRP: Scale base specular
    #if defined (_MASKMAP) && defined(_STANDARDLIGHTING)
        [branch]
        if (addData.coatThickness > 0.0h) {
    #endif
            half coatF = F_Schlick(addData.reflectivity /*addData.coatSpecular*/ /*CLEAR_COAT_F0*/, LoH) * addData.coatThickness;
            spec *= Sq(1.0h - coatF);
            //spec *= (1.0h - coatF); // as used by filament, na, not really
            NoH = saturate(dot(float3(addData.normalWS), halfDir));
            d = NoH * NoH * addData.roughness2MinusOne + 1.00001f;
            d2 = half(d * d);
            //LoH2 = LoH * LoH; no need to recalculate LoH2!
            specularTerm = addData.roughness2 / (d2 * max(half(0.1), LoH2) * addData.normalizationTerm);
            #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
                specularTerm = specularTerm - HALF_MIN;
                specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
            #endif
            spec += specularTerm * addData.coatSpecular * saturate(dot(addData.normalWS, lightDirectionWS));
    #if defined (_MASKMAP) && defined(_STANDARDLIGHTING)
        }
    #endif
    half3 color = spec + brdfData.diffuse * NdotL; // from HDRP (but does not do much?) * lerp(1.0h, 1.0h - coatF, addData.coatThickness);
    return color;
#else
    return brdfData.diffuse * NdotL;
#endif
}

half3 LightingPhysicallyBased_LuxClearCoat(BRDFData brdfData, AdditionalData addData, half3 lightColor, half3 lightDirectionWS, half lightAttenuation, half3 normalWS, half3 viewDirectionWS)
{
    half NdotL = saturate(dot(normalWS, lightDirectionWS));
    half3 radiance = lightColor * (lightAttenuation); // * NdotL);
    return DirectBDRF_LuxClearCoat(brdfData, addData, normalWS, lightDirectionWS, viewDirectionWS, NdotL) * radiance;
}

half3 LightingPhysicallyBased_LuxClearCoat(BRDFData brdfData, AdditionalData addData, Light light, half3 normalWS, half3 viewDirectionWS)
{
    return LightingPhysicallyBased_LuxClearCoat(brdfData, addData, light.color, light.direction, light.distanceAttenuation * light.shadowAttenuation, normalWS, viewDirectionWS);
}

half3 EnvironmentBRDF_LuxClearCoat(BRDFData brdfData, AdditionalData addData, half3 indirectDiffuse, half3 indirectSpecular, half fresnelTerm)
{
    half3 c = indirectDiffuse * brdfData.diffuse;
    float surfaceReduction = 1.0 / (addData.roughness2 + 1.0);
    c += surfaceReduction * indirectSpecular * lerp(addData.coatSpecular, addData.grazingTerm, fresnelTerm);
    return c;
}


half3 GlobalIllumination_LuxClearCoat(BRDFData brdfData, AdditionalData addData, half3 bakedGI, half occlusion, float3 positionWS, half3 normalWS, half3 baseNormalWS, half3 viewDirectionWS, half NdotV)
{
    half3 reflectVector = reflect(-viewDirectionWS, normalWS);
    half fresnelTerm = Pow4(1.0 - NdotV);

    half3 indirectDiffuse = bakedGI * occlusion; 
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, positionWS, addData.perceptualRoughness, addData.specOcclusion);

    half3 res = EnvironmentBRDF_LuxClearCoat(brdfData, addData, indirectDiffuse, indirectSpecular, fresnelTerm);

    #if defined(_SECONDARYLOBE)
        #if defined (_MASKMAP) && defined(_STANDARDLIGHTING)
            [branch]
            if (addData.coatThickness > 0.0h) {
        #endif
                reflectVector = reflect(-viewDirectionWS, baseNormalWS);
                indirectSpecular = GlossyEnvironmentReflection(reflectVector, positionWS, brdfData.perceptualRoughness, occlusion);
                float surfaceReduction = 1.0 / (brdfData.roughness2 + 1.0);
                res += NdotV * surfaceReduction * indirectSpecular * lerp(brdfData.specular, brdfData.grazingTerm, fresnelTerm);
        #if defined (_MASKMAP) && defined(_STANDARDLIGHTING)
            }
        #endif
    #endif

//  Debug
    if (IsOnlyAOLightingFeatureEnabled())
    {
        res = occlusion.xxx; // "Base white" for AO debug lighting mode // Lux: We return occlusion here
    }

    return res;
}

half3 f0ClearCoatToSurface_Lux(half3 f0) 
{
    // Approximation of iorTof0(f0ToIor(f0), 1.5)
    // This assumes that the clear coat layer has an IOR of 1.5
#if defined(SHADER_API_MOBILE)
    return saturate(f0 * (f0 * 0.526868h + 0.529324h) - 0.0482256h);
#else
    return saturate(f0 * (f0 * (0.941892h - 0.263008h * f0) + 0.346479h) - 0.0285998h);
#endif
}


half4 LuxClearCoatFragmentPBR(InputData inputData, 
    SurfaceData surfaceData,
    half3 clearcoatSpecular,
    half3 vertexNormalWS,
    half NdotV
)
{
 

//  URP 12: due to decals we pass in the lerped color    
    // half NdotV = saturate( dot(vertexNormalWS, inputData.viewDirectionWS) );
    // #if defined(_SECONDARYCOLOR)
    //     surfaceData.albedo = lerp(secondaryColor, baseColor, NdotV);
    // #else
    //     surfaceData.albedo = baseColor;
    // #endif

    BRDFData brdfData;
    InitializeBRDFData(surfaceData, brdfData);

    #if defined(DEBUG_DISPLAY)
        half4 debugColor;
        if (CanDebugOverrideOutputColor(inputData, surfaceData, brdfData, debugColor))
        {
            return debugColor;
        }
    #endif

    //#if defined(_ADJUSTSPEC)
//        brdfData.specular = lerp(brdfData.specular, ConvertF0ForAirInterfaceToF0ForClearCoat15(brdfData.specular), clearcoatThickness);
          brdfData.specular = lerp(brdfData.specular, f0ClearCoatToSurface_Lux(brdfData.specular), surfaceData.clearCoatMask);
    //#endif

//  URP does also modify the roughness
//  Modify Roughness of base layer
/*  half ieta = lerp(1.0h, CLEAR_COAT_IETA, outBRDFData.clearCoat);
    half coatRoughnessScale = Sq(ieta);
    half sigma = RoughnessToVariance(PerceptualRoughnessToRoughness(outBRDFData.perceptualRoughness));
    outBRDFData.perceptualRoughness = RoughnessToPerceptualRoughness(VarianceToRoughness(sigma * coatRoughnessScale));
*/

    half4 shadowMask = CalculateShadowMask(inputData);
//  We can't use the bulit in new macro here as we calculate 2 different ao terms. See below.
    //AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
    AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(inputData.normalizedScreenSpaceUV);
    half minAmbientOcclusion = min(aoFactor.indirectAmbientOcclusion, surfaceData.occlusion);
    uint meshRenderingLayers = GetMeshRenderingLightLayer();

    AdditionalData addData; //  = (AdditionalData)0;
    #if defined (_MASKMAP) && defined(_STANDARDLIGHTING)
        [branch]
        if (surfaceData.clearCoatMask == 0.0h) {
            addData.coatThickness = 0.0h;
            addData.coatSpecular = brdfData.specular;
            addData.normalWS = inputData.normalWS;
            addData.perceptualRoughness = brdfData.perceptualRoughness;
            addData.roughness = brdfData.roughness;
            addData.roughness2 = brdfData.roughness2;
            addData.normalizationTerm = brdfData.normalizationTerm;
            addData.roughness2MinusOne = brdfData.roughness2MinusOne; 
            addData.reflectivity = ReflectivitySpecular(brdfData.specular);
            addData.grazingTerm = brdfData.grazingTerm;
        //  In order to get ssao we have to use aoFactor.indirectAmbientOcclusion
            addData.specOcclusion = minAmbientOcclusion;
        }
        else {
    #endif
        addData.coatThickness = surfaceData.clearCoatMask; //clearcoatThickness;
        addData.coatSpecular = clearcoatSpecular;
        addData.normalWS = vertexNormalWS;
        addData.perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(surfaceData.clearCoatSmoothness);
        addData.roughness = PerceptualRoughnessToRoughness(addData.perceptualRoughness);
        addData.roughness2 = addData.roughness * addData.roughness;
        addData.normalizationTerm = addData.roughness * 4.0h + 2.0h;
        addData.roughness2MinusOne = addData.roughness2 - 1.0h;
        addData.reflectivity = ReflectivitySpecular(clearcoatSpecular);
        addData.grazingTerm = saturate(surfaceData.clearCoatSmoothness + addData.reflectivity);
    //  This contain ssao only
        addData.specOcclusion = aoFactor.indirectAmbientOcclusion;
    #if defined (_MASKMAP) && defined(_STANDARDLIGHTING)
        }
    #endif

//  Now fix aoFactor.indirectAmbientOcclusion
    aoFactor.indirectAmbientOcclusion = minAmbientOcclusion;
    
//  URP 12:
    Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);

    LightingData lightingData = CreateLightingData(inputData, surfaceData);

//  Approximation of refraction on BRDF
    half refractionScale = ((NdotV * 0.5h + 0.5h) * NdotV - 1.0h) * saturate(1.25h - 1.25h * (1.0h - surfaceData.clearCoatSmoothness)) + 1.0h;
    brdfData.diffuse = lerp(brdfData.diffuse, brdfData.diffuse * refractionScale, surfaceData.clearCoatMask);
//  brdfData.specular = brdfData.specular * lerp(1.0, refractionScale, clearcoatThickness);

    lightingData.giColor = GlobalIllumination_LuxClearCoat(brdfData, addData, inputData.bakedGI, aoFactor.indirectAmbientOcclusion, inputData.positionWS, addData.normalWS, inputData.normalWS, inputData.viewDirectionWS, NdotV);

//  Adjust base specular as we have a transition from coat to material and not air to material
    #if defined(_ADJUSTSPEC)
//        brdfData.specular = lerp(brdfData.specular, ConvertF0ForAirInterfaceToF0ForClearCoat15(brdfData.specular), addData.coatThickness);
    #endif

#if defined(_LIGHT_LAYERS)
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
#endif
    lightingData.mainLightColor = LightingPhysicallyBased_LuxClearCoat(brdfData, addData, mainLight, inputData.normalWS, inputData.viewDirectionWS);
#if defined(_LIGHT_LAYERS)
    }
#endif

    #if defined(_ADDITIONAL_LIGHTS)
        uint pixelLightCount = GetAdditionalLightsCount();

        #if USE_CLUSTERED_LIGHTING
            for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
            {
                Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);

                if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
                {
                    lightingData.additionalLightsColor += LightingPhysicallyBased_LuxClearCoat(brdfData, addData, light, inputData.normalWS, inputData.viewDirectionWS);
                }
            }
        #endif
        
        LIGHT_LOOP_BEGIN(pixelLightCount)
            Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);
        #if defined(_LIGHT_LAYERS)
            if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
            {
        #endif
            #if defined(_SCREEN_SPACE_OCCLUSION)
                light.color *= aoFactor.directAmbientOcclusion;
            #endif
            lightingData.additionalLightsColor += LightingPhysicallyBased_LuxClearCoat(brdfData, addData, light, inputData.normalWS, inputData.viewDirectionWS);
        #if defined(_LIGHT_LAYERS)
            }
        #endif
        LIGHT_LOOP_END

    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        lightingData.vertexLightingColor += inputData.vertexLighting * brdfData.diffuse;
    #endif

    return CalculateFinalColor(lightingData, surfaceData.alpha);
}

#endif