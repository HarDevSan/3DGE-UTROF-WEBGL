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

using System;
using UnityEditor.ShaderGraph;

namespace ShaderGraphEssentials
{
    class ValueNoise : NoiseBase
    {
        public ValueNoise(NoiseCombine combine, NoiseDimension dimension, NoisePeriodicity periodicity) : base(combine, dimension, periodicity)
        {
        }

        public override bool Support()
        {
            return Dimension == NoiseDimension.D2;
        }

        public override NoiseDimension[] GetSupportedDimensions()
        {
            return new[] {NoiseDimension.D2};
        }

        public override NoisePeriodicity[] GetSupportedPeriodicities()
        {
            return new[] { NoisePeriodicity.NonPeriodic, NoisePeriodicity.Periodic };
        }

        public override string GetNoiseFunctionName()
        {
            switch (Combine)
            {
                case NoiseCombine.Simple:
                    return GetSimpleNoiseFunctionName();
                case NoiseCombine.Fractal:
                    return ShaderUtils.GetFractalFunctionName(GetSimpleNoiseFunctionName());
                case NoiseCombine.Turbulence:
                    return ShaderUtils.GetTurbulenceFunctionName(GetSimpleNoiseFunctionName());
                case NoiseCombine.Ridge:
                    return ShaderUtils.GetRidgeFunctionName(GetSimpleNoiseFunctionName());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // function name of the noise, with the combine if any
        private string GetSimpleNoiseFunctionName()
        {
            return Periodicity == NoisePeriodicity.NonPeriodic ? ShaderUtils.ValueNoiseFunctionName : ShaderUtils.PeriodicValueNoiseFunctionName;
        }

        internal override void RegisterFunctions(FunctionRegistry registry)
        {
            ShaderUtils.RandomValue2dTo1dFunction(registry);

            switch (Periodicity)
            {
                case NoisePeriodicity.NonPeriodic:
                    ShaderUtils.ValueNoiseFunction(registry);
                    if (Combine == NoiseCombine.Fractal)
                        ShaderUtils.FractalFunction(registry, GetSimpleNoiseFunctionName());
                    else if (Combine == NoiseCombine.Turbulence)
                        ShaderUtils.TurbulenceFunction(registry, GetSimpleNoiseFunctionName());
                    else if (Combine == NoiseCombine.Ridge)
                        ShaderUtils.RidgeFunction(registry, GetSimpleNoiseFunctionName());
                    break;
                case NoisePeriodicity.Periodic:
                    ShaderUtils.PeriodicValueNoiseFunction(registry);
                    if (Combine == NoiseCombine.Fractal)
                        ShaderUtils.PeriodicFractalFunction(registry, GetSimpleNoiseFunctionName());
                    else if (Combine == NoiseCombine.Turbulence)
                        ShaderUtils.PeriodicTurbulenceFunction(registry, GetSimpleNoiseFunctionName());
                    else if (Combine == NoiseCombine.Ridge)
                        ShaderUtils.PeriodicRidgeFunction(registry, GetSimpleNoiseFunctionName());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool NeedRemapTo0_1()
        {
            return Combine != NoiseCombine.Turbulence && Combine != NoiseCombine.Ridge;
        }
    }
}
