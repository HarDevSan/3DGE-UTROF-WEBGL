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

/*
 * NOTES:
 * Both of SGE_DiffuseLightingCustom and SGE_SpecularLightingCustom functions are called for each pixel light
 * The name is hardcoded and very important, don't change it !
 * Also, you can't access Uniforms / Properties directly in there,
    nor pass it to the custom function node
    trying either of this will throw an error
    but you can pass it through data.customLightingData1 and use it how you see fit.
    (there's an example of this in SGE_ToonLighting.hlsl)
*/

// this will function is called for every light
half3 SGE_DiffuseLightingCustom(Light light, SGECustomLightingData data)
{
    float NdotL = saturate(dot(data.normal, light.direction));
    return light.color * NdotL * light.distanceAttenuation * light.shadowAttenuation;
}

// this will function is called for every light
half3 SGE_SpecularLightingCustom(Light light, half3 viewDir, SGECustomLightingData data)
{
    float3 halfVec = SafeNormalize(light.direction + viewDir);
    float NdotH = saturate(dot(data.normal, halfVec));
    float modifier = pow(NdotH, data.smoothness);
    half3 specularReflection = data.specular * modifier;
    return light.color * specularReflection * light.distanceAttenuation * light.shadowAttenuation;
}

// necessary function because at the moment a custom function node NEEDS to output a result in the graph
// otherwise it can be optimized out
// here we don't do anything, so it doesn't matter where you plug this node,
// because the custom lighting function will be called by the master node
void Passthrough_float(float4 In, out float4 Out)
{
    Out = In;
}