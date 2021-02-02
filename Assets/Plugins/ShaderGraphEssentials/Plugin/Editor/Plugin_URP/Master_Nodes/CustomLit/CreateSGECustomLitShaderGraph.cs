//
// ShaderGraphEssentials for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace ShaderGraphEssentials
{
    class CreateSGECustomLitShaderGraph : EndNameEditAction
    {
        private const string DefaultCustomLightingGraphPath =
            "Assets/Plugins/ShaderGraphEssentials/Plugin/Editor/Plugin_URP/Shaders/SGE_DefaultCustomLightingGraph.shadergraph";
        
        [MenuItem("Assets/Create/Shader/SGE Custom Lit Graph", false, 208)]
        public static void CreateMaterialGraph()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreateSGECustomLitShaderGraph>(),
                string.Format("New Shader Graph.{0}", ShaderGraphImporter.Extension), null, null);
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            // we try to copy the default already have the custom lighting function node setup
            bool success = AssetDatabase.CopyAsset(DefaultCustomLightingGraphPath, pathName);
            // if it fails we fallback to the "official" method
            if (!success)
            {
                Debug.LogWarning("ShaderGraphEssentials couldn't create the new custom lit graph by copying the default from " + DefaultCustomLightingGraphPath + 
                                 " to " + pathName + ". An empty graph has been created instead, you now have to create the custom lighting function node before it will compile correctly.");
                var graph = new GraphData();
                graph.AddNode(new SGECustomLitMasterNode());
                graph.path = "Shader Graphs";
                File.WriteAllText(pathName, EditorJsonUtility.ToJson(graph));
            }
            
            AssetDatabase.Refresh();
        }
    }
}
