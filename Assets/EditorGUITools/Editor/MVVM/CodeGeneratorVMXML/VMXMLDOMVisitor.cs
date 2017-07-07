using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor.Experimental.CodeGenerator;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VMXMLInternal
{
    class VMXMLDOMVisitor : DynamicVisitor<DOMMember>, IAssetCodeGenerator<DOMDocument>
    {
        static readonly Regex kVariableReference = new Regex("\\$\\{([^\\}]+)\\}");

        CSFile m_File = null;
        CSClass m_Class = null;
        CSClass m_PropertyClass = null;

        Dictionary<string, HashSet<string>> m_PropertyGetterDependencies = new Dictionary<string, HashSet<string>>();

        public VMXMLDOMVisitor()
        {
            SetHandler(this);
        }

        public string GenerateFrom(string workingdir, string assetName, DOMDocument document)
        {
            m_File = new CSFile(document.@namespace);

            m_Class = new CSClass(Scope.Internal, assetName, CSClass.Modifier.Partial);
            m_File
                .AddClass(m_Class)
                .AddInclude("UnityEditor.Experimental.ViewModel")
                .AddWarningDisable(414);
            m_Class.AddParent("SerializedViewModelBase");

            m_PropertyClass = new CSClass(Scope.Internal, "Properties", CSClass.Modifier.Static);
            m_Class.AddClass(m_PropertyClass);

            if (document.members != null)
            {
                for (int i = 0; i < document.members.Length; i++)
                {
                    if (document.members[i] is DOMProperty)
                        RegisterPropertyGetterDependencies((DOMProperty)document.members[i]);
                }

                for (int i = 0; i < document.members.Length; i++)
                    Visit(document.members[i]);
            }

            var builder = new StringBuilder();
            m_File.GenerateIn(builder);
            return builder.ToString();
        }

        protected override IEnumerable<DOMMember> GetChildrenOf(DOMMember node)
        {
            return null;
        }

        void VisitIn(DOMField field)
        {
            Assert.IsFalse(string.IsNullOrEmpty(field.type), "type is required for <field />");
            Assert.IsFalse(string.IsNullOrEmpty(field.name), "name is required for <field />");

            m_Class.AddField(new CSField(Scope.Private, field.name, field.type, field.@default));
        }

        void VisitIn(DOMCommand command)
        {
            Assert.IsFalse(string.IsNullOrEmpty(command.name), "name is required for <command />");

            var parametricArgs0 = string.Empty;
            var parametricArgs1 = string.Empty;
            if (command.args != null && command.args.Length > 0)
            {
                var parametricTypes = command.args.Select(a => a.type).ToArray();
                parametricArgs0 = "<" + string.Join(", ", parametricTypes) + ">";
                parametricArgs1 = ", " + string.Join(", ", parametricTypes);
            }

            m_PropertyClass.AddField(
                new CSField(
                    Scope.Public, 
                    NameUtility.SlugifyConstName(command.name),
                    "IClassMethod" + parametricArgs0, "new StaticClassMethod<" + m_Class.name + parametricArgs1 + ">(\"" + command.name + "\")", 
                    CSField.Modifier.Static | CSField.Modifier.Readonly));
        }

        void VisitIn(DOMUsing @using)
        {
            Assert.IsFalse(string.IsNullOrEmpty(@using.textContent));

            m_File.AddInclude(@using.textContent);
        }

        void VisitIn(DOMProperty property)
        {
            Assert.IsFalse(string.IsNullOrEmpty(property.type), "type is required for <property />");
            Assert.IsFalse(string.IsNullOrEmpty(property.name), "name is required for <property />");

            var fieldName = NameUtility.SlugifyFieldName(property.name);
            var pptName = NameUtility.SlugifyConstName(property.name);

            var getterContent = property.getter != null ? property.getter.textContent : string.Empty;
            var setterContent = property.setter != null ? property.setter.textContent : string.Empty;

            GeneratePropertyInternal(property.type, property.name, fieldName, pptName, getterContent, setterContent);

            m_Class.AddField(new CSField(Scope.Private, fieldName, property.type, property.@default));
        }

        void VisitIn(DOMSerializedProperty serializedProperty)
        {
            Assert.IsFalse(string.IsNullOrEmpty(serializedProperty.name), "name is required for <serialized-property />");

            var type = string.Empty;
            switch (serializedProperty.type)
            {
                case DOMSerializedProperty.Type.Object:
                case DOMSerializedProperty.Type.Enum:
                {
                    type = serializedProperty.customType;
                    break;
                }
                case DOMSerializedProperty.Type.AnimationCurve:
                case DOMSerializedProperty.Type.Bounds:
                case DOMSerializedProperty.Type.Vector2:
                case DOMSerializedProperty.Type.Vector3:
                case DOMSerializedProperty.Type.Vector4:
                case DOMSerializedProperty.Type.Color:
                case DOMSerializedProperty.Type.Rect:
                case DOMSerializedProperty.Type.Quaternion:
                {
                    type = serializedProperty.type.ToString();
                    break;
                }
                default:
                {
                    type = serializedProperty.type.ToString().ToLowerInvariant();
                    break;
                }
                    
            }

            var pptName = NameUtility.SlugifyConstName(serializedProperty.name);
            var fieldName = "this[\"" + serializedProperty.name + "\"]";
            var fieldType = (string)null;
            switch (serializedProperty.type)
            {
                case DOMSerializedProperty.Type.AnimationCurve: fieldName += ".animationCurveValue"; break;
                case DOMSerializedProperty.Type.Bool: fieldName += ".boolValue"; break;
                case DOMSerializedProperty.Type.Bounds: fieldName += ".boundsValue"; break;
                case DOMSerializedProperty.Type.Color: fieldName += ".colorValue"; break;
                case DOMSerializedProperty.Type.Double: fieldName += ".doubleValue"; break;
                case DOMSerializedProperty.Type.Enum: fieldName += ".enumValueIndex"; fieldType = "int"; break;
                case DOMSerializedProperty.Type.Float: fieldName += ".floatValue"; break;
                case DOMSerializedProperty.Type.Int: fieldName += ".intValue"; break;
                case DOMSerializedProperty.Type.Long: fieldName += ".longValue"; break;
                case DOMSerializedProperty.Type.Object: fieldName += ".objectReferenceValue"; break;
                case DOMSerializedProperty.Type.Quaternion: fieldName += ".quaternionValue"; break;
                case DOMSerializedProperty.Type.String: fieldName += ".stringValue"; break;
                case DOMSerializedProperty.Type.Vector2: fieldName += ".vector2Value"; break;
                case DOMSerializedProperty.Type.Vector3: fieldName += ".vector3Value"; break;
                case DOMSerializedProperty.Type.Vector4: fieldName += ".vector4Value"; break;
                case DOMSerializedProperty.Type.Rect: fieldName += ".rectValue"; break;
                default: throw new NotSupportedException("Not supported type: " + serializedProperty.type);
            }

            GeneratePropertyInternal(type, serializedProperty.name, fieldName, pptName, null, null, fieldType);
        }

        void RegisterPropertyGetterDependencies(DOMProperty property)
        {
            Assert.IsNotNull(property);
            Assert.IsFalse(string.IsNullOrEmpty(property.name));

            if (property.getter != null && !string.IsNullOrEmpty(property.getter.textContent))
            {
                var matches = kVariableReference.Matches(property.getter.textContent);
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    var referencedProperty = match.Groups[1].Value.Trim();
                    if (!m_PropertyGetterDependencies.ContainsKey(referencedProperty))
                        m_PropertyGetterDependencies[referencedProperty] = new HashSet<string>();
                    m_PropertyGetterDependencies[referencedProperty].Add(property.name);
                }
            }
        }

        void GeneratePropertyInternal(string type, string name, string fieldName, string pptName, string getterContent, string setterContent, string fieldType = null)
        {
            var csProperty = new CSProperty(Scope.Public, name, type);
            m_Class.AddProperty(csProperty);

            if (string.IsNullOrEmpty(getterContent))
                csProperty.SetGetter("return (" + type + ")" + fieldName + ";");
            else
                csProperty.SetGetter(kVariableReference.Replace(getterContent, "$1") + ";");

            var hasSetter = !string.IsNullOrEmpty(setterContent)
                || string.IsNullOrEmpty(getterContent) && string.IsNullOrEmpty(setterContent);
            if (hasSetter)
            {
                var setterMethod = new CSMethod(Scope.Private, "bool", "Set" + NameUtility.PascalCase(name))
                    .AddArgument(type, "value");
                m_Class.AddMethod(setterMethod);

                if (!string.IsNullOrEmpty(setterContent))
                    setterMethod.AddBodyLine(setterContent);
                else
                {
                    var fieldTypeCast = (string.IsNullOrEmpty(fieldType) ? "" : "(" + fieldType + ")");
                    setterMethod
                        .AddBodyLine("if (" + fieldName + " != " + fieldTypeCast + "value)")
                        .AddBodyLine("{")
                        .AddBodyLine("    " + fieldName + " = " + fieldTypeCast + "value;")
                        .AddBodyLine("    return true;")
                        .AddBodyLine("}")
                        .AddBodyLine("return false;");
                }

                var dependentProperties = new HashSet<string>() { name };

                if (m_PropertyGetterDependencies.ContainsKey(name))
                    dependentProperties.UnionWith(m_PropertyGetterDependencies[name]);

                var properties = dependentProperties.Select(p => "Properties." + NameUtility.SlugifyConstName(p)).Aggregate((l, r) => l + ", " + r);

                csProperty.SetSetter("if (" + setterMethod.name + "(value)) { SetPropertyChanged(" + properties + "); }");
            }

            m_PropertyClass.AddField(
                new CSField(
                    Scope.Public,
                    pptName,
                    "ClassProperty<" + type + ">",
                    "new StaticClassProperty<" + type + ", " + m_Class.name + ">(\"" + name + "\")",
                    CSField.Modifier.Static | CSField.Modifier.Readonly));
        }
    }
}
