using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor.Experimental.SSXMLInternal;
using UnityEngine;

namespace UnityEditor.Experimental
{
    public class StyleSheet
    {
        public enum GUILayoutType
        {
            [XmlEnum("")] None,
            [XmlEnum("horizontal")] Horizontal,
            [XmlEnum("vertical")] Vertical
        }

        public enum Alignment
        {
            [XmlEnum("")] None,
            [XmlEnum("horizontal-center")] HorizontalCenter,
            [XmlEnum("vertical-center")] VerticalCenter,
            [XmlEnum("both-center")] BothCenter
        }

        public struct Style
        {
            public struct GUIStyleDescription
            {
                public struct State
                {
                    public float? textColorR;
                    public float? textColorG;
                    public float? textColorB;
                    public float? textColorA;
                }

                public TextAnchor? alignment;
                public ImagePosition? imagePosition;
                public bool? wordWrap;

                public float? fixedWidth;
                public float? fixedHeight;
                public bool? stretchWidth;
                public bool? stretchHeight;

                public int? fontSize;
                public FontStyle? fontStyle;
                public bool? richText;

                public float? contentOffsetX;
                public float? contentOffsetY;

                public int? borderTop;
                public int? borderRight;
                public int? borderBottom;
                public int? borderLeft;
                public int? marginTop;
                public int? marginRight;
                public int? marginBottom;
                public int? marginLeft;
                public int? paddingTop;
                public int? paddingRight;
                public int? paddingBottom;
                public int? paddingLeft;
                public int? overflowTop;
                public int? overflowRight;
                public int? overflowBottom;
                public int? overflowLeft;

                public State normal;
                public State hover;
                public State active;
                public State onNormal;
                public State onHover;
                public State onActive;
                public State focused;
                public State onFocused;

            }

            public bool? expandWidth;
            public bool? expandHeight;
            public float? width;
            public float? height;
            public float? minWidth;
            public float? minHeight;
            public float? maxWidth;
            public float? maxHeight;

            public GUILayoutType layout;
            public Alignment alignment;
            public string guiStyleName;
            public GUIStyleDescription guiStyleDescription;

            GUIStyle m_GUIStyle;
            public GUIStyle guiStyle { get { return m_GUIStyle ?? (m_GUIStyle = BuildGUIStyle(guiStyleName, guiStyleDescription)); } }

            static GUIStyle BuildGUIStyle(string guiStyleName, GUIStyleDescription description)
            {
                var style = string.IsNullOrEmpty(guiStyleName) ? new GUIStyle() : new GUIStyle(guiStyleName);

                style.imagePosition = description.imagePosition ?? style.imagePosition;
                style.alignment = description.alignment ?? style.alignment;
                style.wordWrap = description.wordWrap ?? style.wordWrap;

                style.fixedWidth = description.fixedWidth ?? style.fixedWidth;
                style.fixedHeight = description.fixedHeight ?? style.fixedHeight;
                style.stretchWidth = description.stretchWidth ?? style.stretchWidth;
                style.stretchHeight = description.stretchHeight ?? style.stretchHeight;

                style.fontSize = description.fontSize ?? style.fontSize;
                style.fontStyle = description.fontStyle ?? style.fontStyle;
                style.richText = description.richText ?? style.richText;

                style.contentOffset = new Vector2(
                    description.contentOffsetX ?? style.contentOffset.x,
                    description.contentOffsetY ?? style.contentOffset.y);

                style.border = new RectOffset(
                    description.borderLeft ?? style.border.left,
                    description.borderRight ?? style.border.right,
                    description.borderTop ?? style.border.top, 
                    description.borderBottom ?? style.border.bottom);
                style.margin = new RectOffset(
                    description.marginLeft ?? style.margin.left,
                    description.marginRight ?? style.margin.right,
                    description.marginTop ?? style.margin.top,
                    description.marginBottom ?? style.margin.bottom);
                style.padding = new RectOffset(
                    description.paddingLeft ?? style.padding.left,
                    description.paddingRight ?? style.padding.right,
                    description.paddingTop ?? style.padding.top,
                    description.paddingBottom ?? style.padding.bottom);
                style.overflow = new RectOffset(
                    description.overflowLeft ?? style.overflow.left,
                    description.overflowRight ?? style.overflow.right,
                    description.overflowTop ?? style.overflow.top,
                    description.overflowBottom ?? style.overflow.bottom);

                style.normal.textColor = new Color(
                     description.normal.textColorR ?? style.normal.textColor.r,
                     description.normal.textColorG ?? style.normal.textColor.g,
                     description.normal.textColorB ?? style.normal.textColor.b,
                     description.normal.textColorA ?? style.normal.textColor.a);
                style.hover.textColor = new Color(
                     description.hover.textColorR ?? style.hover.textColor.r,
                     description.hover.textColorG ?? style.hover.textColor.g,
                     description.hover.textColorB ?? style.hover.textColor.b,
                     description.hover.textColorA ?? style.hover.textColor.a);
                style.active.textColor = new Color(
                     description.active.textColorR ?? style.active.textColor.r,
                     description.active.textColorG ?? style.active.textColor.g,
                     description.active.textColorB ?? style.active.textColor.b,
                     description.active.textColorA ?? style.active.textColor.a);
                style.onNormal.textColor = new Color(
                     description.onNormal.textColorR ?? style.onNormal.textColor.r,
                     description.onNormal.textColorG ?? style.onNormal.textColor.g,
                     description.onNormal.textColorB ?? style.onNormal.textColor.b,
                     description.onNormal.textColorA ?? style.onNormal.textColor.a);
                style.onHover.textColor = new Color(
                     description.onHover.textColorR ?? style.onHover.textColor.r,
                     description.onHover.textColorG ?? style.onHover.textColor.g,
                     description.onHover.textColorB ?? style.onHover.textColor.b,
                     description.onHover.textColorA ?? style.onHover.textColor.a);
                style.onActive.textColor = new Color(
                     description.onActive.textColorR ?? style.onActive.textColor.r,
                     description.onActive.textColorG ?? style.onActive.textColor.g,
                     description.onActive.textColorB ?? style.onActive.textColor.b,
                     description.onActive.textColorA ?? style.onActive.textColor.a);
                style.focused.textColor = new Color(
                     description.focused.textColorR ?? style.focused.textColor.r,
                     description.focused.textColorG ?? style.focused.textColor.g,
                     description.focused.textColorB ?? style.focused.textColor.b,
                     description.focused.textColorA ?? style.focused.textColor.a);
                style.onFocused.textColor = new Color(
                     description.onFocused.textColorR ?? style.onFocused.textColor.r,
                     description.onFocused.textColorG ?? style.onFocused.textColor.g,
                     description.onFocused.textColorB ?? style.onFocused.textColor.b,
                     description.onFocused.textColorA ?? style.onFocused.textColor.a);

                return style;
            }

            public void InheritFrom(Style other)
            {
                expandWidth = other.expandWidth ?? expandWidth;
                expandHeight = other.expandHeight ?? expandHeight;
                width = other.width ?? width;
                height = other.height ?? height;
                minWidth = other.minWidth ?? minWidth;
                minHeight = other.minHeight ?? minHeight;
                maxWidth = other.maxWidth ?? maxWidth;
                maxHeight = other.maxHeight ?? maxHeight;

                if (other.layout != GUILayoutType.None)
                    layout = other.layout;

                if (other.alignment != Alignment.None)
                    alignment = other.alignment;

                if (!string.IsNullOrEmpty(other.guiStyleName))
                    guiStyleName = other.guiStyleName;

                guiStyleDescription.imagePosition = other.guiStyleDescription.imagePosition ?? guiStyleDescription.imagePosition;
                guiStyleDescription.alignment = other.guiStyleDescription.alignment ?? guiStyleDescription.alignment;
                guiStyleDescription.wordWrap = other.guiStyleDescription.wordWrap ?? guiStyleDescription.wordWrap;
                guiStyleDescription.fixedWidth = other.guiStyleDescription.fixedWidth ?? guiStyleDescription.fixedWidth;
                guiStyleDescription.fixedHeight = other.guiStyleDescription.fixedHeight ?? guiStyleDescription.fixedHeight;
                guiStyleDescription.stretchWidth = other.guiStyleDescription.stretchWidth ?? guiStyleDescription.stretchWidth;
                guiStyleDescription.stretchHeight = other.guiStyleDescription.stretchHeight ?? guiStyleDescription.stretchHeight;
                guiStyleDescription.fontSize = other.guiStyleDescription.fontSize ?? guiStyleDescription.fontSize;
                guiStyleDescription.fontStyle = other.guiStyleDescription.fontStyle ?? guiStyleDescription.fontStyle;
                guiStyleDescription.richText = other.guiStyleDescription.richText ?? guiStyleDescription.richText;
                guiStyleDescription.contentOffsetX = other.guiStyleDescription.contentOffsetX ?? guiStyleDescription.contentOffsetX;
                guiStyleDescription.contentOffsetY = other.guiStyleDescription.contentOffsetY ?? guiStyleDescription.contentOffsetY;
                guiStyleDescription.borderLeft = other.guiStyleDescription.borderLeft ?? guiStyleDescription.borderLeft;
                guiStyleDescription.borderRight = other.guiStyleDescription.borderRight ?? guiStyleDescription.borderRight;
                guiStyleDescription.borderTop = other.guiStyleDescription.borderTop ?? guiStyleDescription.borderTop;
                guiStyleDescription.borderBottom = other.guiStyleDescription.borderBottom ?? guiStyleDescription.borderBottom;
                guiStyleDescription.marginLeft = other.guiStyleDescription.marginLeft ?? guiStyleDescription.marginLeft;
                guiStyleDescription.marginRight = other.guiStyleDescription.marginRight ?? guiStyleDescription.marginRight;
                guiStyleDescription.marginTop = other.guiStyleDescription.marginTop ?? guiStyleDescription.marginTop;
                guiStyleDescription.marginBottom = other.guiStyleDescription.marginBottom ?? guiStyleDescription.marginBottom;
                guiStyleDescription.paddingLeft = other.guiStyleDescription.paddingLeft ?? guiStyleDescription.paddingLeft;
                guiStyleDescription.paddingRight = other.guiStyleDescription.paddingRight ?? guiStyleDescription.paddingRight;
                guiStyleDescription.paddingTop = other.guiStyleDescription.paddingTop ?? guiStyleDescription.paddingTop;
                guiStyleDescription.paddingBottom = other.guiStyleDescription.paddingBottom ?? guiStyleDescription.paddingBottom;
                guiStyleDescription.overflowLeft = other.guiStyleDescription.overflowLeft ?? guiStyleDescription.overflowLeft;
                guiStyleDescription.overflowRight = other.guiStyleDescription.overflowRight ?? guiStyleDescription.overflowRight;
                guiStyleDescription.overflowTop = other.guiStyleDescription.overflowTop ?? guiStyleDescription.overflowTop;
                guiStyleDescription.overflowBottom = other.guiStyleDescription.overflowBottom ?? guiStyleDescription.overflowBottom;
                guiStyleDescription.normal.textColorR = other.guiStyleDescription.normal.textColorR ?? guiStyleDescription.normal.textColorR;
                guiStyleDescription.normal.textColorG = other.guiStyleDescription.normal.textColorG ?? guiStyleDescription.normal.textColorG;
                guiStyleDescription.normal.textColorB = other.guiStyleDescription.normal.textColorB ?? guiStyleDescription.normal.textColorB;
                guiStyleDescription.normal.textColorA = other.guiStyleDescription.normal.textColorA ?? guiStyleDescription.normal.textColorA;
                guiStyleDescription.hover.textColorR = other.guiStyleDescription.hover.textColorR ?? guiStyleDescription.hover.textColorR;
                guiStyleDescription.hover.textColorG = other.guiStyleDescription.hover.textColorG ?? guiStyleDescription.hover.textColorG;
                guiStyleDescription.hover.textColorB = other.guiStyleDescription.hover.textColorB ?? guiStyleDescription.hover.textColorB;
                guiStyleDescription.hover.textColorA = other.guiStyleDescription.hover.textColorA ?? guiStyleDescription.hover.textColorA;
                guiStyleDescription.active.textColorR = other.guiStyleDescription.active.textColorR ?? guiStyleDescription.active.textColorR;
                guiStyleDescription.active.textColorG = other.guiStyleDescription.active.textColorG ?? guiStyleDescription.active.textColorG;
                guiStyleDescription.active.textColorB = other.guiStyleDescription.active.textColorB ?? guiStyleDescription.active.textColorB;
                guiStyleDescription.active.textColorA = other.guiStyleDescription.active.textColorA ?? guiStyleDescription.active.textColorA;
                guiStyleDescription.onNormal.textColorR = other.guiStyleDescription.onNormal.textColorR ?? guiStyleDescription.onNormal.textColorR;
                guiStyleDescription.onNormal.textColorG = other.guiStyleDescription.onNormal.textColorG ?? guiStyleDescription.onNormal.textColorG;
                guiStyleDescription.onNormal.textColorB = other.guiStyleDescription.onNormal.textColorB ?? guiStyleDescription.onNormal.textColorB;
                guiStyleDescription.onNormal.textColorA = other.guiStyleDescription.onNormal.textColorA ?? guiStyleDescription.onNormal.textColorA;
                guiStyleDescription.onHover.textColorR = other.guiStyleDescription.onHover.textColorR ?? guiStyleDescription.onHover.textColorR;
                guiStyleDescription.onHover.textColorG = other.guiStyleDescription.onHover.textColorG ?? guiStyleDescription.onHover.textColorG;
                guiStyleDescription.onHover.textColorB = other.guiStyleDescription.onHover.textColorB ?? guiStyleDescription.onHover.textColorB;
                guiStyleDescription.onHover.textColorA = other.guiStyleDescription.onHover.textColorA ?? guiStyleDescription.onHover.textColorA;
                guiStyleDescription.onActive.textColorR = other.guiStyleDescription.onActive.textColorR ?? guiStyleDescription.onActive.textColorR;
                guiStyleDescription.onActive.textColorG = other.guiStyleDescription.onActive.textColorG ?? guiStyleDescription.onActive.textColorG;
                guiStyleDescription.onActive.textColorB = other.guiStyleDescription.onActive.textColorB ?? guiStyleDescription.onActive.textColorB;
                guiStyleDescription.onActive.textColorA = other.guiStyleDescription.onActive.textColorA ?? guiStyleDescription.onActive.textColorA;
                guiStyleDescription.focused.textColorR = other.guiStyleDescription.focused.textColorR ?? guiStyleDescription.focused.textColorR;
                guiStyleDescription.focused.textColorG = other.guiStyleDescription.focused.textColorG ?? guiStyleDescription.focused.textColorG;
                guiStyleDescription.focused.textColorB = other.guiStyleDescription.focused.textColorB ?? guiStyleDescription.focused.textColorB;
                guiStyleDescription.focused.textColorA = other.guiStyleDescription.focused.textColorA ?? guiStyleDescription.focused.textColorA;
                guiStyleDescription.onFocused.textColorR = other.guiStyleDescription.onFocused.textColorR ?? guiStyleDescription.onFocused.textColorR;
                guiStyleDescription.onFocused.textColorG = other.guiStyleDescription.onFocused.textColorG ?? guiStyleDescription.onFocused.textColorG;
                guiStyleDescription.onFocused.textColorB = other.guiStyleDescription.onFocused.textColorB ?? guiStyleDescription.onFocused.textColorB;
                guiStyleDescription.onFocused.textColorA = other.guiStyleDescription.onFocused.textColorA ?? guiStyleDescription.onFocused.textColorA;
            }

            public GUILayoutOption[] ToGUILayoutOptionArray(float? widthOverride, float? heightOverride)
            {
                width = widthOverride ?? width;
                height = heightOverride ?? height;

                var count = 0;
                if (width.HasValue)
                    ++count;
                else
                {
                    if (expandWidth.HasValue && expandWidth.Value)
                        ++count;
                    if (minWidth.HasValue)
                        ++count;
                    if (maxWidth.HasValue)
                        ++count;
                }

                if (height.HasValue)
                    ++count;
                else
                {
                    if (expandHeight.HasValue && expandHeight.Value)
                        ++count;
                    if (minHeight.HasValue)
                        ++count;
                    if (maxHeight.HasValue)
                        ++count;
                }

                var result = new GUILayoutOption[count];
                count = 0;

                if (width.HasValue)
                    result[count++] = GUILayout.Width(width.Value);
                else
                {
                    if (expandWidth.HasValue && expandWidth.Value)
                        result[count++] = GUILayout.ExpandWidth(true);
                    if (minWidth.HasValue)
                        result[count++] = GUILayout.MinWidth(minWidth.Value);
                    if (maxWidth.HasValue)
                        result[count++] = GUILayout.MaxWidth(maxWidth.Value);
                }

                if (height.HasValue)
                    ++count;
                else
                {
                    if (expandHeight.HasValue && expandHeight.Value)
                        result[count++] = GUILayout.ExpandHeight(true);
                    if (minHeight.HasValue)
                        result[count++] = GUILayout.MinHeight(minHeight.Value);
                    if (maxHeight.HasValue)
                        result[count++] = GUILayout.MaxHeight(maxHeight.Value);
                }

                return result;
            }
        }

        static Dictionary<Type, StyleSheet> s_Instances = new Dictionary<Type, StyleSheet>();
        public static StyleSheet GetStyleSheet<TType>()
            where TType : StyleSheet
        {
            if (!s_Instances.ContainsKey(typeof(TType)))
                s_Instances[typeof(TType)] = Activator.CreateInstance<TType>();

            return s_Instances[typeof(TType)];
        }

        Dictionary<string, Style> m_StylePerClass = new Dictionary<string, Style>();
        Dictionary<Type, Style> m_StylePerElement = new Dictionary<Type, Style>();

        public Style GetStyleFor(string[] classes, Type elementType)
        {
            var resolver = new Style();
            var hasClass = false;
            if (classes.Length > 0)
            {
                for (int i = 0; i < classes.Length; i++)
                {
                    Style layoutClassParam;
                    if (m_StylePerClass.TryGetValue(classes[i], out layoutClassParam))
                    {
                        resolver.InheritFrom(layoutClassParam);
                        hasClass = true;
                    }
                }
            }
            if (!hasClass)
            {
                Style layoutClassParam;
                if (m_StylePerElement.TryGetValue(elementType, out layoutClassParam))
                    resolver.InheritFrom(layoutClassParam);
            }

            return resolver;
        }

        protected StyleSheet SetClassStyle(string @class, Style style)
        {
            m_StylePerClass[@class] = style;
            return this;
        }

        protected StyleSheet SetClassStyle(Type elementType, Style style)
        {
            m_StylePerElement[elementType] = style;
            return this;
        }
    }
}
