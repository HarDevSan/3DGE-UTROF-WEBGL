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

[Title("Artistic", "Normal", "Create Normal from Height vector")]
class NormalFromHeightVectorNode : CodeFunctionNode
{
    public NormalFromHeightVectorNode()
    {
        name = "Create Normal from Height vector";
    }

    protected override MethodInfo GetFunctionToConvert()
    {
        return GetType().GetMethod("NormalFromHeightVectorFunction",
            BindingFlags.Static | BindingFlags.NonPublic);
    }

    static string NormalFromHeightVectorFunction(
        [Slot(0, Binding.None)] Vector1 Height,
        [Slot(1, Binding.None)] Vector1 UOffset,
        [Slot(2, Binding.None)] Vector1 VOffset,
        [Slot(3, Binding.None, 1f, 1f, 1f, 1f)] Vector1 Strength,
        [Slot(4, Binding.None)] out Vector3 Out)
    {
        Out = Vector3.forward;
        return

            @"
{
    float3 va = float3(1, 0, (UOffset - Height) * Strength);
    float3 vb = float3(0, 1, (VOffset - Height) * Strength);
    Out = normalize(cross(va, vb));
} 
";
    }
}
