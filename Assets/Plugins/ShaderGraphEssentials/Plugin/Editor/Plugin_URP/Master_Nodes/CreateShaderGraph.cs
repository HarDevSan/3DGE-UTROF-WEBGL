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

namespace ShaderGraphEssentials
{
    class CreateShaderGraph
    {
        [MenuItem("Assets/Create/Shader/SGE Unlit Graph", false, 208)]
        public static void CreateSGEUnlitMasterMaterialGraph()
        {
            GraphUtil.CreateNewGraph(new SGEUnlitMasterNode());
        }
        
        [MenuItem("Assets/Create/Shader/SGE SimpleLit Graph", false, 208)]
        public static void CreateSGESimpleLitMasterMaterialGraph()
        {
            GraphUtil.CreateNewGraph(new SGESimpleLitMasterNode());
        }
    }
}
