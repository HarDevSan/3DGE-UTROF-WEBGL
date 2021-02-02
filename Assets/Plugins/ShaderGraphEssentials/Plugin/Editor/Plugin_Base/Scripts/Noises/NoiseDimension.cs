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

namespace ShaderGraphEssentials
{
    public enum NoiseDimension
    {
        // you can have an enum value start with a figure
        D2,
        D3
    }

    public static class NoiseDimensionUtils
    {
        public static string ToDisplayName(this NoiseDimension dimension)
        {
            switch (dimension)
            {
                case NoiseDimension.D2:
                    return "2D";
                case NoiseDimension.D3:
                    return "3D";
            }

            return "Unknown Dimension";
        }

        public static string ToShaderVectorEquivalent(this NoiseDimension dimension)
        {
            switch (dimension)
            {
                case NoiseDimension.D2:
                    return "float2";
                case NoiseDimension.D3:
                    return "float3";
            }

            return "Unknown Dimension";
        }

        public static string[] GetDisplayNames()
        {
            return new[] {NoiseDimension.D2.ToDisplayName(), NoiseDimension.D3.ToDisplayName()};
        }
    }
}