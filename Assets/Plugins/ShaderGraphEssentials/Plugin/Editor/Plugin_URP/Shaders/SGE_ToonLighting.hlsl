#include "Assets/Plugins/ShaderGraphEssentials/Plugin/Editor/Plugin_URP/Shaders/SGE_CustomLightingInternal.hlsl"

/*
 * This is what you can access in the custom functions
struct Light
{
    half3   direction;
    half3   color;
    half    distanceAttenuation;
    half    shadowAttenuation;
};

struct SGECustomLightingData
{
    float3 albedo;
    float3 normal; // world space
    float3 specular;
    float glossiness;
    float smoothness; // automatically converted from shininess
    float3 emission;
    float alpha;
    float4 customLightingData1; // custom data you can use how you want (pass texture UV, properties / uniforms, vertex color ...etc)
    float4 customLightingData2;
};
*/

// this will function is called for every light
half3 SGE_DiffuseLightingCustom(Light light, SGECustomLightingData data)
{
    half NdotL = saturate(dot(data.normal, light.direction));
    half lightIntensity = smoothstep(0, 0.01, NdotL);   
    return light.color * lightIntensity * light.distanceAttenuation * light.shadowAttenuation;
}

// this will function is called for every light
// you can't access Uniforms directly in there, 
// nor pass it to the custom function node
// trying either of this will throw an error
// but you can pass it through data.customLightingData1 and use it how you see fit here.
half3 SGE_SpecularLightingCustom(Light light, half3 viewDir, SGECustomLightingData data)
{
    const float EPSILON = 0.01f;
    // compute specular
    half NdotL = saturate(dot(data.normal, light.direction));
    half lightIntensity = smoothstep(0, EPSILON, NdotL);   
    float3 halfVec = SafeNormalize(light.direction + viewDir);
    float NdotH = saturate(dot(data.normal, halfVec));
    float modifier = pow(NdotH * lightIntensity, data.smoothness);
    float modifierSmooth = smoothstep(EPSILON * 0.5, EPSILON, modifier);
    half3 specularReflection = data.specular * modifierSmooth;

    // compute rimlight
    // data.customLightingData1: xyz = _RimColor
    // data.customLightingData2: x = _RimThreshold, y = _RimAmount
    half rimDot = 1 - dot(viewDir, data.normal);
    half rimIntensity = rimDot * pow(NdotL, (half)data.customLightingData2.x);
    rimIntensity = smoothstep(data.customLightingData2.y - EPSILON, data.customLightingData2.y + EPSILON, rimIntensity);
    half3 rim = rimIntensity * data.customLightingData1.xyz;

    // I chose to have the rim affected by the light's color, but feel free to improvise!
    return light.color * (specularReflection * light.distanceAttenuation * light.shadowAttenuation + rim);
}

// necessary function because at the moment a custom function node NEEDS to output a result in the graph
// otherwise it can be optimized out
// here we don't do anything, so it doesn't matter where you plug this node,
// because the custom lighting function will be called by the master node
void Passthrough_float(float4 In, out float4 Out)
{
    Out = In;
}