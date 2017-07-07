using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Experimental.CodeGenerator;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.VXMLInternal
{
    class VXMLDOMVisitor : DynamicVisitor<DOMNode>, IAssetCodeGenerator<DOMDocument>
    {
        CSFile m_File = null;
        CSClass m_Class = null;

        HashSet<DOMProperty> m_Properties = new HashSet<DOMProperty>();
        HashSet<DOMUsing> m_Usings = new HashSet<DOMUsing>();
        HashSet<DOMCommand> m_Commands = new HashSet<DOMCommand>();
        DOMStyleSheet m_StyleSheet = null;

        string m_WorkingDirectory = null;
        HashSet<string> m_FileDependencies = new HashSet<string>();
        CSMethod m_BuildMethod;
        public IEnumerable<string> fileDependencies { get { return m_FileDependencies; } }

        public VXMLDOMVisitor()
        {
            SetHandler(this);
        }

        public string GenerateFrom(string workingDirectory, string assetName, DOMDocument document)
        {
            m_Properties.Clear();
            m_Usings.Clear();
            m_Commands.Clear();
            m_FileDependencies.Clear();

            m_WorkingDirectory = workingDirectory;
            m_File = new CSFile(document.@namespace)
                .AddWarningDisable(414)
                .AddInclude("UnityEditor.Experimental.VisualElements");

            m_Class = new CSClass(Scope.Internal, assetName, CSClass.Modifier.Partial)
                .AddParent("IMGUIVisualContainer");
            m_File.AddClass(m_Class);

            if (document.nodes != null)
            {
                for (int i = 0; i < document.nodes.Length; i++)
                    Visit(document.nodes[i]);
            }

            VisitIn(document.stylesheet);
            VisitIn(document.root);

            Build();

            var builder = new StringBuilder();
            m_File.GenerateIn(builder);
            return builder.ToString();
        }

        protected override IEnumerable<DOMNode> GetChildrenOf(DOMNode node)
        {
            return null;
        }

        void VisitIn(DOMRoot root)
        {
            if (root == null)
                return;

            m_BuildMethod = new CSMethod(Scope.Protected, "void", "Build", CSMethod.Modifier.Override);
            m_Class.AddMethod(m_BuildMethod);
            var visitor = new VXMLDOMElementVisitor(m_File, m_Class, m_BuildMethod);
            visitor.Visit(root);
            visitor.Build();
        }

        void VisitIn(DOMCommand command)
        {
            Assert.IsFalse(string.IsNullOrEmpty(command.name), "name is required for <command />");

            m_Commands.Add(command);
        }

        void VisitIn(DOMImport import)
        {
            Assert.IsFalse(string.IsNullOrEmpty(import.textContent), "content is required for <import />");

            var referencedFile = Path.Combine(m_WorkingDirectory, import.textContent).Replace("\\", "/");
            m_FileDependencies.Add(referencedFile);

            if (!File.Exists(referencedFile))
            {
                Debug.LogWarningFormat("Referenced file {0} is missing", referencedFile);
                return;
            }

            DOMDocument definitionInstance = null;
            using (var reader = new StreamReader(referencedFile))
            {
                definitionInstance = ViewGenerator.serializer.Deserialize(reader) as DOMDocument;
            }
            if (definitionInstance != null
                && definitionInstance.nodes != null)
            {
                foreach (var node in definitionInstance.nodes)
                    Visit(node);
            }
        }

        void VisitIn(DOMProperty property)
        {
            Assert.IsFalse(string.IsNullOrEmpty(property.type), "type is required for <property />");
            Assert.IsFalse(string.IsNullOrEmpty(property.name), "name is required for <property />");

            m_Properties.Add(property);
        }

        void VisitIn(DOMUsing @using)
        {
            Assert.IsFalse(string.IsNullOrEmpty(@using.textContent), "content is required for <using />");

            m_Usings.Add(@using);
        }

        void VisitIn(DOMStyleSheet styleSheet)
        {
            m_StyleSheet = styleSheet;
        }

        void Build(DOMCommand command)
        {
            Assert.IsFalse(string.IsNullOrEmpty(command.name), "name is required for <command />");

            var pptName = NameUtility.SlugifyConstName(command.name);

            var parametricArgs = string.Empty;
            if (command.args != null && command.args.Length > 0)
            {
                var builder = new StringBuilder();
                builder.Append("<");
                for (int i = 0; i < command.args.Length; i++)
                {
                    if (i > 0)
                        builder.Append(", ");
                    builder.Append(command.args[i].type);
                }
                builder.Append(">");
                parametricArgs = builder.ToString();
            }

            m_Class.AddField(
                new CSField(
                    Scope.Private,
                    pptName,
                    "IClassMethod" + parametricArgs,
                    "new DynamicClassMethod" + parametricArgs + "(\"" + command.name + "\")",
                    CSField.Modifier.Static | CSField.Modifier.Readonly));
        }

        void Build(DOMProperty property)
        {
            Assert.IsFalse(string.IsNullOrEmpty(property.type), "type is required for <property />");
            Assert.IsFalse(string.IsNullOrEmpty(property.name), "name is required for <property />");

            var pptName = NameUtility.SlugifyConstName(property.name);

            m_Class.AddField(
                new CSField(
                    Scope.Private,
                    pptName,
                    "ClassProperty<" + property.type + ">",
                    "new DynamicClassProperty<" + property.type + ">(\"" + property.name + "\")",
                    CSField.Modifier.Static | CSField.Modifier.Readonly));
        }

        void Build(DOMUsing @using)
        {
            Assert.IsFalse(string.IsNullOrEmpty(@using.textContent), "content is required for <using />");

            m_File.AddInclude(@using.textContent);
        }

        void Build(DOMStyleSheet styleSheet)
        {
            if (m_BuildMethod != null && styleSheet != null)
                m_BuildMethod.AddBodyLine("AddStyleSheet(StyleSheet.GetStyleSheet<" + styleSheet.path + ">());");
        }

        void Build()
        {
            foreach (var command in m_Commands)
                Build(command);
            foreach (var property in m_Properties)
                Build(property);
            foreach (var @using in m_Usings)
                Build(@using);
            Build(m_StyleSheet);
        }
    }
}
