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
using System.Reflection;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.UIElements;

namespace ShaderGraphEssentials
{
    [AttributeUsage(AttributeTargets.Property)]
    class LabelControlAttribute : Attribute, IControlAttribute
    {
        string m_Label;

        public LabelControlAttribute(string label = null)
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(AbstractMaterialNode node, PropertyInfo propertyInfo)
        {
            return new LabelControlView(m_Label, node, propertyInfo);
        }
    }

    class LabelControlView : VisualElement
    {
        AbstractMaterialNode m_Node;
        PropertyInfo m_PropertyInfo;

        public LabelControlView(string label, AbstractMaterialNode node, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(string))
                throw new ArgumentException("Property must be of type string", nameof(propertyInfo));
            m_Node = node;
            m_PropertyInfo = propertyInfo;
            label = label ?? propertyInfo.Name;

            var labelText = (string) m_PropertyInfo.GetValue(m_Node, null);

            if (string.IsNullOrEmpty(label))
                Add(new Label { text = labelText });
            else
                Add(new Label { text = label + ": " + labelText });
        }
    }
}