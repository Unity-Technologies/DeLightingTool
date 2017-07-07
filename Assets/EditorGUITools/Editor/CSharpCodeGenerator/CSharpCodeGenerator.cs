using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UnityEditor.Experimental
{
    [Flags]
    public enum Scope
    {
        None = 0,
        Private = 1 << 0,
        Protected = 1 << 1,
        Public = 1 << 2,
        Internal = 1 << 3
    }

    public class CSFile
    {
        string m_Namespace = string.Empty;
        List<CSClass> m_Classes = new List<CSClass>();
        List<CSEnum> m_Enums = new List<CSEnum>();
        HashSet<string> m_Includes = new HashSet<string>();
        HashSet<int> m_PragmaDisableWarnings = new HashSet<int>();

        public CSFile(string @namespace = null)
        {
            m_Namespace = @namespace;
        }

        public CSFile AddInclude(string include)
        {
            m_Includes.Add(include);
            return this;
        }

        public CSFile AddClass(CSClass @class)
        {
            m_Classes.Add(@class);
            return this;
        }

        public CSFile AddEnum(CSEnum @enum)
        {
            m_Enums.Add(@enum);
            return this;
        }

        public CSFile AddWarningDisable(int warningCode)
        {
            m_PragmaDisableWarnings.Add(warningCode);
            return this;
        }

        public void GenerateIn(StringBuilder builder)
        {
            var indent = 0;

            if (m_PragmaDisableWarnings.Count > 0)
            {
                builder.Append("#pragma warning disable ");
                var first = true;
                foreach (var warning in m_PragmaDisableWarnings)
                {
                    if (!first)
                        builder.Append(", ");
                    builder.Append(warning);
                    first = false;
                }
                builder.AppendLine();
            }

            if (!string.IsNullOrEmpty(m_Namespace))
            {
                builder.AppendLine("namespace " + m_Namespace);
                builder.AppendLine("{");
                ++indent;
            }

            foreach (var include in m_Includes)
            {
                Helpers.Indent(builder, indent);
                builder.Append("using ");
                builder.Append(include);
                builder.AppendLine(";");
            }
            builder.AppendLine("");
            for (int i = 0; i < m_Enums.Count; i++)
            {
                m_Enums[i].GenerateIn(builder, indent);
                builder.AppendLine("");
            }
            for (int i = 0; i < m_Classes.Count; i++)
            {
                m_Classes[i].GenerateIn(builder, indent);
                builder.AppendLine("");
            }

            if (!string.IsNullOrEmpty(m_Namespace))
            {
                --indent;
                builder.AppendLine("}");
            }

            if (m_PragmaDisableWarnings.Count > 0)
            {
                builder.Append("#pragma warning restore ");
                var first = true;
                foreach (var warning in m_PragmaDisableWarnings)
                {
                    if (!first)
                        builder.Append(", ");
                    builder.Append(warning);
                    first = false;
                }
                builder.AppendLine();
            }
        }
    }

    public class CSClass
    {
        [Flags]
        public enum Modifier
        {
            None = 0,
            Static = 1 << 0,
            Sealed = 1 << 1,
            Partial = 1 << 2,
            Abstract = 1 << 3
        }

        public string name { get { return m_Name; } }

        Scope m_Scope;
        string m_Name;
        Modifier m_Modifiers;
        List<string> m_Parents = new List<string>();
        List<CSEnum> m_Enums = new List<CSEnum>();
        List<CSField> m_Fields = new List<CSField>();
        List<CSProperty> m_Properties = new List<CSProperty>();
        List<CSMethod> m_Methods = new List<CSMethod>();
        List<CSClass> m_InnerClasses = new List<CSClass>();

        public CSClass(Scope scope, string name, Modifier modifiers = 0)
        {
            m_Scope = scope;
            m_Name = name;
            m_Modifiers = modifiers;
        }

        public void AddClass(CSClass @class)
        {
            m_InnerClasses.Add(@class);
        }

        public CSClass AddParent(string parent)
        {
            m_Parents.Add(parent);
            return this;
        }

        public CSClass AddEnum(CSEnum @enum)
        {
            m_Enums.Add(@enum);
            return this;
        }

        public CSClass AddField(CSField field)
        {
            m_Fields.Add(field);
            return this;
        }

        public CSClass AddProperty(CSProperty property)
        {
            m_Properties.Add(property);
            return this;
        }

        public CSClass AddMethod(CSMethod method)
        {
            m_Methods.Add(method);
            return this;
        }

        public bool HasMethod(string name)
        {
            return GetMethod(name) != null;
        }

        public CSMethod GetMethod(string name)
        {
            for (int i = 0; i < m_Methods.Count; i++)
            {
                if (m_Methods[i].name == name)
                {
                    return m_Methods[i];
                }
            }
            return null;
        }

        public void GenerateIn(StringBuilder builder, int indentLevel)
        {
            Helpers.Indent(builder, indentLevel);

            if ((m_Modifiers & Modifier.Sealed) != 0)
                builder.Append("sealed ");
            else if ((m_Modifiers & Modifier.Abstract) != 0)
                builder.Append("abstract ");

            builder.Append(Mapping.GetScope(m_Scope));
            if ((m_Modifiers & Modifier.Partial) != 0)
                builder.Append(" partial");
            if ((m_Modifiers & Modifier.Static) != 0)
                builder.Append(" static");
            builder.Append(" class ");
            builder.Append(m_Name);
            if (m_Parents.Count > 0)
            {
                builder.Append(" : ");
                for (int i = 0; i < m_Parents.Count; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(m_Parents[i]);
                }
            }
            builder.AppendLine();
            Helpers.Indent(builder, indentLevel);
            builder.AppendLine("{");
            ++indentLevel;

            for (int i = 0; i < m_InnerClasses.Count; i++)
                m_InnerClasses[i].GenerateIn(builder, indentLevel);

            for (int i = 0; i < m_Enums.Count; i++)
            {
                m_Enums[i].GenerateIn(builder, indentLevel);
                builder.AppendLine("");
            }
            builder.AppendLine();
            for (int i = 0; i < m_Fields.Count; i++)
            {
                m_Fields[i].GenerateIn(builder, indentLevel);
                builder.AppendLine("");
            }
            builder.AppendLine();
            for (int i = 0; i < m_Properties.Count; i++)
            {
                m_Properties[i].GenerateIn(builder, indentLevel);
                builder.AppendLine("");
            }
            builder.AppendLine();
            for (int i = 0; i < m_Methods.Count; i++)
            {
                m_Methods[i].GenerateIn(builder, indentLevel);
                builder.AppendLine("");
            }
            --indentLevel;
            Helpers.Indent(builder, indentLevel);
            builder.AppendLine("}");
        }
    }

    public class CSEnum
    {
        Scope m_Scope;
        string m_Name;
        List<string> m_Names = new List<string>();
        List<int?> m_Values = new List<int?>();

        public CSEnum(Scope scope, string name)
        {
            m_Scope = scope;
            m_Name = name;
        }

        public CSEnum AddName(string name, int? value = null)
        {
            m_Names.Add(name);
            m_Values.Add(value);
            return this;
        }

        public void GenerateIn(StringBuilder builder, int indentLevel)
        {
            Helpers.Indent(builder, indentLevel);
            builder.Append(Mapping.GetScope(m_Scope));
            builder.Append(" enum ");
            builder.AppendLine(m_Name);
            Helpers.Indent(builder, indentLevel);
            builder.AppendLine("{");
            ++indentLevel;
            for (int i = 0; i < m_Names.Count; i++)
            {
                Helpers.Indent(builder, indentLevel);
                builder.Append(m_Names[i]);
                if (i < m_Values.Count && m_Values[i].HasValue)
                {
                    builder.Append(" = ");
                    builder.Append(m_Values[i].Value);
                }
                builder.AppendLine(",");
            }
            --indentLevel;
            Helpers.Indent(builder, indentLevel);
            builder.AppendLine("}");
        }
    }

    public class CSField
    {
        [Flags]
        public enum Modifier
        {
            None = 0,
            Static = 1 << 0,
            Readonly = 1 << 1,
            Const = 1 << 2
        }

        Scope m_Scope;
        string m_Name;
        string m_Type;
        string m_Value;
        Modifier m_Modifers;

        public CSField(Scope scope, string name, string type, string value = null, Modifier modifiers = 0)
        {
            m_Modifers = modifiers;
            m_Scope = scope;
            m_Name = name;
            m_Type = type;
            m_Value = value;
        }

        public void GenerateIn(StringBuilder builder, int indentLevel)
        {
            Helpers.Indent(builder, indentLevel);
            builder.Append(Mapping.GetScope(m_Scope));
            builder.Append(" ");
            if ((m_Modifers & Modifier.Const) != 0)
                builder.Append("const ");
            if ((m_Modifers & Modifier.Static) != 0)
                builder.Append("static ");
            if ((m_Modifers & Modifier.Readonly) != 0)
                builder.Append("readonly ");
            builder.Append(m_Type);
            builder.Append(" ");
            builder.Append(m_Name);
            if (!string.IsNullOrEmpty(m_Value))
            {
                builder.Append(" = ");
                builder.Append(m_Value);
            }
            builder.AppendLine(";");
        }
    }

    public class CSProperty
    {
        [Flags]
        public enum Modifier
        {
            None = 0,
            Abstract = 1 << 0,
            Override = 1 << 1,
            Virtual = 1 << 2,
            Static = 1 << 3
        }

        Scope m_Scope;
        string m_Name;
        string m_Type;
        string m_Getter = string.Empty;
        string m_Setter = string.Empty;
        Modifier m_Modifiers;

        public CSProperty(Scope scope, string name, string type, Modifier modifiers = 0)
        {
            m_Modifiers = modifiers;
            m_Scope = scope;
            m_Name = name;
            m_Type = type;
        }

        public CSProperty SetGetter(string getter)
        {
            m_Getter = getter;
            return this;
        }

        public CSProperty SetSetter(string setter)
        {
            m_Setter = setter;
            return this;
        }

        public void GenerateIn(StringBuilder builder, int indentLevel)
        {
            Helpers.Indent(builder, indentLevel);
            builder.Append(Mapping.GetScope(m_Scope));
            builder.Append(" ");
            if ((m_Modifiers & Modifier.Abstract) != 0)
                builder.Append("abstract ");
            if ((m_Modifiers & Modifier.Override) != 0)
                builder.Append("override ");
            if ((m_Modifiers & Modifier.Virtual) != 0)
                builder.Append("virtual ");
            if ((m_Modifiers & Modifier.Static) != 0)
                builder.Append("static ");
            builder.Append(" ");
            builder.Append(m_Type);
            builder.Append(" ");
            builder.AppendLine(m_Name);
            Helpers.Indent(builder, indentLevel);
            builder.AppendLine("{");
            ++indentLevel;
            if (!string.IsNullOrEmpty(m_Getter))
            {
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("get");
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("{");
                ++indentLevel;
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine(m_Getter);
                --indentLevel;
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("}");
            }
            if (!string.IsNullOrEmpty(m_Setter))
            {
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("set");
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("{");
                ++indentLevel;
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine(m_Setter);
                --indentLevel;
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("}");
            }
            --indentLevel;
            Helpers.Indent(builder, indentLevel);
            builder.AppendLine("}");
        }
    }

    public class CSMethod
    {
        [Flags]
        public enum Modifier
        {
            None = 0,
            Abstract = 1 << 0,
            Override = 1 << 1,
            Virtual = 1 << 2,
            Static = 1 << 3
        }

        public struct Argument
        {
            internal readonly string type;
            internal readonly string name;
            internal readonly string @default;

            public Argument(string type, string name, string @default = null)
            {
                this.type = type;
                this.name = name;
                this.@default = @default;
            }
        }

        public struct Attribute
        {
            internal readonly string name;
            internal readonly List<string> arguments;

            public Attribute(string name, List<string> arguments)
            {
                this.name = name;
                this.arguments = arguments ?? new List<string>();
            }
        }

        public string name { get { return m_Name; } }

        Scope m_Scope;
        string m_Name;
        string m_ReturnType;
        List<Argument> m_Arguments = new List<Argument>();
        List<Attribute> m_Attributes = new List<Attribute>();
        List<string> m_Body = new List<string>();
        Modifier m_Modifiers;

        public CSMethod(Scope scope, string returnType, string name, Modifier modifiers = 0)
        {
            m_Scope = scope;
            m_Name = name;
            m_ReturnType = returnType;
            m_Modifiers = modifiers;
        }

        public CSMethod AddArgument(string type, string name, string @default = null)
        {
            m_Arguments.Add(new Argument(type, name, @default));
            return this;
        }

        public CSMethod AddAttribute(string name, params string[] arguments)
        {
            m_Attributes.Add(new Attribute(name, new List<string>(arguments)));
            return this;
        }

        public CSMethod AddBodyLine(string body)
        {
            m_Body.Add(body);
            return this;
        }

        public void GenerateIn(StringBuilder builder, int indentLevel)
        {
            for (int i = 0; i < m_Attributes.Count; i++)
            {
                Helpers.Indent(builder, indentLevel);
                builder.Append("[");
                builder.Append(m_Attributes[i].name);
                if (m_Attributes[i].arguments.Count > 0)
                {
                    builder.Append("(");
                    for (int j = 0; j < m_Attributes[i].arguments.Count; j++)
                    {
                        if (j > 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(m_Attributes[i].arguments[j]);
                    }
                    builder.Append(")");
                }
                builder.AppendLine("]");
            }
            Helpers.Indent(builder, indentLevel);
            builder.Append(Mapping.GetScope(m_Scope));
            builder.Append(" ");
            if ((m_Modifiers & Modifier.Static) != 0)
                builder.Append("static ");
            if ((m_Modifiers & Modifier.Abstract) != 0)
                builder.Append("abstract ");
            if ((m_Modifiers & Modifier.Override) != 0)
                builder.Append("override ");
            if ((m_Modifiers & Modifier.Virtual) != 0)
                builder.Append("virtual ");
            builder.Append(" ");
            builder.Append(m_ReturnType);
            builder.Append(" ");
            builder.Append(m_Name);
            builder.Append("(");
            for (int i = 0; i < m_Arguments.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(m_Arguments[i].type);
                builder.Append(" ");
                builder.Append(m_Arguments[i].name);
                if (!string.IsNullOrEmpty(m_Arguments[i].@default))
                {
                    builder.Append(" = ");
                    builder.Append(m_Arguments[i].@default);
                }
            }
            builder.Append(")");
            if ((m_Modifiers & Modifier.Abstract) != 0)
                builder.AppendLine(";");
            else
            {
                builder.AppendLine();
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("{");
                ++indentLevel;
                for (int i = 0; i < m_Body.Count; i++)
                {
                    Helpers.Indent(builder, indentLevel);
                    builder.AppendLine(m_Body[i]);
                }
                --indentLevel;
                Helpers.Indent(builder, indentLevel);
                builder.AppendLine("}");
            }
        }
    }

    static class Mapping
    {
        public static string GetScope(Scope scope)
        {
            if ((scope & Scope.Protected) != 0)
                return "protected";
            if ((scope & (Scope.Protected | Scope.Internal)) == (Scope.Protected | Scope.Internal))
                return "protected internal";
            if ((scope & Scope.Internal) != 0)
                return "internal";
            if ((scope & Scope.Public) != 0)
                return "public";
            return "private";
        }
    }

    static class Helpers
    {
        const string kIndent = "    ";

        internal static void Indent(StringBuilder builder, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++)
                builder.Append(kIndent);
        }
    }
}
