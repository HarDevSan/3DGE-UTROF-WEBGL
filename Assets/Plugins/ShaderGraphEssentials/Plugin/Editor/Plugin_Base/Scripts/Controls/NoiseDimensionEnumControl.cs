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
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace ShaderGraphEssentials
{
    [AttributeUsage(AttributeTargets.Property)]
    class NoiseDimensionEnumControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;
        int m_SlotId;

        public NoiseDimensionEnumControlAttribute(string label = null, int slotId = 0)
        {
            m_Label = label;
            m_SlotId = slotId;
        }

        public VisualElement InstantiateControl(AbstractMaterialNode node, PropertyInfo propertyInfo)
        {
            return new NoiseDimensionEnumControlView(m_Label, m_SlotId, node, propertyInfo);
        }
    }

    class NoiseDimensionEnumControlView : VisualElement, AbstractMaterialNodeModificationListener
    {
        NoiseNode m_Node;
        PropertyInfo m_PropertyInfo;
        int m_SlotId;

        PopupField<string> m_PopupField;
        string[] m_ValueNames;

        NoiseDimension[] m_previousNoiseDimensions = new NoiseDimension[0];

        public NoiseDimensionEnumControlView(string label, int slotId, AbstractMaterialNode node, PropertyInfo propertyInfo)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("Styles/Controls/NoiseDimensionEnumControlView"));
            m_PropertyInfo = propertyInfo;
            m_SlotId = slotId;
            if (!propertyInfo.PropertyType.IsEnum)
                throw new ArgumentException("Property must be an enum.", "propertyInfo");
            NoiseNode noiseNode = node as NoiseNode;
            if (noiseNode == null)
                throw new ArgumentException("NoiseDimensionEnumControl can only be applied on NoiseNode.", "node");
            m_Node = noiseNode;

            Add(new Label(label ?? ObjectNames.NicifyVariableName(propertyInfo.Name)));

            m_ValueNames = NoiseDimensionUtils.GetDisplayNames();

            CreatePopup();
        }

        void OnValueChanged(ChangeEvent<string> evt)
        {
            var index = m_PopupField.index;
            var value = (int)m_PropertyInfo.GetValue(m_Node, null);
            if (!index.Equals(value))
            {
                m_Node.owner.owner.RegisterCompleteObjectUndo("Change " + m_Node.name);
                m_PropertyInfo.SetValue(m_Node, index, null);
            }

            CreatePopup();
        }

        public void OnNodeModified(ModificationScope scope)
        {
            CreatePopup();
            m_PopupField.MarkDirtyRepaint();
        }

        void CreatePopup()
        {
            var supportedDimensions = m_Node.Noise.GetSupportedDimensions();

            if (m_PopupField != null)
            {
                if (supportedDimensions.Length == m_previousNoiseDimensions.Length)
                {
                    bool identical = true;
                    for (int i = 0; i < supportedDimensions.Length; i++)
                    {
                        if (supportedDimensions[i] == m_previousNoiseDimensions[i])
                            continue;
                        identical = false;
                        break;
                    }

                    if (identical)
                        return;
                }

                Remove(m_PopupField);
            }

            m_previousNoiseDimensions = supportedDimensions;
            List<string> popupEntries = new List<string>();
            for (int i = 0; i < m_previousNoiseDimensions.Length; i++)
                popupEntries.Add(m_ValueNames[i]);

            var value = (int)m_PropertyInfo.GetValue(m_Node, null);
            if (value >= m_previousNoiseDimensions.Length)
                value = 0;

            m_PopupField = new PopupField<string>(popupEntries, value);
            m_PopupField.RegisterValueChangedCallback(OnValueChanged);
            Add(m_PopupField);
        }
    }
}