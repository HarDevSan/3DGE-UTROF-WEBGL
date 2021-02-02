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
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace ShaderGraphEssentials
{
    [Title("Procedural", "Noise", "SGE Noise")]
    class NoiseNode : AbstractMaterialNode, IGeneratesBodyCode, IGeneratesFunction, IMayRequireMeshUV
    {
        public override bool hasPreview { get { return true; } }

        const int UvSlotId = 0;
        const int PeriodSlotId = 1;
        const int PersistenceSlotId = 2;
        const int LacunaritySlotId = 3;
        const int SharpnessSlotId = 5;
        const int OutputSlotId = 4;
        const string kUvSlotName = "Uv";
        const string kPeriodSlotName = "Period";
        const string kPersistenceSlotName = "Persistence";
        const string kLacunaritySlotName = "Lacunarity";
        const string kSharpnessSlotName = "Sharpness";
        const string kOutputSlotName = "Out";

        public NoiseBase Noise;

        [SerializeField]
        private NoiseType m_noiseType = NoiseType.Value;
        [EnumControl("Type")]
        public NoiseType Type
        {
            get { return m_noiseType; }
            set
            {
                if (m_noiseType == value)
                    return;

                m_noiseType = value;
                InitializeNoiseAccordingToType();
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private NoiseCombine m_noiseCombine = NoiseCombine.Simple;
        [EnumControl("Combine")]
        public NoiseCombine Combine
        {
            get { return m_noiseCombine; }
            set
            {
                if (m_noiseCombine == value)
                    return;

                m_noiseCombine = value;
                Noise.Combine = value;
                UpdateSlots();
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private NoiseDimension m_noiseDimension = NoiseDimension.D2;
        [NoiseDimensionEnumControl("Dimension")]
        public NoiseDimension Dimension
        {
            get
            {
                return m_noiseDimension;
            }
            set
            {
                if (m_noiseDimension == value)
                    return;
                m_noiseDimension = value;
                Noise.Dimension = value;
                UpdateSlots();
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        private NoisePeriodicity m_noisePeriodicity = NoisePeriodicity.NonPeriodic;
        [NoisePeriodicityEnumControl("Periodicity")]
        public NoisePeriodicity Periodicity
        {
            get
            {
                return m_noisePeriodicity;
            }
            set
            {
                if (m_noisePeriodicity == value)
                    return;
                m_noisePeriodicity = value;
                Noise.Periodicity = value;
                UpdateSlots();
                Dirty(ModificationScope.Graph);
            }
        }

        public NoiseNode()
        {
            name = "SGE Noise";
            UpdateNodeAfterDeserialization();
        }

        private void InitializeNoiseAccordingToType()
        {
            switch (m_noiseType)
            {
                case NoiseType.Value:
                    Noise = new ValueNoise(m_noiseCombine, m_noiseDimension, m_noisePeriodicity);
                    break;
                case NoiseType.Simplex:
                    Noise = new SimplexNoise(m_noiseCombine, m_noiseDimension, m_noisePeriodicity);
                    break;
                case NoiseType.Perlin:
                    Noise = new PerlinNoise(m_noiseCombine, m_noiseDimension, m_noisePeriodicity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_noiseType), m_noiseType, null);
            }

            UpdateSlots();
        }

        private void ValidateNoisePeriodicity()
        {
            if (Noise == null)
                return;

            var supported = Noise.GetSupportedPeriodicities();
            bool isCurrentPeriodicitySupported = false;
            for (int i = 0; i < supported.Length; i++)
            {
                if (supported[i] == Periodicity)
                {
                    isCurrentPeriodicitySupported = true;
                    break;
                }
            }

            if (!isCurrentPeriodicitySupported)
                Periodicity = supported[0];
        }

        private void ValidateNoiseDimension()
        {
            if (Noise == null)
                return;

            var supported = Noise.GetSupportedDimensions();
            bool isCurrentDimensionSupported = false;
            for (int i = 0; i < supported.Length; i++)
            {
                if (supported[i] == Dimension)
                {
                    isCurrentDimensionSupported = true;
                    break;
                }
            }

            if (!isCurrentDimensionSupported)
                Dimension = supported[0];
        }

        private void UpdateSlots()
        {
            ValidateNoiseDimension();
            ValidateNoisePeriodicity();

            var idList = new List<int> { UvSlotId, OutputSlotId };

            if (Dimension == NoiseDimension.D2)
                AddSlot(new UVMaterialSlot(UvSlotId, kUvSlotName, kUvSlotName, UVChannel.UV0));
            else
                AddSlot(new DynamicVectorMaterialSlot(UvSlotId, kUvSlotName, kUvSlotName, SlotType.Input, Vector4.zero));

            AddSlot(new Vector1MaterialSlot(OutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, 0.0f));

            if (IsNoiseSupported())
            {
                if (m_noisePeriodicity == NoisePeriodicity.Periodic)
                {
                    AddSlot(new Vector1MaterialSlot(PeriodSlotId, kPeriodSlotName, kPeriodSlotName, SlotType.Input, 1.0f));
                    idList.Add(PeriodSlotId);
                }

                if (m_noiseCombine == NoiseCombine.Fractal || m_noiseCombine == NoiseCombine.Turbulence || m_noiseCombine == NoiseCombine.Ridge)
                {
                    AddSlot(new Vector1MaterialSlot(PersistenceSlotId, kPersistenceSlotName, kPersistenceSlotName, SlotType.Input, 0.5f));
                    idList.Add(PersistenceSlotId);
                    AddSlot(new Vector1MaterialSlot(LacunaritySlotId, kLacunaritySlotName, kLacunaritySlotName, SlotType.Input, 2.0f));
                    idList.Add(LacunaritySlotId);
                }

                if (m_noiseCombine == NoiseCombine.Ridge)
                {
                    AddSlot(new Vector1MaterialSlot(SharpnessSlotId, kSharpnessSlotName, kSharpnessSlotName, SlotType.Input, 1.0f));
                    idList.Add(SharpnessSlotId);
                }
            }


            RemoveSlotsNameNotMatching(idList, true);

        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            InitializeNoiseAccordingToType();
            UpdateSlots();
        }

        private bool IsNoiseSupported()
        {
            return Noise == null ? false : Noise.Support();
        }

        private string GetFunctionName()
        {
            bool isNoiseSupported = IsNoiseSupported();
            if (isNoiseSupported)
            {
                string functionName = "SGE_Noise";
                functionName += "_" + m_noiseType;
                functionName += "_" + m_noisePeriodicity;
                functionName += "_" + m_noiseCombine;
                functionName += "_" + m_noiseDimension;
                return functionName;
            }

            return "SGE_UnsupportedNoise";
        }

        // generate how a node will be called in code
        public void GenerateNodeCode(ShaderStringBuilder sb, GenerationMode generationMode)
        {
            bool isNoiseSupported = IsNoiseSupported();
            string inputValue = GetSlotValue(UvSlotId, generationMode);
            string outputValue = GetSlotValue(OutputSlotId, generationMode);

            sb.AppendLine("{0} {1};", FindOutputSlot<MaterialSlot>(OutputSlotId).concreteValueType.ToShaderString(), GetVariableNameForSlot(OutputSlotId));

            string functionCall = GetFunctionName() + "(" + inputValue + ", " + outputValue;

            if (isNoiseSupported)
            {
                if (m_noisePeriodicity == NoisePeriodicity.Periodic)
                {
                    string periodValue = GetSlotValue(PeriodSlotId, generationMode);
                    functionCall += ", " + periodValue;
                }

                if (m_noiseCombine == NoiseCombine.Fractal || m_noiseCombine == NoiseCombine.Turbulence || m_noiseCombine == NoiseCombine.Ridge)
                {
                    string persistenceValue = GetSlotValue(PersistenceSlotId, generationMode);
                    string lacunarityValue = GetSlotValue(LacunaritySlotId, generationMode);
                    functionCall += ", " + persistenceValue + ", " + lacunarityValue;
                }

                if (m_noiseCombine == NoiseCombine.Ridge)
                {
                    string sharpnessValue = GetSlotValue(SharpnessSlotId, generationMode);
                    functionCall += ", " + sharpnessValue;
                }
            }

            functionCall += ");";

            sb.AppendLine(functionCall);
        }

        // generate the node's function
        public void GenerateNodeFunction(FunctionRegistry registry, GenerationMode generationMode)
        {
            bool isNoiseSupported = IsNoiseSupported();
            string functionName = GetFunctionName();

            if (isNoiseSupported)
                Noise.RegisterFunctions(registry);

            registry.ProvideFunction(functionName, s =>
            {
                var uvSlot = FindInputSlot<MaterialSlot>(UvSlotId);
                var outputSlot = FindOutputSlot<MaterialSlot>(OutputSlotId);

                if (uvSlot == null)
                    throw new NullReferenceException("UvSlot null, how is it possible ?");
                if (outputSlot == null)
                    throw new NullReferenceException("outputSlot null, how is it possible ?");

                s.Append("void {0}({1} {2}, out {3} {4}",
                    functionName,
                    uvSlot.concreteValueType.ToShaderString(),
                    kUvSlotName,
                    outputSlot.concreteValueType.ToShaderString(),
                    kOutputSlotName);
                if (isNoiseSupported)
                {
                    if (m_noisePeriodicity == NoisePeriodicity.Periodic)
                    {
                        var periodSlot = FindInputSlot<MaterialSlot>(PeriodSlotId);

                        if (periodSlot == null)
                            throw new NullReferenceException("periodSlot null, how is it possible ?");

                        s.Append(", {0} {1}",
                            periodSlot.concreteValueType.ToShaderString(),
                            kPeriodSlotName);
                    }
                    if (m_noiseCombine == NoiseCombine.Fractal || m_noiseCombine == NoiseCombine.Turbulence || m_noiseCombine == NoiseCombine.Ridge)
                    {
                        var persistenceSlot = FindInputSlot<MaterialSlot>(PersistenceSlotId);
                        var lacunaritySlot = FindInputSlot<MaterialSlot>(LacunaritySlotId);

                        if (persistenceSlot == null)
                            throw new NullReferenceException("persistenceSlot null, how is it possible ?");
                        if (lacunaritySlot == null)
                            throw new NullReferenceException("lacunaritySlot null, how is it possible ?");

                        s.Append(", {0} {1}, {2} {3}",
                            persistenceSlot.concreteValueType.ToShaderString(),
                            kPersistenceSlotName,
                            lacunaritySlot.concreteValueType.ToShaderString(),
                            kLacunaritySlotName);
                    }

                    if (m_noiseCombine == NoiseCombine.Ridge)
                    {
                        var sharpnessSlot = FindInputSlot<MaterialSlot>(SharpnessSlotId);

                        if (sharpnessSlot == null)
                            throw new NullReferenceException("sharpnessSlot null, how is it possible ?");

                        s.Append(", {0} {1}",
                            sharpnessSlot.concreteValueType.ToShaderString(),
                            kSharpnessSlotName);
                    }
                }

                s.AppendLine(")");

                using (s.BlockScope())
                {
                    if (isNoiseSupported)
                    {
                        s.Append("{0} = {1}({2}", kOutputSlotName, Noise.GetNoiseFunctionName(), kUvSlotName);

                        if (m_noisePeriodicity == NoisePeriodicity.Periodic)
                        {
                            s.Append(", pow(2, floor({0}))", kPeriodSlotName);
                        }

                        if (m_noiseCombine == NoiseCombine.Fractal || m_noiseCombine == NoiseCombine.Turbulence || m_noiseCombine == NoiseCombine.Ridge)
                        {
                            s.Append(", {0}, {1}", kPersistenceSlotName, kLacunaritySlotName);
                        }

                        if (m_noiseCombine == NoiseCombine.Ridge)
                        {
                            s.Append(", {0}", kSharpnessSlotName);
                        }

                        s.Append(")");

                        if (Noise.NeedRemapTo0_1())
                            s.Append(" * 0.5 + 0.5");
                        s.AppendLine(";");
                    }
                    else
                    {
                        s.AppendLine("{0} = {1};", kOutputSlotName, kUvSlotName);
                    }
                }
            });
        }

        public bool RequiresMeshUV(UVChannel channel, ShaderStageCapability stageCapability)
        {
            if (Dimension != NoiseDimension.D2)
                return false;

            using (var tempSlots = PooledList<MaterialSlot>.Get())
            {
                GetInputSlots(tempSlots);
                foreach (var slot in tempSlots)
                {
                    if (slot.RequiresMeshUV(channel))
                        return true;
                }

                return false;
            }
        }
    }
}