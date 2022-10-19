Shader "Lux URP/Toon Outline"
{
    Properties
    {
        [HeaderHelpLuxURP_URL(68hb5r7b3dnz)]

        [Space(8)]
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Int) = 4
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Culling", Float) = 1

        [Header(Outline)]
        [Space(8)]
        _Color ("Color", Color) = (0,0,0,1)
        _Border ("Width", Float) = 3
        [Toggle(_COMPENSATESCALE)]
        _CompensateScale            ("     Compensate Scale", Float) = 0
        [Toggle(_OUTLINEINSCREENSPACE)]
        _OutlineInScreenSpace       ("     Calculate width in Screen Space", Float) = 0


        [Toggle(_ALPHATEST_ON)]
        _AlphaClip                  ("Alpha Clipping", Float) = 0.0
        [LuxURPHelpDrawer]
        _Help ("Enabling Alpha Clipping needs you to enable and assign the Albedo (RGB) Alpha (A) Map as well.", Float) = 0.0
        _Cutoff                     ("     Threshold", Range(0.0, 1.0)) = 0.5

        [MainTexture]
        _BaseMap                    ("Albedo (RGB) Alpha (A)", 2D) = "white" {}


    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
            "IgnoreProjector" = "True"
        }
        
        Pass
        {
            Name "StandardUnlit"
            Tags{"LightMode" = "UniversalForwardOnly"}

            Blend SrcAlpha OneMinusSrcAlpha
            Cull [_Cull]
            ZTest [_ZTest]
        //  Make sure we do not get overwritten
            ZWrite On

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _COMPENSATESCALE
            #pragma shader_feature_local _OUTLINEINSCREENSPACE
            #pragma shader_feature_local _ALPHATEST_ON

            #define LITPASS

            // -------------------------------------
            // Universal Pipeline keywords

            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile_fog

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Includes/Lux URP Toon Outline Passes.hlsl"

            ENDHLSL
        }


    //  Depth -----------------------------------------------------
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _COMPENSATESCALE
            #pragma shader_feature_local _OUTLINEINSCREENSPACE
            #pragma shader_feature_local _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Includes/Lux URP Toon Outline Passes.hlsl"
            
            ENDHLSL
        }

    //  Depth Normal -----------------------------------------------------
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            ColorMask 0
            Cull [_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _COMPENSATESCALE
            #pragma shader_feature_local _OUTLINEINSCREENSPACE
            #pragma shader_feature_local _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Includes/Lux URP Toon Outline Passes.hlsl"
            
            ENDHLSL
        }

    }
    FallBack "Hidden/InternalErrorShader"
}