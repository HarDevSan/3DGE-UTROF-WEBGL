#ifndef UNIVERSAL_LIGHTING_INCLUDED
#define UNIVERSAL_LIGHTING_INCLUDED
// necessary otherwise it'll throw en error on the node itself in the graph
// because the node HAS to compile by itself
// so without this, the master node is fine but this node itself won't compile because it doesn't know what "Light" is
struct Light
{
    half3   direction;
    half3   color;
    half    distanceAttenuation;
    half    shadowAttenuation;
};
// struct copied from "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"
#endif

#ifndef SGE_CUSTOM_LIGHTING_INTERNAL_INCLUDED
#define SGE_CUSTOM_LIGHTING_INTERNAL_INCLUDED

struct SGECustomLightingData
{
	float3 albedo;
	float3 normal;
	float3 specular;
	float glossiness;
	float smoothness; // converted from shininess
	float3 emission;
	float alpha;
	float4 customLightingData1; // custom data you can use how you want (pass texture UV, vertex color ...etc)
	float4 customLightingData2;
};

#endif