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
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;


namespace ShaderGraphEssentials
{
    [Serializable]
    [Title("Master", "SGE Unlit")]
    class SGEUnlitMasterNode : MasterNode<ISGEUnlitSubShader>, IMayRequirePosition, IMayRequireNormal, IMayRequireTangent
    {
        public const string ColorSlotName = "Color";
        public const string AlphaSlotName = "Alpha";
        public const string AlphaClipThresholdSlotName = "AlphaClipThreshold";
        public const string PositionName = "Vertex Position";
        public const string NormalName = "Vertex Normal";
        public const string TangentName = "Vertex Tangent";

        public const int ColorSlotId = 0;
        public const int AlphaSlotId = 7;
        public const int AlphaThresholdSlotId = 8;
        public const int PositionSlotId = 9;
        public const int VertNormalSlotId = 10;
        public const int VertTangentSlotId = 11;
        
        [SerializeField]
        SurfaceMaterialTags.RenderType m_renderType;

        public SurfaceMaterialTags.RenderType RenderType
        {
            get { return m_renderType; }
            set
            {
                if (m_renderType == value)
                    return;

                m_renderType = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        SurfaceMaterialTags.RenderQueue m_renderQueue;

        public SurfaceMaterialTags.RenderQueue RenderQueue
        {
            get { return m_renderQueue; }
            set
            {
                if (m_renderQueue == value)
                    return;

                m_renderQueue = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        BlendMode m_blendMode;

        public BlendMode BlendMode
        {
            get { return m_blendMode; }
            set
            {
                if (m_blendMode == value)
                    return;

                m_blendMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        SurfaceMaterialOptions.CullMode m_cullMode;

        public SurfaceMaterialOptions.CullMode CullMode
        {
            get { return m_cullMode; }
            set
            {
                if (m_cullMode == value)
                    return;

                m_cullMode = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        SurfaceMaterialOptions.ZWrite m_zwrite;

        public SurfaceMaterialOptions.ZWrite ZWrite
        {
            get { return m_zwrite; }
            set
            {
                if (m_zwrite == value)
                    return;

                m_zwrite = value;
                Dirty(ModificationScope.Graph);
            }
        }

        [SerializeField]
        SurfaceMaterialOptions.ZTest m_ztest;

        public SurfaceMaterialOptions.ZTest ZTest
        {
            get { return m_ztest; }
            set
            {
                if (m_ztest == value)
                    return;

                m_ztest = value;
                Dirty(ModificationScope.Graph);
            }
        }
        
        [SerializeField]
        string m_customEditor;

        public string CustomEditor
        {
            get { return m_customEditor; }
            set
            {
                if (m_customEditor == value)
                    return;

                m_customEditor = value;
                Dirty(ModificationScope.Graph);
            }
        }

        public SGEUnlitMasterNode()
        {
            UpdateNodeAfterDeserialization();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            base.UpdateNodeAfterDeserialization();
            name = "SGE Unlit Master";
            AddSlot(new PositionMaterialSlot(PositionSlotId, PositionName, PositionName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            AddSlot(new NormalMaterialSlot(VertNormalSlotId, NormalName, NormalName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            AddSlot(new TangentMaterialSlot(VertTangentSlotId, TangentName, TangentName, CoordinateSpace.Object, ShaderStageCapability.Vertex));
            AddSlot(new ColorRGBMaterialSlot(ColorSlotId, ColorSlotName, ColorSlotName, SlotType.Input, Color.grey.gamma, ColorMode.Default, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(AlphaSlotId, AlphaSlotName, AlphaSlotName, SlotType.Input, 1, ShaderStageCapability.Fragment));
            AddSlot(new Vector1MaterialSlot(AlphaThresholdSlotId, AlphaClipThresholdSlotName, AlphaClipThresholdSlotName, SlotType.Input, 0.0f, ShaderStageCapability.Fragment));

            // clear out slot names that do not match the slots
            // we support
            RemoveSlotsNameNotMatching(
                new[]
                {
                    PositionSlotId,
                    VertNormalSlotId,
                    VertTangentSlotId,
                    ColorSlotId,
                    AlphaSlotId,
                    AlphaThresholdSlotId
                });
        }

        protected override VisualElement CreateCommonSettingsElement()
        {
            return new SGEUnlitSettingsView(this);
        }

        public NeededCoordinateSpace RequiresPosition(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequirePosition>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresPosition(stageCapability));
        }
        
        public NeededCoordinateSpace RequiresNormal(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireNormal>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresNormal(stageCapability));
        }

        public NeededCoordinateSpace RequiresTangent(ShaderStageCapability stageCapability)
        {
            List<MaterialSlot> slots = new List<MaterialSlot>();
            GetSlots(slots);

            List<MaterialSlot> validSlots = new List<MaterialSlot>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].stageCapability != ShaderStageCapability.All && slots[i].stageCapability != stageCapability)
                    continue;

                validSlots.Add(slots[i]);
            }
            return validSlots.OfType<IMayRequireTangent>().Aggregate(NeededCoordinateSpace.None, (mask, node) => mask | node.RequiresTangent(stageCapability));
        }


        public SurfaceMaterialOptions GetMaterialOptions()
        {
            SurfaceMaterialOptions options = new SurfaceMaterialOptions();

            switch (BlendMode)
            {
                case BlendMode.Off:
                    options.srcBlend = SurfaceMaterialOptions.BlendMode.One;
                    options.dstBlend = SurfaceMaterialOptions.BlendMode.Zero;
                    break;
                case BlendMode.Alpha:
                    options.srcBlend = SurfaceMaterialOptions.BlendMode.SrcAlpha;
                    options.dstBlend = SurfaceMaterialOptions.BlendMode.OneMinusSrcAlpha;
                    break;
                case BlendMode.Premultiply:
                    options.srcBlend = SurfaceMaterialOptions.BlendMode.One;
                    options.dstBlend = SurfaceMaterialOptions.BlendMode.OneMinusSrcAlpha;
                    break;
                case BlendMode.Additive:
                    options.srcBlend = SurfaceMaterialOptions.BlendMode.One;
                    options.dstBlend = SurfaceMaterialOptions.BlendMode.One;
                    break;
                case BlendMode.Multiply:
                    options.srcBlend = SurfaceMaterialOptions.BlendMode.DstColor;
                    options.dstBlend = SurfaceMaterialOptions.BlendMode.Zero;
                    break;
            }

            options.cullMode = CullMode;
            options.zWrite = ZWrite;
            options.zTest = ZTest;

            return options;
        }

        public SurfaceMaterialTags GetMaterialTags()
        {
            SurfaceMaterialTags tags = new SurfaceMaterialTags();

            tags.renderType = RenderType;
            tags.renderQueue = RenderQueue;

            return tags;
        }
    }
}
