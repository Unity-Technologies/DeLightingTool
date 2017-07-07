using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    public partial class VXMLDOMElementVisitor : DynamicVisitor<DOMElement>
    {
        static readonly Regex s_SlugifyTagRegex = new Regex("[^\\w\\d_]+");
        static readonly Regex s_ParameterRegex = new Regex("\\{([\\w\\d_]+)\\}");

        private const string kEditorGUIUtilityLoadPrefix = "EditorGUIUtility.Load:";


        CSFile m_File;
        CSClass m_Class;
        CSMethod m_BuildMethod;
        CSMethod m_BindMethod;
        CSMethod m_OnGUIMethod;
        bool m_OverrideOnGUI = false;
        int m_Counter = 0;
        Stack<string> m_WriteChildFormatStack;

        public VXMLDOMElementVisitor(CSFile file, CSClass @class, CSMethod buildMethod)
        {
            m_File = file;
            m_Class = @class;
            SetHandler(this);

            m_BuildMethod = buildMethod;
            m_BindMethod = new CSMethod(Scope.Protected, "void", "Bind", CSMethod.Modifier.Override);
            m_Class.AddMethod(m_BindMethod);

            m_OnGUIMethod = new CSMethod(Scope.Public, "void", "OnGUI", CSMethod.Modifier.Override);

            m_WriteChildFormatStack = new Stack<string>();
            PushAddChildMethod(string.Empty);
        }

        public void Build()
        {
            if (m_OverrideOnGUI)
                m_Class.AddMethod(m_OnGUIMethod);

            m_BindMethod.AddBodyLine("base.Bind();");
            m_BuildMethod.AddBodyLine("base.Build();");
        }

        string GetIdForTag(string tag)
        {
            return s_SlugifyTagRegex.Replace(tag, string.Empty) + "_" + (++m_Counter);
        }

        protected override IEnumerable<DOMElement> GetChildrenOf(DOMElement node)
        {
            var container = node as DOMContainer;
            return container != null ? container.children : null;
        }

        void WriteSetOrBind(string fieldName, string className, string viewPropertyName, string value, string setFormat = "{0}.{1} = {2};")
        {
            if (!string.IsNullOrEmpty(value))
            {
                var match = GetParameterMatch(value);
                if (match.Success)
                {
                    var slugifiedValue = NameUtility.SlugifyConstName(match.Groups[1].Value);
                    var bindablePropertyName = "property" + NameUtility.PascalCase(viewPropertyName);
                    m_BindMethod.AddBodyLine(string.Format("RegisterBinding({0}.AddBinding({1}.{2}, {3}));", fieldName, className, bindablePropertyName, slugifiedValue));
                }
                else
                    m_BuildMethod.AddBodyLine(string.Format(setFormat, fieldName, viewPropertyName, value));
            }
        }

        void WriteBind(string fieldName, string className, string viewPropertyName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var match = GetParameterMatch(value);
                var actualValue = match.Success
                    ? NameUtility.SlugifyConstName(match.Groups[1].Value)
                    : value;
                var bindablePropertyName = "property" + NameUtility.PascalCase(viewPropertyName);
                m_BindMethod.AddBodyLine(string.Format("RegisterBinding({0}.AddBinding({1}.{2}, {3}));", fieldName, className, bindablePropertyName, actualValue));
            }
        }

        void WriteClasses(string fieldName, string @class)
        {
            if (!string.IsNullOrEmpty(@class))
            {
                var split = @class.Split(' ');
                split = split.Select(s => "\"" + s + "\"").ToArray();
                m_BuildMethod.AddBodyLine(string.Format("{0}.AddClass({1});", fieldName, string.Join(", ", split)));
            }
        }

        void WriteSetOrBindTexture(string fieldName, string @class, string viewPropertyName, string value)
        {
            var setFormat = "{0}.{1} = {2};";
            if (!string.IsNullOrEmpty(value) && value.StartsWith(kEditorGUIUtilityLoadPrefix))
            {
                value = value.Substring(kEditorGUIUtilityLoadPrefix.Length);
                setFormat = "{0}.{1} = (Texture2D)EditorGUIUtility.Load(\"{2}\");";
            }
            WriteSetOrBind(fieldName, @class, viewPropertyName, value, setFormat);
        }

        static string GetCSharpType(DOMElement elt, string defaultClass)
        {
            return string.IsNullOrEmpty(elt.type) ? defaultClass : elt.type;
        }

        string WriteChild(string tag, string @class, DOMElement elt)
        {
            var instanceId = GetIdForTag(tag);
            var fieldName = GetFieldNameFor(instanceId);

            m_Class.AddField(new CSField(Scope.Private, fieldName, @class));
            m_BuildMethod.AddBodyLine(string.Format("{0} = new {1}();", fieldName, @class));
            m_BuildMethod.AddBodyLine(string.Format(m_WriteChildFormatStack.Peek(), fieldName, m_WriteChildFormatStack.Peek()));

            WriteSetOrBind(fieldName, @class, "widthOverride", elt.widthOverride);
            WriteSetOrBind(fieldName, @class, "heightOverride", elt.heightOverride);

            return fieldName;
        }

        void PushAddChildMethod(string container, string method = "AddChild")
        {
            if (!string.IsNullOrEmpty(container))
                m_WriteChildFormatStack.Push(container + "." + method + "({0});");
            else
                m_WriteChildFormatStack.Push(method + "({0});");
        }

        void PopAddChildMethod()
        {
            m_WriteChildFormatStack.Pop();
        }

        static string GetFieldNameFor(string slugified)
        {
            Assert.IsFalse(string.IsNullOrEmpty(slugified));
            return "m_" + slugified.Substring(0, 1).ToUpper() + slugified.Substring(1);
        }

        static MatchCollection GetParameterMatches(string text)
        {
            return s_ParameterRegex.Matches(text);
        }

        static Match GetParameterMatch(string text)
        {
            return s_ParameterRegex.Match(text);
        }
    }
}
