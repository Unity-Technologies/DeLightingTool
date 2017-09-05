using System.Collections.Generic;
using System.Text;
using UnityEditor.Experimental.CodeGenerator;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.SSXMLInternal
{
    class SSXMLDOMVisitor : IAssetCodeGenerator<DOMDocument>
    {
        CSFile m_File = null;
        CSClass m_Class = null;

        static readonly DOMStyle[] s_BuiltinStyles = new[]
        {
            new DOMStyle() { name = "expand-width", expandWidth = true },
            new DOMStyle() { name = "expand-height", expandHeight = true }
        };

        public string GenerateFrom(string workingDirectory, string assetName, DOMDocument document)
        {
            m_File = new CSFile(document.@namespace)
                .AddWarningDisable(414)
                .AddInclude("UnityEngine")
                .AddInclude("UnityEditor")
                .AddInclude("UnityEngine.Experimental")
                .AddInclude("UnityEditor.Experimental");

            m_Class = new CSClass(Scope.Internal, assetName, CSClass.Modifier.Partial)
                .AddParent("StyleSheet");
            m_File.AddClass(m_Class);

            Visit(document.styles);

            var builder = new StringBuilder();
            m_File.GenerateIn(builder);
            return builder.ToString();
        }

        void Visit(DOMStyle[] styles)
        {
            if (styles == null)
                return;

            var constructor = new CSMethod(Scope.Public, string.Empty, m_Class.name);
            m_Class.AddMethod(constructor);

            var tokens = new List<string>();
            WriteStyles(0, tokens, s_BuiltinStyles, constructor, m_Class);
            WriteStyles(s_BuiltinStyles.Length, tokens, styles, constructor, m_Class);
        }

        static void WriteStyles(int counterIndex, List<string> tokens, DOMStyle[] styles, CSMethod constructor, CSClass @class)
        {
            for (int i = 0; i < styles.Length; i++)
            {
                tokens.Clear();
                var style = styles[i];

                ParseGUILayoutTokens(style, tokens);

                var buildMethod = new CSMethod(Scope.Private, "Style", "BuildStyle" + ++counterIndex);
                @class.AddMethod(buildMethod);

                if (tokens.Count > 0)
                {
                    if (!string.IsNullOrEmpty(style.name))
                        constructor.AddBodyLine(string.Format("SetClassStyle(\"{0}\", {1}());", style.name, buildMethod.name));
                    if (!string.IsNullOrEmpty(style.elementType))
                        constructor.AddBodyLine(string.Format("SetClassStyle(typeof({0}), {1}());", style.elementType, buildMethod.name));
                }

                buildMethod.AddBodyLine("var s = new Style();");
                for (int j = 0; j < tokens.Count; j++)
                    buildMethod.AddBodyLine(tokens[j] + ";");
                buildMethod.AddBodyLine("return s;");
            }
        }

        static void ParseGUILayoutTokens(DOMStyle style, List<string> tokens)
        {
            if (style.width >= 0)
                tokens.Add("s.width = " + style.width);
            else
            {
                if (style.expandWidth)
                    tokens.Add("s.expandWidth = true");
                if (style.minWidth >= 0)
                    tokens.Add("s.minWidth = " + style.minWidth);
                if (style.width >= 0)
                    tokens.Add("s.width = " + style.width);
                if (style.maxWidth >= 0)
                    tokens.Add("s.maxWidth = " + style.maxWidth);
            }

            if (style.height >= 0)
                tokens.Add("s.height = " + style.height);
            else
            {
                if (style.expandHeight)
                    tokens.Add("s.expandHeight = true");
                if (style.minHeight >= 0)
                    tokens.Add("s.minHeight = " + style.minHeight);
                if (style.height >= 0)
                    tokens.Add("s.height = " + style.height);
                if (style.maxHeight >= 0)
                    tokens.Add("s.maxHeight = " + style.maxHeight);
            }

            if (style.layout != StyleSheet.GUILayoutType.None)
                tokens.Add("s.layout = GUILayoutType." + style.layout);

            if (style.alignment != StyleSheet.Alignment.None)
                tokens.Add("s.alignment = Alignment." + style.alignment);

            if (!string.IsNullOrEmpty(style.guiStyle))
                tokens.Add("s.guiStyleName = \"" + style.guiStyle + "\"");

            if (style.guiStyleOverrides != null)
            {
                var gs = style.guiStyleOverrides;
                tokens.Add("s.guiStyleDescription.imagePosition = ImagePosition." + gs.imagePosition);
                tokens.Add("s.guiStyleDescription.alignment = TextAnchor." + gs.alignment);
                tokens.Add("s.guiStyleDescription.wordWrap = " + gs.wordWrap.ToString().ToLower());
                tokens.Add("s.guiStyleDescription.stretchWidth = " + gs.stretchWidth.ToString().ToLower());
                tokens.Add("s.guiStyleDescription.stretchHeight = " + gs.stretchHeight.ToString().ToLower());
                tokens.Add("s.guiStyleDescription.fontStyle = FontStyle." + gs.fontStyle);
                tokens.Add("s.guiStyleDescription.richText = " + gs.richText.ToString().ToLower());

                if (gs.fixedWidth != -1)
                    tokens.Add("s.guiStyleDescription.fixedWidth = " + gs.fixedWidth);
                if (gs.fixedHeight != -1)
                    tokens.Add("s.guiStyleDescription.fixedHeight = " + gs.fixedHeight);
                if (gs.fontSize != -1)
                    tokens.Add("s.guiStyleDescription.fontSize = " + gs.fontSize);

                if (gs.contentOffsetX != float.MinValue)
                    tokens.Add("s.guiStyleDescription.contentOffsetX = " + gs.contentOffsetX + "f");
                if (gs.contentOffsetY != float.MinValue)
                        tokens.Add("s.guiStyleDescription.contentOffsetY = " + gs.contentOffsetY + "f");
                if (gs.borderTop != int.MinValue)
                        tokens.Add("s.guiStyleDescription.borderTop = " + gs.borderTop);
                if (gs.borderRight != int.MinValue)
                        tokens.Add("s.guiStyleDescription.borderRight = " + gs.borderRight);
                if (gs.borderBottom != int.MinValue)
                        tokens.Add("s.guiStyleDescription.borderBottom = " + gs.borderBottom);
                if (gs.borderLeft != int.MinValue)
                        tokens.Add("s.guiStyleDescription.borderLeft = " + gs.borderLeft);
                if (gs.marginTop != int.MinValue)
                        tokens.Add("s.guiStyleDescription.marginTop = " + gs.marginTop);
                if (gs.marginRight != int.MinValue)
                        tokens.Add("s.guiStyleDescription.marginRight = " + gs.marginRight);
                if (gs.marginBottom != int.MinValue)
                        tokens.Add("s.guiStyleDescription.marginBottom = " + gs.marginBottom);
                if (gs.marginLeft != int.MinValue)
                        tokens.Add("s.guiStyleDescription.marginLeft = " + gs.marginLeft);
                if (gs.paddingTop != int.MinValue)
                        tokens.Add("s.guiStyleDescription.paddingTop = " + gs.paddingTop);
                if (gs.paddingRight != int.MinValue)
                        tokens.Add("s.guiStyleDescription.paddingRight = " + gs.paddingRight);
                if (gs.paddingBottom != int.MinValue)
                        tokens.Add("s.guiStyleDescription.paddingBottom = " + gs.paddingBottom);
                if (gs.paddingLeft != int.MinValue)
                        tokens.Add("s.guiStyleDescription.paddingLeft = " + gs.paddingLeft);
                if (gs.overflowTop != int.MinValue)
                        tokens.Add("s.guiStyleDescription.overflowTop = " + gs.overflowTop);
                if (gs.overflowRight != int.MinValue)
                        tokens.Add("s.guiStyleDescription.overflowRight = " + gs.overflowRight);
                if (gs.overflowBottom != int.MinValue)
                        tokens.Add("s.guiStyleDescription.overflowBottom = " + gs.overflowBottom);
                if (gs.overflowLeft != int.MinValue)
                        tokens.Add("s.guiStyleDescription.overflowLeft = " + gs.overflowLeft);

                if (gs.normal != null)
                {
                    if (gs.normal.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.normal.textColorR = " + gs.normal.textColorR + "f");
                    if (gs.normal.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.normal.textColorG = " + gs.normal.textColorG + "f");
                    if (gs.normal.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.normal.textColorB = " + gs.normal.textColorB + "f");
                    if (gs.normal.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.normal.textColorA = " + gs.normal.textColorA + "f");
                }

                if (gs.hover != null)
                {
                    if (gs.hover.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.hover.textColorR = " + gs.hover.textColorR + "f");
                    if (gs.hover.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.hover.textColorG = " + gs.hover.textColorG + "f");
                    if (gs.hover.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.hover.textColorB = " + gs.hover.textColorB + "f");
                    if (gs.hover.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.hover.textColorA = " + gs.hover.textColorA + "f");
                }

                if (gs.active != null)
                {
                    if (gs.active.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.active.textColorR = " + gs.active.textColorR + "f");
                    if (gs.active.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.active.textColorG = " + gs.active.textColorG + "f");
                    if (gs.active.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.active.textColorB = " + gs.active.textColorB + "f");
                    if (gs.active.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.active.textColorA = " + gs.active.textColorA + "f");
                }

                if (gs.onNormal != null)
                {
                    if (gs.onNormal.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.onNormal.textColorR = " + gs.onNormal.textColorR + "f");
                    if (gs.onNormal.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.onNormal.textColorG = " + gs.onNormal.textColorG + "f");
                    if (gs.onNormal.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.onNormal.textColorB = " + gs.onNormal.textColorB + "f");
                    if (gs.onNormal.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.onNormal.textColorA = " + gs.onNormal.textColorA + "f");
                }

                if (gs.onHover != null)
                {
                    if (gs.onHover.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.onHover.textColorR = " + gs.onHover.textColorR + "f");
                    if (gs.onHover.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.onHover.textColorG = " + gs.onHover.textColorG + "f");
                    if (gs.onHover.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.onHover.textColorB = " + gs.onHover.textColorB + "f");
                    if (gs.onHover.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.onHover.textColorA = " + gs.onHover.textColorA + "f");
                }

                if (gs.onActive != null)
                {
                    if (gs.onActive.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.onActive.textColorR = " + gs.onActive.textColorR + "f");
                    if (gs.onActive.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.onActive.textColorG = " + gs.onActive.textColorG + "f");
                    if (gs.onActive.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.onActive.textColorB = " + gs.onActive.textColorB + "f");
                    if (gs.onActive.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.onActive.textColorA = " + gs.onActive.textColorA + "f");
                }

                if (gs.focused != null)
                {
                    if (gs.focused.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.focused.textColorR = " + gs.focused.textColorR + "f");
                    if (gs.focused.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.focused.textColorG = " + gs.focused.textColorG + "f");
                    if (gs.focused.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.focused.textColorB = " + gs.focused.textColorB + "f");
                    if (gs.focused.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.focused.textColorA = " + gs.focused.textColorA + "f");
                }

                if (gs.onFocused != null)
                {
                    if (gs.onFocused.textColorR >= 0)
                        tokens.Add("s.guiStyleDescription.onFocused.textColorR = " + gs.onFocused.textColorR + "f");
                    if (gs.onFocused.textColorG >= 0)
                        tokens.Add("s.guiStyleDescription.onFocused.textColorG = " + gs.onFocused.textColorG + "f");
                    if (gs.onFocused.textColorB >= 0)
                        tokens.Add("s.guiStyleDescription.onFocused.textColorB = " + gs.onFocused.textColorB + "f");
                    if (gs.onFocused.textColorA >= 0)
                        tokens.Add("s.guiStyleDescription.onFocused.textColorA = " + gs.onFocused.textColorA + "f");
                }
            }
        }
    }
}
