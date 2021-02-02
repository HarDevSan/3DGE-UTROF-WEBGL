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

using System.Reflection;
using UnityEditor.ShaderGraph;
using UnityEngine;

[Title("Artistic", "Normal", "Create Normal U Offset")]
class NormalUOffsetNode : CodeFunctionNode
{
    public NormalUOffsetNode()
    {
        name = "Create Normal U Offset";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("NormalUOffsetFunction",
            BindingFlags.Static | BindingFlags.NonPublic);
    }

    static string NormalUOffsetFunction(
        [Slot(0, Binding.MeshUV0)] Vector2 Uv,
        [Slot(1, Binding.None, 1f, 1f, 1f, 1f)] Vector1 Offset,
        [Slot(2, Binding.None)] out Vector2 Out)
    {
        Out = Vector2.zero;
        return

            @"
{
    float offset = pow(Offset, 3) * 0.1;
    Out = float2(Uv.x + offset, Uv.y);
} 
";
    }
}