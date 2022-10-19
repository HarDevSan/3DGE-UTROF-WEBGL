﻿Shader "Lux URP/Depth DepthNormal Only"
{
    Properties
    {
        [Header(Surface Options)]
        [Space(8)]
        [Enum(UnityEngine.Rendering.CullMode)]
        _Cull                       ("Culling", Float) = 2
        [Toggle(_ALPHATEST_ON)]
        _AlphaClip                  ("Alpha Clipping", Float) = 0.0
        _Cutoff                     ("     Threshold", Range(0.0, 1.0)) = 0.5

        [Header(Surface Inputs)]
        [Space(8)]
        [MainTexture]
        _BaseMap                    ("Albedo (RGB) Alpha (A)", 2D) = "white" {}

        [Space(5)]
        [Toggle(_NORMALMAP)]
        _ApplyNormal                ("Enable Normal Map", Float) = 0.0
        _BumpMap                    ("     Normal Map", 2D) = "bump" {}
        _BumpScale                  ("     Normal Scale", Float) = 1.0

    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        LOD 100


    //  Depth -----------------------------------------------------
    //  Pass needed to receive proper shadows

        Pass
        {
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            //  Material Inputs
            CBUFFER_START(UnityPerMaterial)
                half    _Cutoff;
                half    _BumpScale;
            CBUFFER_END

            #if defined(_ALPHATEST_ON)
                TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap); float4 _BaseMap_ST;
            #endif

            struct VertexInput {
                float3 positionOS                   : POSITION;
                float2 texcoord                     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput {
                float4 positionCS     : SV_POSITION;
                float2 uv             : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };


            VertexOutput DepthOnlyVertex(VertexInput input)
            {
                VertexOutput output = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                #if defined(_ALPHATEST_ON)
                    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                #endif
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 DepthOnlyFragment(VertexOutput input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                #if defined(_ALPHATEST_ON)
                    half mask = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv.xy).a;
                    clip (mask - _Cutoff);
                #endif
                return 0;
            }

            ENDHLSL
        }

    //  Depth Normal ---------------------------------------------
        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex DepthNormalVertex
            #pragma fragment DepthNormalFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _ALPHATEST_ON      // not per fragment!
            #pragma shader_feature_local _NORMALMAP         // not per fragment!

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            // #pragma multi_compile _ DOTS_INSTANCING_ON // needs shader target 4.5
            
            #define DEPTHNORMALPASS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half    _Cutoff;
                half    _BumpScale;
            CBUFFER_END

            #if defined(_ALPHATEST_ON)
                TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);  float4 _BaseMap_ST;
            #endif

            #if defined(_NORMALMAP)
                TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);  float4 _BumpMap_ST;
            #endif

            struct VertexInput {
                float3 positionOS                   : POSITION;
                float2 texcoord                     : TEXCOORD0;
                float3 normalOS                     : NORMAL;
                #if defined(_NORMALMAP)
                    float4 tangentOS                : TANGENT;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput {
                float4 positionCS     : SV_POSITION;
                float4 uv             : TEXCOORD0;
                half3 normalWS        : TEXCOORD1;
                #if defined(_NORMALMAP)
                    half4 tangentWS   : TEXCOORD2;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput DepthNormalVertex(VertexInput input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                VertexOutput output = (VertexOutput)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                #if defined(_ALPHATEST_ON)
                    output.uv.xy = TRANSFORM_TEX(input.texcoord, _BaseMap);
                #endif
                #if defined(_NORMALMAP)
                    output.uv.zw = TRANSFORM_TEX(input.texcoord, _BumpMap);
                #endif

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                #if defined(_NORMALMAP)
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    output.normalWS = normalInput.normalWS;
                    real sgn = input.tangentOS.w * GetOddNegativeScale();
                    half4 tangentWS = half4(normalInput.tangentWS, sgn);
                    output.tangentWS = tangentWS;
                #else
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, float4(1,1,1,1)); //input.tangentOS);
                    output.normalWS = normalInput.normalWS;
                #endif

                return output;
            }

            half4 DepthNormalFragment(VertexOutput input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                #if defined(_ALPHATEST_ON)
                    half mask = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv.xy).a;
                    clip (mask - _Cutoff);
                #endif

                #if defined(_NORMALMAP)
                    half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv.zw);
                    #if BUMP_SCALE_NOT_SUPPORTED
                        half3 normalTS = UnpackNormal(n);
                    #else
                        half3 normalTS = UnpackNormalScale(n, _BumpScale);
                    #endif
                    float sgn = input.tangentWS.w;      // should be either +1 or -1
                    float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                    input.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent, input.normalWS.xyz));
                #endif

                #if defined(_GBUFFER_NORMALS_OCT)
                    half3 normalWS = normalize(input.normalWS);
                    float2 octNormalWS = PackNormalOctQuadEncode(normalWS);           // values between [-1, +1], must use fp32 on some platforms.
                    float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);   // values between [ 0,  1]
                    half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);      // values between [ 0,  1]
                    return half4(packedNormalWS, 0.0);
                #else
                    half3 normalWS = NormalizeNormalPerPixel(input.normalWS);
                    return half4(normalWS, 0.0);
                #endif

            }
            ENDHLSL
        }


    //  End Passes -----------------------------------------------------
    
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}