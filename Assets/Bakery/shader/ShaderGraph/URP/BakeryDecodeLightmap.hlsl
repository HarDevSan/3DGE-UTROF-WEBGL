#define BAKERY_INV_PI        0.31830988618f

sampler2D _RNM0, _RNM1, _RNM2;

Texture3D _Volume0, _Volume1, _Volume2, _VolumeMask;
SamplerState sampler_Volume0;
SamplerState sampler_VolumeMask;
float4x4 _VolumeMatrix, _GlobalVolumeMatrix;
float3 _VolumeMin, _VolumeInvSize;
float3 _GlobalVolumeMin, _GlobalVolumeInvSize;

void LightmapUV_float(float2 uv, out float2 lightmapUV)
{
    lightmapUV = uv * unity_LightmapST.xy + unity_LightmapST.zw;
}

void DecodeLightmap(float4 lightmap, out float3 result)
{

#ifdef UNITY_LIGHTMAP_FULL_HDR
    float4 decodeInstructions = float4(0.0, 0.0, 0.0, 0.0); // Never used but needed for the interface since it supports gamma lightmaps
#else
    #if defined(UNITY_LIGHTMAP_RGBM_ENCODING)
        float4 decodeInstructions = float4(34.493242, 2.2, 0.0, 0.0); // range^2.2 = 5^2.2, gamma = 2.2
    #else
        float4 decodeInstructions = float4(2.0, 2.2, 0.0, 0.0); // range = 2.0^2.2 = 4.59
    #endif
#endif

	result = DecodeLightmap(lightmap, decodeInstructions);
}

void SampleRNM0_float(float2 lightmapUV, out float3 result)
{
    DecodeLightmap(tex2D(_RNM0, lightmapUV), result);
}

void SampleRNM1_float(float2 lightmapUV, out float3 result)
{
    DecodeLightmap(tex2D(_RNM1, lightmapUV), result);
}

void SampleRNM2_float(float2 lightmapUV, out float3 result)
{
    DecodeLightmap(tex2D(_RNM2, lightmapUV), result);
}

void SampleL1x_float(float2 lightmapUV, out float3 result)
{
    result = tex2D(_RNM0, lightmapUV);
}

void SampleL1y_float(float2 lightmapUV, out float3 result)
{
    result = tex2D(_RNM1, lightmapUV);
}

void SampleL1z_float(float2 lightmapUV, out float3 result)
{
    result = tex2D(_RNM2, lightmapUV);
}

// Following two functions are copied from the original Unity standard shader for compatibility
// -----
float SmoothnessToPerceptualRoughness(float smoothness)
{
    return (1 - smoothness);
}
float BakeryPerceptualRoughnessToRoughness(float perceptualRoughness)
{
    return perceptualRoughness * perceptualRoughness;
}
float GGXTerm (half NdotH, half roughness)
{
    half a2 = roughness * roughness;
    half d = (NdotH * a2 - NdotH) * NdotH + 1.0f; // 2 mad
    return BAKERY_INV_PI * a2 / (d * d + 1e-7f); // This function is not intended to be running on Mobile,
                                            // therefore epsilon is smaller than what can be represented by half
}

#define UNITY_SPECCUBE_LOD_STEPS 6

float BakeryPerceptualRoughnessToMipmapLevel(float perceptualRoughness, uint mipMapCount)
{
    perceptualRoughness = perceptualRoughness * (1.7 - 0.7 * perceptualRoughness);

    return perceptualRoughness * mipMapCount;
}

float BakeryPerceptualRoughnessToMipmapLevel(float perceptualRoughness)
{
    return BakeryPerceptualRoughnessToMipmapLevel(perceptualRoughness, UNITY_SPECCUBE_LOD_STEPS);
}

#define unity_ColorSpaceDielectricSpec half4(0.04, 0.04, 0.04, 1.0 - 0.04) // standard dielectric reflectivity coef at incident angle (= 4%)

// -----

void DirectionalSpecular_float(float2 lightmapUV, float3 normalWorld, float3 viewDir, float smoothness, out float3 color)
{
#ifdef LIGHTMAP_ON
#ifdef DIRLIGHTMAP_COMBINED
    float4 lmColor = unity_Lightmap.Sample(samplerunity_Lightmap, lightmapUV);
    float3 lmDir = unity_LightmapInd.Sample(samplerunity_Lightmap, lightmapUV) * 2 - 1;
    float3 halfDir = normalize(normalize(lmDir) + viewDir);
    float nh = saturate(dot(normalWorld, halfDir));
    float perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
    float roughness = BakeryPerceptualRoughnessToRoughness(perceptualRoughness);
    float spec = GGXTerm(nh, roughness);
    color = lmColor * spec * 0.99999;
    return;
#endif
#endif
    color = 0;
}

float shEvaluateDiffuseL1Geomerics(float L0, float3 L1, float3 n)
{
    // average energy
    float R0 = L0;

    // avg direction of incoming light
    float3 R1 = 0.5f * L1;

    // directional brightness
    float lenR1 = length(R1);

    // linear angle between normal and direction 0-1
    //float q = 0.5f * (1.0f + dot(R1 / lenR1, n));
    //float q = dot(R1 / lenR1, n) * 0.5 + 0.5;
    float q = dot(normalize(R1), n) * 0.5 + 0.5;

    // power for q
    // lerps from 1 (linear) to 3 (cubic) based on directionality
    float p = 1.0f + 2.0f * lenR1 / R0;

    // dynamic range constant
    // should vary between 4 (highly directional) and 0 (ambient)
    float a = (1.0f - lenR1 / R0) / (1.0f + lenR1 / R0);

    return R0 * (a + (1.0f - a) * (p + 1.0f) * pow(q, p));
}

void BakerySH_float(float3 L0, float3 normalWorld, float2 lightmapUV, out float3 sh)
{
    float3 nL1x = tex2D(_RNM0, lightmapUV) * 2 - 1;
    float3 nL1y = tex2D(_RNM1, lightmapUV) * 2 - 1;
    float3 nL1z = tex2D(_RNM2, lightmapUV) * 2 - 1;
    float3 L1x = nL1x * L0 * 2;
    float3 L1y = nL1y * L0 * 2;
    float3 L1z = nL1z * L0 * 2;

    float lumaL0 = dot(L0, 1);
    float lumaL1x = dot(L1x, 1);
    float lumaL1y = dot(L1y, 1);
    float lumaL1z = dot(L1z, 1);
    float lumaSH = shEvaluateDiffuseL1Geomerics(lumaL0, float3(lumaL1x, lumaL1y, lumaL1z), normalWorld);

    sh = L0 + normalWorld.x * L1x + normalWorld.y * L1y + normalWorld.z * L1z;
    float regularLumaSH = dot(sh, 1);

    sh *= lerp(1, lumaSH / regularLumaSH, saturate(regularLumaSH*16));

    sh = max(sh, 0);
}


void BakerySpecSHFull_float(float3 L0, float3 normalWorld, float2 lightmapUV, float3 viewDir, float smoothness, float3 albedo, float metalness,
                                                                                                    out float3 diffuseSH, out float3 specularSH)
{
    float3 nL1x = tex2D(_RNM0, lightmapUV) * 2 - 1;
    float3 nL1y = tex2D(_RNM1, lightmapUV) * 2 - 1;
    float3 nL1z = tex2D(_RNM2, lightmapUV) * 2 - 1;
    float3 L1x = nL1x * L0 * 2;
    float3 L1y = nL1y * L0 * 2;
    float3 L1z = nL1z * L0 * 2;

    float lumaL0 = dot(L0, 1);
    float lumaL1x = dot(L1x, 1);
    float lumaL1y = dot(L1y, 1);
    float lumaL1z = dot(L1z, 1);
    float lumaSH = shEvaluateDiffuseL1Geomerics(lumaL0, float3(lumaL1x, lumaL1y, lumaL1z), normalWorld);

    diffuseSH = L0 + normalWorld.x * L1x + normalWorld.y * L1y + normalWorld.z * L1z;
    float regularLumaSH = dot(diffuseSH, 1);

    diffuseSH *= lerp(1, lumaSH / regularLumaSH, saturate(regularLumaSH*16));
    diffuseSH = max(diffuseSH, 0.0);

    const float3 lumaConv = float3(0.2125f, 0.7154f, 0.0721f);

    float3 dominantDir = float3(dot(nL1x, lumaConv), dot(nL1y, lumaConv), dot(nL1z, lumaConv));
    float focus = saturate(length(dominantDir));
    float3 halfDir = normalize(normalize(dominantDir) - -viewDir);
    float nh = saturate(dot(normalWorld, halfDir));
    float perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
    float roughness = BakeryPerceptualRoughnessToRoughness(perceptualRoughness);
    float spec = GGXTerm(nh, roughness);

    specularSH = L0 + dominantDir.x * L1x + dominantDir.y * L1y + dominantDir.z * L1z;

    specularSH = max(spec * specularSH, 0.0);

    // Convert metalness to specular and "oneMinusReflectivity"
    float3 specularColor = lerp(float3(0.04, 0.04, 0.04), albedo, metalness);
    float oneMinusDielectricSpec = 1.0 - 0.04;
    float oneMinusReflectivity = oneMinusDielectricSpec - metalness * oneMinusDielectricSpec;

    // Directly apply fresnel and smoothness-dependent grazing term
    float nv = 1.0f - saturate(dot(normalWorld, viewDir));
    float nv2 = nv * nv;
    float fresnel = nv * nv2 * nv2;

    float reflectivity = max(max(specularColor.r, specularColor.g), specularColor.b); // hack, but consistent with Unity code
    float grazingTerm = saturate(smoothness + reflectivity);
    float3 fresnel3 = lerp(specularColor, float3(grazingTerm, grazingTerm, grazingTerm), fresnel);

    diffuseSH *= oneMinusReflectivity; // no baked GI override: modify diffuse
    specularSH *= fresnel3;

    diffuseSH = max(diffuseSH, 0);
    specularSH = max(specularSH, 0);

}

void BakeryVolume(float3 lpUV, float3 normalWorld, out float3 sh)
{
    float4 tex0, tex1, tex2;
    float3 L0, L1x, L1y, L1z;
    tex0 = _Volume0.Sample(sampler_Volume0, lpUV);
    tex1 = _Volume1.Sample(sampler_Volume0, lpUV);
    tex2 = _Volume2.Sample(sampler_Volume0, lpUV);
    L0 = tex0.xyz;
    L1x = tex1.xyz;
    L1y = tex2.xyz;
    L1z = float3(tex0.w, tex1.w, tex2.w);
    sh.r = shEvaluateDiffuseL1Geomerics(L0.r, float3(L1x.r, L1y.r, L1z.r), normalWorld);
    sh.g = shEvaluateDiffuseL1Geomerics(L0.g, float3(L1x.g, L1y.g, L1z.g), normalWorld);
    sh.b = shEvaluateDiffuseL1Geomerics(L0.b, float3(L1x.b, L1y.b, L1z.b), normalWorld);
    sh = max(sh, 0);
}

void BakeryVolume_float(float3 posWorld, float3 normalWorld, out float3 sh)
{
    bool isGlobal = dot(abs(_VolumeInvSize),1) == 0;
    float3 lpUV = (posWorld - (isGlobal ? _GlobalVolumeMin : _VolumeMin)) * (isGlobal ? _GlobalVolumeInvSize : _VolumeInvSize);
    BakeryVolume(lpUV, normalWorld, sh);
}

void BakeryVolumeRotatable_float(float3 posWorld, float3 normalWorld, out float3 sh)
{
    bool isGlobal = dot(abs(_VolumeInvSize),1) == 0;

    float4x4 volMatrix = (isGlobal ? _GlobalVolumeMatrix : _VolumeMatrix);
    float3 volInvSize = (isGlobal ? _GlobalVolumeInvSize : _VolumeInvSize);
    float3 lpUV = mul(volMatrix, float4(posWorld,1)).xyz * volInvSize + 0.5f;
    normalWorld = mul((float3x3)volMatrix, normalWorld);

    BakeryVolume(lpUV, normalWorld, sh);
}

void BakeryVolumeSpec_float(float3 posWorld, float3 normalWorld, float3 viewDir, float smoothness, float3 albedo, float metalness,
                                                                                                    out float3 diffuseSH, out float3 specularSH)
{
    bool isGlobal = dot(abs(_VolumeInvSize),1) == 0;
    float3 lpUV = (posWorld - (isGlobal ? _GlobalVolumeMin : _VolumeMin)) * (isGlobal ? _GlobalVolumeInvSize : _VolumeInvSize);

    float4 tex0, tex1, tex2;
    float3 L0, L1x, L1y, L1z;
    tex0 = _Volume0.Sample(sampler_Volume0, lpUV);
    tex1 = _Volume1.Sample(sampler_Volume0, lpUV);
    tex2 = _Volume2.Sample(sampler_Volume0, lpUV);
    L0 = tex0.xyz;
    L1x = tex1.xyz;
    L1y = tex2.xyz;
    L1z = float3(tex0.w, tex1.w, tex2.w);
    diffuseSH.r = shEvaluateDiffuseL1Geomerics(L0.r, float3(L1x.r, L1y.r, L1z.r), normalWorld);
    diffuseSH.g = shEvaluateDiffuseL1Geomerics(L0.g, float3(L1x.g, L1y.g, L1z.g), normalWorld);
    diffuseSH.b = shEvaluateDiffuseL1Geomerics(L0.b, float3(L1x.b, L1y.b, L1z.b), normalWorld);
    diffuseSH = max(diffuseSH, 0);

    const float3 lumaConv = float3(0.2125f, 0.7154f, 0.0721f);

    float3 nL1x = L1x / L0;
    float3 nL1y = L1y / L0;
    float3 nL1z = L1z / L0;
    float3 dominantDir = float3(dot(nL1x, lumaConv), dot(nL1y, lumaConv), dot(nL1z, lumaConv));
    float3 halfDir = normalize(normalize(dominantDir) - -viewDir);
    float nh = saturate(dot(normalWorld, halfDir));
    float perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
    float roughness = BakeryPerceptualRoughnessToRoughness(perceptualRoughness);
    float spec = GGXTerm(nh, roughness);

    specularSH = L0 + dominantDir.x * L1x + dominantDir.y * L1y + dominantDir.z * L1z;

    specularSH = max(spec * specularSH, 0.0);

    // Convert metalness to specular and "oneMinusReflectivity"
    float3 specularColor = lerp(float3(0.04, 0.04, 0.04), albedo, metalness);
    float oneMinusDielectricSpec = 1.0 - 0.04;
    float oneMinusReflectivity = oneMinusDielectricSpec - metalness * oneMinusDielectricSpec;

    // Directly apply fresnel and smoothness-dependent grazing term
    float nv = 1.0f - saturate(dot(normalWorld, viewDir));
    float nv2 = nv * nv;
    float fresnel = nv * nv2 * nv2;

    float reflectivity = max(max(specularColor.r, specularColor.g), specularColor.b); // hack, but consistent with Unity code
    float grazingTerm = saturate(smoothness + reflectivity);
    float3 fresnel3 = lerp(specularColor, float3(grazingTerm, grazingTerm, grazingTerm), fresnel);

    diffuseSH *= oneMinusReflectivity; // no baked GI override: modify diffuse
    specularSH *= fresnel3;

    diffuseSH = max(diffuseSH, 0);
    specularSH = max(specularSH, 0);
}

void URPMainLightDiffuse_float(float3 normalWorld, float3 albedo, out float3 diffuseLight)
{
    #ifndef UNIVERSAL_LIGHTING_INCLUDED
        float3 direction = float3(0.5, 0.5, 1);
        float3 color = 1;
    #else
        Light mainLight = GetMainLight();
        float3 direction = mainLight.direction;
        float3 color = mainLight.color;
    #endif

    diffuseLight = saturate(dot(normalWorld, direction)) * albedo * color;
}

void VolumeShadowmaskA_float(float3 posWorld, out float shadowmask)
{
    bool isGlobal = dot(abs(_VolumeInvSize),1) == 0;
    float3 lpUV = (posWorld - (isGlobal ? _GlobalVolumeMin : _VolumeMin)) * (isGlobal ? _GlobalVolumeInvSize : _VolumeInvSize);
    shadowmask = _VolumeMask.Sample(sampler_VolumeMask, lpUV).a;
}

void SmoothnessToMip_float(float smoothness, out float mip)
{
    float pr = SmoothnessToPerceptualRoughness(smoothness);
    mip = BakeryPerceptualRoughnessToMipmapLevel(pr);
}

void WeightReflection_float(float smoothness, float metallic, float occlusion,
                                     float3 baseColor, float3 normal, float3 viewDir, float3 reflection,
                                     out float3 newReflection)
{
    half perceptualRoughness = SmoothnessToPerceptualRoughness(smoothness);
    half roughness = BakeryPerceptualRoughnessToRoughness(perceptualRoughness);
    half surfaceReduction = 1.0 / (roughness*roughness + 1.0);

    float3 pSpecular = lerp(unity_ColorSpaceDielectricSpec.rgb, baseColor, metallic);
    //baseColor = lerp(baseColor, 0, metallic);

    half reflectivity = max(max(pSpecular.r, pSpecular.g), pSpecular.b);
    half grazingTerm = saturate(smoothness + reflectivity);

    surfaceReduction *= occlusion;

    float3 pNdotV = saturate(dot(normal, viewDir));

    float fresnel = 1 - pNdotV;
    float t2 = fresnel * fresnel;
    fresnel *= t2 * t2;

    newReflection = surfaceReduction * reflection * lerp(pSpecular, grazingTerm, fresnel);
}

void GetURPShadow_float(float3 worldPos, float3 worldNormal, float3 bakedGI, out float3 modifiedGI, out float shadow)
{
#ifdef UNIVERSAL_SHADOWS_INCLUDED
    #ifdef _MAIN_LIGHT_SHADOWS_CASCADE
        half cascadeIndex = ComputeCascadeIndex(worldPos);
    #else
        half cascadeIndex = 0;
    #endif

    float4 shadowCoord = mul(_MainLightWorldToShadow[cascadeIndex], float4(worldPos, 1.0));

    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    half4 shadowParams = _MainLightShadowParams;

    shadow =  SampleShadowmap(TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture), shadowCoord, shadowSamplingData, shadowParams, false);

    float3 lightDir = _MainLightPosition.xyz;
    float3 lightColor = _MainLightColor.rgb;

    half shadowStrength = GetMainLightShadowStrength();
    half contributionTerm = saturate(dot(lightDir, worldNormal));
    half3 lambert = lightColor * contributionTerm;
    half3 estimatedLightContributionMaskedByInverseOfShadow = lambert * (1.0 - shadow);
    half3 subtractedLightmap = bakedGI - estimatedLightContributionMaskedByInverseOfShadow;

    // Subtractive shadows are awful
    // But it is the only thing URP supports ¯\_(ツ)_/¯

    half3 realtimeShadow = max(subtractedLightmap, _SubtractiveShadowColor.xyz);
    realtimeShadow = lerp(bakedGI, realtimeShadow, shadowStrength);

    modifiedGI = min(bakedGI, realtimeShadow);
#else
    modifiedGI = bakedGI; // shadows are undefined in this scene!
    shadow = 1;
#endif
}

void NonLinearLightProbe_float(float3 normalWorld, out float3 color)
{
    float3 L0 = float3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);
    color.r = shEvaluateDiffuseL1Geomerics(L0.r, unity_SHAr.xyz, normalWorld);
    color.g = shEvaluateDiffuseL1Geomerics(L0.g, unity_SHAg.xyz, normalWorld);
    color.b = shEvaluateDiffuseL1Geomerics(L0.b, unity_SHAb.xyz, normalWorld);
}
