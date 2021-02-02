using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Data.Util;
using UnityEditor;

namespace ShaderGraphEssentials
{
    [Serializable]
    class SGECustomLitSubShader : ISGECustomLitSubShader
    {
#region Passes
        ShaderPass m_ForwardPass = new ShaderPass
        {
            // Definition
            displayName = "CustomLit Forward",
            referenceName = "SHADERPASS_FORWARD",
            lightMode = "UniversalForward",
            passInclude = "Assets/Plugins/ShaderGraphEssentials/Plugin/Editor/Plugin_URP/Shaders/CustomLitForwardPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",
            useInPreview = true,

            // Port mask
            vertexPorts = new List<int>()
            {
                SGECustomLitMasterNode.PositionSlotId,
                SGECustomLitMasterNode.VertNormalSlotId,
                SGECustomLitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>
            {
                SGECustomLitMasterNode.AlbedoSlotId,
                SGECustomLitMasterNode.SpecularSlotId,
                SGECustomLitMasterNode.ShininessSlotId,
                SGECustomLitMasterNode.GlossinessSlotId,
                SGECustomLitMasterNode.NormalSlotId,
                SGECustomLitMasterNode.EmissionSlotId,
                SGECustomLitMasterNode.AlphaSlotId,
                SGECustomLitMasterNode.AlphaThresholdSlotId,
                SGECustomLitMasterNode.CustomLightingData1SlotId,
                SGECustomLitMasterNode.CustomLightingData2SlotId
            },

            // Required fields
            requiredAttributes = new List<string>()
            {
                "Attributes.uv1", //needed for meta vertex position
            },

            // Required fields
            requiredVaryings = new List<string>()
            {
                "Varyings.positionWS",
                "Varyings.normalWS",
                "Varyings.tangentWS", //needed for vertex lighting
                "Varyings.viewDirectionWS",
                "Varyings.lightmapUV",
                "Varyings.sh",
                "Varyings.fogFactorAndVertexLight", //fog and vertex lighting, vert input is dependency
                "Varyings.shadowCoord", //shadow coord, vert input is dependency
            },

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl",
                "Assets/Plugins/ShaderGraphEssentials/Plugin/Editor/Plugin_URP/Shaders/SGE_CustomLightingInternal.hlsl"
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_fog",
                "multi_compile_instancing",
            },
            keywords = new KeywordDescriptor[]
            {
                s_LightmapKeyword,
                s_DirectionalLightmapCombinedKeyword,
                s_MainLightShadowsKeyword,
                s_MainLightShadowsCascadeKeyword,
                s_AdditionalLightsKeyword,
                s_AdditionalLightShadowsKeyword,
                s_ShadowsSoftKeyword,
                s_MixedLightingSubtractiveKeyword,
            },
        };

        ShaderPass m_DepthOnlyPass = new ShaderPass()
        {
            // Definition
            displayName = "DepthOnly",
            referenceName = "SHADERPASS_DEPTHONLY",
            lightMode = "DepthOnly",
            passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",
            useInPreview = true,

            // Port mask
            vertexPorts = new List<int>()
            {
                SGECustomLitMasterNode.PositionSlotId,
                SGECustomLitMasterNode.VertNormalSlotId,
                SGECustomLitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>()
            {
                SGECustomLitMasterNode.AlphaSlotId,
                SGECustomLitMasterNode.AlphaThresholdSlotId
            },

            // Render State Overrides
            ZWriteOverride = "ZWrite On",
            ColorMaskOverride = "ColorMask 0",

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
            },
        };

        ShaderPass m_ShadowCasterPass = new ShaderPass()
        {
            // Definition
            displayName = "ShadowCaster",
            referenceName = "SHADERPASS_SHADOWCASTER",
            lightMode = "ShadowCaster",
            passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",
            
            // Port mask
            vertexPorts = new List<int>()
            {
                SGECustomLitMasterNode.PositionSlotId,
                SGECustomLitMasterNode.VertNormalSlotId,
                SGECustomLitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>()
            {
                SGECustomLitMasterNode.AlphaSlotId,
                SGECustomLitMasterNode.AlphaThresholdSlotId
            },

            // Required fields
            requiredAttributes = new List<string>()
            {
                "Attributes.normalOS",
            },

            // Render State Overrides
            ZWriteOverride = "ZWrite On",
            ZTestOverride = "ZTest LEqual",

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
            },
        };
        ShaderPass m_LitMetaPass = new ShaderPass()
        {
            // Definition
            displayName = "Meta",
            referenceName = "SHADERPASS_META",
            lightMode = "Meta",
            passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",

            // Port mask
            vertexPorts = new List<int>()
            {
                SGECustomLitMasterNode.PositionSlotId,
                SGECustomLitMasterNode.VertNormalSlotId,
                SGECustomLitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>()
            {
                SGECustomLitMasterNode.AlbedoSlotId,
                SGECustomLitMasterNode.EmissionSlotId,
                SGECustomLitMasterNode.AlphaSlotId,
                SGECustomLitMasterNode.AlphaThresholdSlotId
            },

            // Required fields
            requiredAttributes = new List<string>()
            {
                "Attributes.uv1", //needed for meta vertex position
                "Attributes.uv2", //needed for meta vertex position
            },

            // Render State Overrides
            ZWriteOverride = "ZWrite On",
            ZTestOverride = "ZTest LEqual",

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl",
                "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
            }
        };

        ShaderPass m_2DPass = new ShaderPass()
        {
            // Definition
            referenceName = "SHADERPASS_2D",
            lightMode = "Universal2D",
            passInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl",
            varyingsInclude = "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl",

            // Port mask
            vertexPorts = new List<int>()
            {
                SGECustomLitMasterNode.PositionSlotId,
                SGECustomLitMasterNode.VertNormalSlotId,
                SGECustomLitMasterNode.VertTangentSlotId
            },
            pixelPorts = new List<int>()
            {
                SGECustomLitMasterNode.AlbedoSlotId,
                SGECustomLitMasterNode.AlphaSlotId,
                SGECustomLitMasterNode.AlphaThresholdSlotId
            },

            // Pass setup
            includes = new List<string>()
            {
                "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl",
                "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl",
                "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            },
            pragmas = new List<string>()
            {
                "prefer_hlslcc gles",
                "exclude_renderers d3d11_9x",
                "target 2.0",
                "multi_compile_instancing",
            },
        };
#endregion

#region Keywords
        static KeywordDescriptor s_LightmapKeyword = new KeywordDescriptor()
        {
            displayName = "Lightmap",
            referenceName = "LIGHTMAP_ON",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_DirectionalLightmapCombinedKeyword = new KeywordDescriptor()
        {
            displayName = "Directional Lightmap Combined",
            referenceName = "DIRLIGHTMAP_COMBINED",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_SampleGIKeyword = new KeywordDescriptor()
        {
            displayName = "Sample GI",
            referenceName = "_SAMPLE_GI",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_MainLightShadowsKeyword = new KeywordDescriptor()
        {
            displayName = "Main Light Shadows",
            referenceName = "_MAIN_LIGHT_SHADOWS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_MainLightShadowsCascadeKeyword = new KeywordDescriptor()
        {
            displayName = "Main Light Shadows Cascade",
            referenceName = "_MAIN_LIGHT_SHADOWS_CASCADE",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_AdditionalLightsKeyword = new KeywordDescriptor()
        {
            displayName = "Additional Lights",
            referenceName = "_ADDITIONAL",
            type = KeywordType.Enum,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
            entries = new KeywordEntry[]
            {
                new KeywordEntry() { displayName = "Vertex", referenceName = "LIGHTS_VERTEX" },
                new KeywordEntry() { displayName = "Fragment", referenceName = "LIGHTS" },
                new KeywordEntry() { displayName = "Off", referenceName = "OFF" },
            }
        };

        static KeywordDescriptor s_AdditionalLightShadowsKeyword = new KeywordDescriptor()
        {
            displayName = "Additional Light Shadows",
            referenceName = "_ADDITIONAL_LIGHT_SHADOWS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_ShadowsSoftKeyword = new KeywordDescriptor()
        {
            displayName = "Shadows Soft",
            referenceName = "_SHADOWS_SOFT",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };

        static KeywordDescriptor s_MixedLightingSubtractiveKeyword = new KeywordDescriptor()
        {
            displayName = "Mixed Lighting Subtractive",
            referenceName = "_MIXED_LIGHTING_SUBTRACTIVE",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Global,
        };
        #endregion

        public int GetPreviewPassIndex() { return 0; }

        ActiveFields GetActiveFieldsFromMasterNode(SGECustomLitMasterNode masterNode, ShaderPass pass)
        {
            var activeFields = new ActiveFields();
            var baseActiveFields = activeFields.baseInstance;

            // Graph Vertex
            if(masterNode.IsSlotConnected(SGECustomLitMasterNode.PositionSlotId) || 
               masterNode.IsSlotConnected(SGECustomLitMasterNode.VertNormalSlotId) || 
               masterNode.IsSlotConnected(SGECustomLitMasterNode.VertTangentSlotId))
            {
                baseActiveFields.Add("features.graphVertex");
            }

            // Graph Pixel (always enabled)
            baseActiveFields.Add("features.graphPixel");

            if (masterNode.IsSlotConnected(SGECustomLitMasterNode.AlphaThresholdSlotId) ||
                masterNode.GetInputSlots<Vector1MaterialSlot>().First(x => x.id == SGECustomLitMasterNode.AlphaThresholdSlotId).value > 0.0f)
            {
                baseActiveFields.Add("AlphaClip");
            }

            if (masterNode.IsSlotConnected(SGECustomLitMasterNode.NormalSlotId))
            {
                baseActiveFields.Add("Normal");
            }
            
            switch(masterNode.normalDropOffSpace)
            {
                case NormalDropOffSpace.Tangent:
                    baseActiveFields.AddAll("features.NormalDropOffTS");
                    break;
                case NormalDropOffSpace.Object:
                    baseActiveFields.AddAll("features.NormalDropOffOS");
                    break;
                case NormalDropOffSpace.World:
                    baseActiveFields.AddAll("features.NormalDropOffWS");
                    break;
                default:
                    UnityEngine.Debug.LogError("Unknown normal drop off space: " + masterNode.normalDropOffSpace);
                    break;
            }

            // Keywords for transparent
            // #pragma shader_feature _SURFACE_TYPE_TRANSPARENT
            if (masterNode.BlendMode != BlendMode.Off)
            {
                // transparent-only defines
                baseActiveFields.Add("SurfaceType.Transparent");

                // #pragma shader_feature _ _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
                if (masterNode.BlendMode == BlendMode.Alpha)
                {
                    baseActiveFields.Add("BlendMode.Alpha");
                }
                else if (masterNode.BlendMode == BlendMode.Additive)
                {
                    baseActiveFields.Add("BlendMode.Add");
                }
                else if (masterNode.BlendMode == BlendMode.Premultiply)
                {
                    baseActiveFields.Add("BlendMode.Premultiply");
                }
            }

            baseActiveFields.Add("SpecularColor");

            return activeFields;
        }

        bool GenerateShaderPass(SGECustomLitMasterNode masterNode, ShaderPass pass, GenerationMode mode, ShaderGenerator result, List<string> sourceAssetDependencyPaths)
        {
            var options = masterNode.GetMaterialOptions();
            SGEShaderGraphUtilities.SetRenderState(options, ref pass);

            // apply master node options to active fields
            var activeFields = GetActiveFieldsFromMasterNode(masterNode, pass);

            return SGEGenerationUtils.GenerateShaderPass(masterNode, pass, mode, activeFields, result, sourceAssetDependencyPaths,
                SGEShaderGraphResources.s_Dependencies, SGEShaderGraphResources.s_ResourceClassName, SGEShaderGraphResources.s_AssemblyName);
        }

        public string GetSubshader(IMasterNode masterNode, GenerationMode mode, List<string> sourceAssetDependencyPaths = null)
        {
            if (sourceAssetDependencyPaths != null)
            {
                // SGECustomLitSubShader.cs
                sourceAssetDependencyPaths.Add(AssetDatabase.GUIDToAssetPath("177588872ef822641941d0e221590b9d"));
            }

            // Master Node data
            var customLitMasterNode = masterNode as SGECustomLitMasterNode;
            var subShader = new ShaderGenerator();

            subShader.AddShaderChunk("SubShader", true);
            subShader.AddShaderChunk("{", true);
            subShader.Indent();
            {
                var surfaceTags = customLitMasterNode.GetMaterialTags();
                var tagsBuilder = new ShaderStringBuilder(0);
                surfaceTags.GetTags(tagsBuilder, "UniversalPipeline");
                subShader.AddShaderChunk(tagsBuilder.ToString());
                
                GenerateShaderPass(customLitMasterNode, m_ForwardPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPass(customLitMasterNode, m_ShadowCasterPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPass(customLitMasterNode, m_DepthOnlyPass, mode, subShader, sourceAssetDependencyPaths);
                GenerateShaderPass(customLitMasterNode, m_LitMetaPass, mode, subShader, sourceAssetDependencyPaths);
                //GenerateShaderPass(customLitMasterNode, m_2DPass, mode, subShader, sourceAssetDependencyPaths);
            }
            subShader.Deindent();
            subShader.AddShaderChunk("}", true);
            
            if (customLitMasterNode.CustomEditor != null && !customLitMasterNode.CustomEditor.Equals(String.Empty))
                subShader.AddShaderChunk("CustomEditor \"" + customLitMasterNode.CustomEditor + "\"", true);

            return subShader.GetShaderString(0);
        }

        public bool IsPipelineCompatible(RenderPipelineAsset renderPipelineAsset)
        {
            return renderPipelineAsset is UniversalRenderPipelineAsset;
        }

        public SGECustomLitSubShader() { }
    }
}
