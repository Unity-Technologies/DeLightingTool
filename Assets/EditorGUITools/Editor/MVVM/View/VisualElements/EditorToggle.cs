using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;

namespace UnityEditor.Experimental.VisualElements
{
    public class EditorToggle : Toggle
    {
        public static readonly DependencyProperty<bool> propertyValue = new DependencyProperty<EditorToggle, bool>(
            "value",
            elt => elt.value,
            (elt, v) => elt.value = v,
            v => (bool)v,
            new RoutedEvent<EditorToggle, bool>((el, c) => el.ValueChanged += c, (el, c) => el.ValueChanged-= c));

        public static readonly DependencyProperty<GUIContent> propertyLabel = new DependencyProperty<EditorToggle, GUIContent>(
            "label",
            elt => elt.label,
            (elt, v) => elt.label = v,
            v => (GUIContent)v);

        public static readonly DependencyProperty<string> propertyLabelText = new DependencyProperty<EditorToggle, string>(
            "labelText",
            elt => elt.labelText,
            (elt, v) => elt.labelText = v,
            v => (string)v);

        public static readonly DependencyProperty<string> propertyLabelTooltip = new DependencyProperty<EditorToggle, string>(
            "labelTooltip",
            elt => elt.labelTooltip,
            (elt, v) => elt.labelTooltip = v,
            v => (string)v);

        public static readonly DependencyProperty<Texture> propertyLabelImage = new DependencyProperty<EditorToggle, Texture>(
            "labelImage",
            elt => elt.labelImage,
            (elt, v) => elt.labelImage = v,
            v => (Texture)v);

        public override void OnGUI()
        {
            BeginAlignment();
            value = EditorGUILayout.Toggle(label, value, style.guiStyle, guiLayoutOptions);
            EndAlignment();
        }
    }
}
