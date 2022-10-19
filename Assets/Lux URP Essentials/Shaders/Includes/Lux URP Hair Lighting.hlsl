// NOTE: Based on URP Lighting.hlsl which replaced some half3 with floats to avoid lighting artifacts on mobile
// Hair lighting functions renamed to solves problems with LWRP 6.x


#ifndef UNIVERSAL_HAIRLIGHTING_INCLUDED
#define UNIVERSAL_HAIRLIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ImageBasedLighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

// From HDRP -----------------------------------------

float RoughnessToBlinnPhongSpecularExponent_Lux(float roughness)
{
    return clamp(2 * rcp(roughness * roughness) - 2, FLT_EPS, rcp(FLT_EPS));
}

//http://web.engr.oregonstate.edu/~mjb/cs519/Projects/Papers/HairRendering.pdf
float3 ShiftTangent_Lux(float3 T, float3 N, float shift)
{
    return normalize(T + N * shift);
}

// Note: this is Blinn-Phong, the original paper uses Phong.
float3 D_KajiyaKay_Lux(float3 T, float3 H, float specularExponent)
{
    float TdotH = dot(T, H);
    float sinTHSq = saturate(1.0 - TdotH * TdotH);

    float dirAttn = saturate(TdotH + 1.0); // Evgenii: this seems like a hack? Do we really need this?

    // Note: Kajiya-Kay is not energy conserving.
    // We attempt at least some energy conservation by approximately normalizing Blinn-Phong NDF.
    // We use the formulation with the NdotL.
    // See http://www.thetenthplanet.de/archives/255.
    float n    = specularExponent;
    float norm = (n + 2) * rcp(2 * PI);

    return dirAttn * norm * PositivePow(sinTHSq, 0.5 * n);
}

/*
#if (_USE_LIGHT_FACING_NORMAL)
    // The Kajiya-Kay model has a "built-in" transmission, and the 'NdotL' is always positive.
    float cosTL = dot(bsdfData.hairStrandDirectionWS, L);
    float sinTL = sqrt(saturate(1 - cosTL * cosTL));
    float NdotL = sinTL; // Corresponds to the cosine w.r.t. the light-facing normal
#else
    // Double-sided Lambert.
    float NdotL = dot(bsdfData.normalWS, L);
#endif
*/



// From HDRP END -----------------------------------------


/*
This is input data:

struct InputData
{
    float3  positionWS;
    half3   normalWS;
    half3   viewDirectionWS;
    float4  shadowCoord;
    half    fogCoord;
    half3   vertexLighting;
    half3   bakedGI;
};
*/

// Ref: Donald Revie - Implementing Fur Using Deferred Shading (GPU Pro 2)
// The grain direction (e.g. hair or brush direction) is assumed to be orthogonal to the normal.
// The returned normal is NOT normalized.
half3 ComputeGrainNormal_Lux(half3 grainDir, half3 V)
{
    half3 B = cross(-V, grainDir);
    return cross(B, grainDir);
}

// Fake anisotropic by distorting the normal.
// The grain direction (e.g. hair or brush direction) is assumed to be orthogonal to N.
// Anisotropic ratio (0->no isotropic; 1->full anisotropy in tangent direction)
half3 GetAnisotropicModifiedNormal_Lux(half3 grainDir, half3 N, half3 V, half anisotropy)
{
    half3 grainNormal = ComputeGrainNormal_Lux(grainDir, V);
    return lerp(N, grainNormal, anisotropy);
}


half3 GlobalIlluminationHair_Lux(
    //BRDFData brdfData,
    half3 albedo,
    half3 specular,
    half roughness,
    half perceptualRoughness,
    half occlusion,
    
    half3 bakedGI,
    float3 positionWS,
    half3 normalWS,
    half3 viewDirectionWS,
    half3 bitangentWS,
    half ambientReflection
)
{

//  We do not handle backfaces properly yet. 
    half NdotV = dot(normalWS, viewDirectionWS);
    half s = sign(NdotV);
//  Lets fix this for reflections?
    //NdotV = s * NdotV;

//  Strengthen occlusion on backfaces    
    //occlusion = lerp(occlusion * 0.5, occlusion, saturate(1 + s));

//  We do not "fix" the reflection vector. This gives us some scattering like reflections
    //half3 reflectNormalWS = GetAnisotropicModifiedNormal_Lux(s * bitangentWS, s * normalWS, viewDirectionWS, 0.6h);
    half3 reflectNormalWS = GetAnisotropicModifiedNormal_Lux(bitangentWS, normalWS, viewDirectionWS, 0.6h);
    half3 reflectVector = reflect(-viewDirectionWS, reflectNormalWS);

    half fresnelTerm = Pow4(1.0 - saturate(NdotV) );
//  ??? perceptualRoughness *= saturate(1.2 - 0.8); //abs(bsdfData.anisotropy));
    half3 indirectDiffuse = bakedGI * occlusion;
    half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, positionWS, perceptualRoughness, occlusion) * ambientReflection;

//  EnvironmentBRDFHair
    half3 c = indirectDiffuse * albedo;
    float surfaceReduction = 1.0 / (roughness * roughness + 1.0);
    half reflectivity = ReflectivitySpecular(specular);
    half grazingTerm = saturate( (1.0h - roughness) + reflectivity);
    c += surfaceReduction * indirectSpecular * lerp(specular, grazingTerm, fresnelTerm);

//  Debug
    if (IsOnlyAOLightingFeatureEnabled())
    {
        c = occlusion.xxx; // "Base white" for AO debug lighting mode // Lux: We return occlusion here
    }

    return c;
}


half3 LightingHair_Lux(
    half3 albedo,
    half3 specular,
    Light light,
    
    half3 normalWS,
    half geomNdotV,
    half3 viewDirectionWS,

    half roughness1,
    half roughness2,
    half3 t1,
    half3 t2,
    half3 specularTint,
    half3 secondarySpecularTint,
    half rimTransmissionIntensity
)
{
    half NdotL = dot(normalWS, light.direction);
    half LdotV = dot(light.direction, viewDirectionWS);
    float invLenLV = rsqrt(max(2.0 * LdotV + 2.0, FLT_EPS));

    float3 lightDirectionWSFloat3 = float3(light.direction);
    float3 halfDir = (lightDirectionWSFloat3 + float3(viewDirectionWS)) * invLenLV;

    half3 hairSpec1 = specularTint * D_KajiyaKay_Lux(t1, halfDir, roughness1);
    #if defined(_SECONDARYLOBE)
        half3 hairSpec2 = secondarySpecularTint * D_KajiyaKay_Lux(t2, halfDir, roughness2);
    #endif

    float NdotH = saturate(dot(normalWS, halfDir));
    half LdotH = half(saturate(dot(lightDirectionWSFloat3, halfDir)));

    half3 F = F_Schlick(specular, LdotH);

//  Reflection
    half3 specR = 0.25h * F * (hairSpec1 
    #if defined(_SECONDARYLOBE)
        + hairSpec2
    #endif
    ) * saturate(NdotL) * saturate(geomNdotV * HALF_MAX);

//  Transmission // Yibing's and Morten's hybrid scatter model hack.
    half scatterFresnel1 = pow(saturate(-LdotV), 9.0h) * pow(saturate(1.0h - geomNdotV * geomNdotV), 12.0h);
//  This looks shitty (using 20)   
    //half scatterFresnel2 = saturate(PositivePow((1.0h - geomNdotV), 20.0h));
    half scatterFresnel2 = saturate(Pow4(1.0h - geomNdotV));
    half transmission = scatterFresnel1 + rimTransmissionIntensity * scatterFresnel2;
    half3 specT = albedo * transmission;

    half3 diffuse = albedo * saturate(NdotL);

//  combine
    half3 result = (diffuse + specR + specT) * light.color * light.distanceAttenuation * light.shadowAttenuation; 
    return result;
}


half4 LuxURPHairFragment(
    InputData inputData,
    SurfaceData surfaceData,
    half3 tangentWS,
    half3 noise,
    half specularShift,
    half3 specularTint,
    half perceptualRoughness,
    half secondarySpecularShift,
    half3 secondarySpecularTint,
    half secondaryPerceptualRoughness,
    half rimTransmissionIntensity,
    half ambientReflection
)
{

    #if defined(DEBUG_DISPLAY)
        BRDFData brdfData;
        InitializeBRDFData(surfaceData, brdfData);
        half4 debugColor;
        if (CanDebugOverrideOutputColor(inputData, surfaceData, brdfData, debugColor)) {
            return debugColor;
        }
    #endif

//  TODO: Simplify this...
    perceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(perceptualRoughness); // * saturate(noise.r * 2) );
    half roughness1 = PerceptualRoughnessToRoughness(perceptualRoughness);
    half pbRoughness1 = RoughnessToBlinnPhongSpecularExponent_Lux(roughness1);

    #if defined(_SECONDARYLOBE)
        secondaryPerceptualRoughness = PerceptualSmoothnessToPerceptualRoughness(secondaryPerceptualRoughness); // * saturate(noise.r * 2) );
        half roughness2 = PerceptualRoughnessToRoughness(secondaryPerceptualRoughness);
        half pbRoughness2 = RoughnessToBlinnPhongSpecularExponent_Lux(roughness2);
    #else
        secondaryPerceptualRoughness = 0;
        half roughness2 = 0;
        half pbRoughness2 = 0;
    #endif

    half geomNdotV = dot(inputData.normalWS, inputData.viewDirectionWS); 

//  Adjust tangentWS in case normal mapping is enabled
    #if defined(_NORMALMAP)   
        tangentWS = Orthonormalize(tangentWS, inputData.normalWS);
    #endif
//  Always calculate bitangent WS
    half3 bitangentWS = cross(inputData.normalWS, tangentWS);
    #if defined(_STRANDDIR_BITANGENT)
        half3 strandDirWS = bitangentWS;
    #else
        half3 strandDirWS = tangentWS;
    #endif

    half3 t1 = ShiftTangent_Lux(strandDirWS, inputData.normalWS, specularShift);
    #if defined(_SECONDARYLOBE)
        half3 t2 = ShiftTangent_Lux(strandDirWS, inputData.normalWS, secondarySpecularShift);
    #else
        half3 t2 = 0;
    #endif

//  Start Lighting    
//  (From HDRP) Note: For Kajiya hair we currently rely on a single cubemap sample instead of two, as in practice smoothness of both lobe aren't too far from each other.
//  and we take smoothness of the secondary lobe as it is often more rough (it is the colored one).
//  NOPE: We use primary!!!!! 

    half4 shadowMask = CalculateShadowMask(inputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);
    uint meshRenderingLayers = GetMeshRenderingLightLayer();
    Light mainLight = GetMainLight(inputData, shadowMask, aoFactor);

    // NOTE: We don't apply AO to the GI here because it's done in the lighting calculation below...
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI);

    LightingData lightingData = CreateLightingData(inputData, surfaceData);
    lightingData.giColor = GlobalIlluminationHair_Lux(surfaceData.albedo, surfaceData.specular, roughness1, perceptualRoughness, aoFactor.indirectAmbientOcclusion, inputData.bakedGI, inputData.positionWS, inputData.normalWS, inputData.viewDirectionWS, bitangentWS, ambientReflection);
    
    #if defined(_LIGHT_LAYERS)
        if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
        {
    #endif
        lightingData.mainLightColor = LightingHair_Lux(surfaceData.albedo, surfaceData.specular, mainLight, inputData.normalWS, geomNdotV, inputData.viewDirectionWS, pbRoughness1, pbRoughness2, t1, t2, specularTint, secondarySpecularTint, rimTransmissionIntensity);
    #if defined(_LIGHT_LAYERS)
        }
    #endif

//  Additional Lights
    #if defined(_ADDITIONAL_LIGHTS)
        uint pixelLightCount = GetAdditionalLightsCount();

// Clustered!

        LIGHT_LOOP_BEGIN(pixelLightCount)
            Light light = GetAdditionalLight(lightIndex, inputData, shadowMask, aoFactor);
        
        #if defined(_LIGHT_LAYERS)
            if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
            {
        #endif
            //  SSAO does not play nicely with blend?!
                lightingData.additionalLightsColor += LightingHair_Lux(surfaceData.albedo, surfaceData.specular, light, inputData.normalWS, geomNdotV, inputData.viewDirectionWS, pbRoughness1, pbRoughness2, t1, t2, specularTint, secondarySpecularTint, rimTransmissionIntensity);
        #if defined(_LIGHT_LAYERS)
            }
        #endif
        LIGHT_LOOP_END

    #endif

    #ifdef _ADDITIONAL_LIGHTS_VERTEX
//  NOTE: We never set the standard brdfData except from debug mode. So we have to use surfaceData here.
        lightingData.vertexLightingColor += inputData.vertexLighting * surfaceData.albedo; // brdfData.diffuse;
    #endif
    return CalculateFinalColor(lightingData, surfaceData.alpha);
}
#endif