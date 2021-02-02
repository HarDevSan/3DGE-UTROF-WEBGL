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
using UnityEditor.Experimental.UIElements;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace ShaderGraphEssentials
{
    class SGESimpleLitSettingsView : VisualElement
    {
        SGESimpleLitMasterNode m_Node;
        public SGESimpleLitSettingsView(SGESimpleLitMasterNode node)
        {
            m_Node = node;

            PropertySheet ps = new PropertySheet();

            ps.Add(new PropertyRow(new Label("Render Type")), (row) =>
            {
                row.Add(new EnumField(SurfaceMaterialTags.RenderType.Opaque), (field) =>
                {
                    field.value = m_Node.RenderType;
                    field.RegisterValueChangedCallback(ChangeRenderType);
                });
            });

            ps.Add(new PropertyRow(new Label("Render Queue")), (row) =>
            {
                row.Add(new EnumField(SurfaceMaterialTags.RenderQueue.Geometry), (field) =>
                {
                    field.value = m_Node.RenderQueue;
                    field.RegisterValueChangedCallback(ChangeRenderQueue);
                });
            });

            ps.Add(new PropertyRow(new Label("Blend")), (row) =>
            {
                row.Add(new EnumField(BlendMode.Off), (field) =>
                {
                    field.value = m_Node.BlendMode;
                    field.RegisterValueChangedCallback(ChangeBlendMode);
                });
            });

            ps.Add(new PropertyRow(new Label("Cull")), (row) =>
            {
                row.Add(new EnumField(SurfaceMaterialOptions.CullMode.Back), (field) =>
                {
                    field.value = m_Node.CullMode;
                    field.RegisterValueChangedCallback(ChangeCullMode);
                });
            });

            ps.Add(new PropertyRow(new Label("ZWrite")), (row) =>
            {
                row.Add(new EnumField(SurfaceMaterialOptions.ZWrite.On), (field) =>
                {
                    field.value = m_Node.ZWrite;
                    field.RegisterValueChangedCallback(ChangeZWrite);
                });
            });

            ps.Add(new PropertyRow(new Label("ZTest")), (row) =>
            {
                row.Add(new EnumField(SurfaceMaterialOptions.ZTest.LEqual), (field) =>
                {
                    field.value = m_Node.ZTest;
                    field.RegisterValueChangedCallback(ChangeZTest);
                });
            });
            
            ps.Add(new PropertyRow(new Label("Fragment Normal Space")), (row) =>
            {
                row.Add(new EnumField(NormalDropOffSpace.Tangent), (field) =>
                {
                    field.value = m_Node.normalDropOffSpace;
                    field.RegisterValueChangedCallback(ChangeSpaceOfNormalDropOffMode);
                });
            });

            
            ps.Add(new PropertyRow(new Label("Custom Editor")), (row) =>
            {
                row.Add(new TextField(String.Empty), (field) =>
                {
                    field.value = m_Node.CustomEditor;
                    field.RegisterValueChangedCallback(ChangeCustomEditor);
                });
            });

            Add(ps);
        }

        void ChangeRenderType(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.RenderType, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("RenderType Change");
            m_Node.RenderType = (SurfaceMaterialTags.RenderType)evt.newValue;
        }

        void ChangeRenderQueue(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.RenderQueue, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("RenderQueue Change");
            m_Node.RenderQueue = (SurfaceMaterialTags.RenderQueue)evt.newValue;
        }

        void ChangeBlendMode(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.BlendMode, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("BlendMode Change");
            m_Node.BlendMode = (BlendMode)evt.newValue;
        }

        void ChangeCullMode(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.CullMode, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("CullMode Change");
            m_Node.CullMode = (SurfaceMaterialOptions.CullMode)evt.newValue;
        }

        void ChangeZWrite(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.ZWrite, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("ZWrite Change");
            m_Node.ZWrite = (SurfaceMaterialOptions.ZWrite)evt.newValue;
        }

        void ChangeZTest(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.ZTest, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("ZTest Change");
            m_Node.ZTest = (SurfaceMaterialOptions.ZTest)evt.newValue;
        }
        
        void ChangeSpaceOfNormalDropOffMode(ChangeEvent<Enum> evt)
        {
            if (Equals(m_Node.normalDropOffSpace, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Normal Space Drop-Off Mode Change");
            m_Node.normalDropOffSpace = (NormalDropOffSpace)evt.newValue;
        }

        
        void ChangeCustomEditor(ChangeEvent<string> evt)
        {
            if (Equals(m_Node.CustomEditor, evt.newValue))
                return;

            m_Node.owner.owner.RegisterCompleteObjectUndo("Custom Editor Change");
            m_Node.CustomEditor = evt.newValue;
        }
    }
}