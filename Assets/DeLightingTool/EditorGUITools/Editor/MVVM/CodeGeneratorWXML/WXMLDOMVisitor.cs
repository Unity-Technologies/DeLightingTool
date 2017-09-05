using System.Collections.Generic;
using System.Text;
using UnityEditor.Experimental.CodeGenerator;
using UnityEngine.Assertions;

namespace UnityEditor.Experimental.WXMLInternal
{
    class WXMLDOMVisitor : IAssetCodeGenerator<DOMDocument>
    {
        CSFile m_File = null;
        CSClass m_Class = null;
        CSMethod m_AwakeMethod = null;
        CSMethod m_OnDestroyMethod = null;
        CSMethod m_OnVMPropertyChanged = null;

        bool m_HasService = false;
        bool m_HasRoot = false;

        public string GenerateFrom(string workingDirectory, string assetName, DOMDocument document)
        {
            m_File = new CSFile(document.@namespace)
                .AddWarningDisable(414)
                .AddInclude("System.ComponentModel")
                .AddInclude("UnityEngine")
                .AddInclude("UnityEditor.Experimental.VisualElements")
                .AddInclude("UnityEditor");

            m_Class = new CSClass(Scope.Internal, assetName, CSClass.Modifier.Partial)
                .AddParent("EditorWindow");
            m_File.AddClass(m_Class);

            m_AwakeMethod = new CSMethod(Scope.Private, "void", "Awake");
            m_Class.AddMethod(m_AwakeMethod);
            m_OnDestroyMethod = new CSMethod(Scope.Private, "void", "OnDestroy");
            m_Class.AddMethod(m_OnDestroyMethod);
            m_OnVMPropertyChanged = new CSMethod(Scope.Private, "void", "VmOnPropertyChanged")
                .AddArgument("object", "sender")
                .AddArgument("PropertyChangedEventArgs", "propertyChangedEventArgs")
                .AddBodyLine("Repaint();");
            m_Class.AddMethod(m_OnVMPropertyChanged);

            VisitIn(document);
            VisitIn(document.usings);
            VisitIn(document.editorPrefsProperties);

            var builder = new StringBuilder();
            m_File.GenerateIn(builder);
            return builder.ToString();
        }

        private void VisitIn(DOMEditorPrefsProperty[] editorPrefsProperties)
        {
            if (editorPrefsProperties == null)
                return;

            var prefsToBind = new List<DOMEditorPrefsProperty>();

            for (int i = 0; i < editorPrefsProperties.Length; i++)
            {
                var pref = editorPrefsProperties[i];

                Assert.IsFalse(string.IsNullOrEmpty(pref.name), "name is required for field <editorPrefs />");
                Assert.AreNotEqual(DOMEditorPrefsProperty.Type.None, pref.type, "type is required for field <editorPrefs />");

                var type = string.Empty;
                var setter = string.Empty;
                var getter = string.Empty;
                switch (pref.type)
                {
                    case DOMEditorPrefsProperty.Type.Bool:
                        setter = string.Format("EditorPrefs.SetInt(\"{0}.{1}\", value ? 1 : 0);", m_Class.name, pref.name);
                        getter = string.Format(
                            "return EditorPrefs.GetInt(\"{0}.{1}\", {2}) == 1;", 
                            m_Class.name, 
                            pref.name, 
                            string.IsNullOrEmpty(pref.@default) ? "false" : pref.@default);
                        type = "bool";
                        break;
                    case DOMEditorPrefsProperty.Type.Int:
                        setter = string.Format("EditorPrefs.SetInt(\"{0}.{1}\", value);", m_Class.name, pref.name);
                        getter = string.Format(
                            "return EditorPrefs.GetInt(\"{0}.{1}\", {2});",
                            m_Class.name,
                            pref.name,
                            string.IsNullOrEmpty(pref.@default) ? "0" : pref.@default);
                        type = "int";
                        break;
                    case DOMEditorPrefsProperty.Type.String:
                        setter = string.Format("EditorPrefs.SetString(\"{0}.{1}\", value);", m_Class.name, pref.name);
                        getter = string.Format(
                            "return EditorPrefs.GetString(\"{0}.{1}\", {2});",
                            m_Class.name,
                            pref.name,
                            string.IsNullOrEmpty(pref.@default) ? "string.Empty" : "\"" + pref.@default + "\"");
                        type = "string";
                        break;
                    case DOMEditorPrefsProperty.Type.Enum:
                        Assert.IsFalse(string.IsNullOrEmpty(pref.customType), "customType is required for type 'enum' in <editorPrefs />");
                        setter = string.Format("EditorPrefs.SetInt(\"{0}.{1}\", (int)value);", m_Class.name, pref.name);
                        getter = string.Format(
                            "return ({3})EditorPrefs.GetInt(\"{0}.{1}\", {2});",
                            m_Class.name,
                            pref.name,
                            string.IsNullOrEmpty(pref.@default) ? "default(" + pref.customType + ")" : pref.@default,
                            pref.customType);
                        type = pref.customType;
                        break;
                }

                m_Class.AddProperty(new CSProperty(Scope.Internal, pref.name, type)
                    .SetSetter(setter)
                    .SetGetter(getter));

                if (m_HasService && !string.IsNullOrEmpty(pref.vmPropertyName))
                {
                    m_AwakeMethod.AddBodyLine("m_Service.vm." + pref.vmPropertyName + " = " + pref.name);

                    if (m_HasRoot)
                        prefsToBind.Add(pref);
                }
            }

            if (prefsToBind.Count > 0)
            {
                m_OnVMPropertyChanged.AddBodyLine("switch (propertyChangedEventArgs.PropertyName)");
                m_OnVMPropertyChanged.AddBodyLine("{");
                foreach (var pref in prefsToBind)
                    m_OnVMPropertyChanged.AddBodyLine("    case \"" + pref.vmPropertyName + "\": " + pref.name + " = m_Service.vm." + pref.vmPropertyName + "; break;");
                m_OnVMPropertyChanged.AddBodyLine("}");
            }
        }

        private void VisitIn(DOMDocument document)
        {
            // Create window method
            var createWindowMethod = new CSMethod(Scope.Internal, m_Class.name, "CreateWindow", CSMethod.Modifier.Static)
                .AddBodyLine("var window = GetWindow<" + m_Class.name + ">();");

            if (!string.IsNullOrEmpty(document.title))
                createWindowMethod.AddBodyLine("window.titleContent = new GUIContent(\"" + document.title + "\");");
            createWindowMethod.AddBodyLine("return window;");

            m_Class.AddMethod(createWindowMethod);

            m_HasService = false;
            m_HasRoot = false;

            if (!string.IsNullOrEmpty(document.serviceClass))
            {
                m_HasService = true;
                m_Class.AddField(new CSField(Scope.Private, "m_Service", document.serviceClass));
                m_AwakeMethod.AddBodyLine("m_Service = new " + document.serviceClass + "();");

                var exposedClass = string.IsNullOrEmpty(document.exposedServiceClass)
                    ? document.serviceClass
                    : document.exposedServiceClass;

                m_Class.AddProperty(new CSProperty(Scope.Public, "service", exposedClass)
                    .SetGetter("return m_Service;"));
            }

            if (!string.IsNullOrEmpty(document.rootClass))
            {
                m_HasRoot = true; 
                m_Class.AddField(new CSField(Scope.Private, "m_Root", document.rootClass));
                m_AwakeMethod.AddBodyLine("m_Root = new " + document.rootClass + "();");
            }

            if (m_HasRoot)
            {
                m_AwakeMethod.AddBodyLine("m_Root.parent = new VisualElementRoot();");
                m_AwakeMethod.AddBodyLine("m_Root.RepaintRequested += Repaint;");
                m_OnDestroyMethod.AddBodyLine("m_Root.RepaintRequested -= Repaint;");
                m_OnDestroyMethod.AddBodyLine("m_Root.Dispose();");
                m_OnDestroyMethod.AddBodyLine("m_Root = null;");
                if (m_HasService)
                    m_AwakeMethod.AddBodyLine("m_Root.dataContext = m_Service.vm;");

                m_Class.AddMethod(new CSMethod(Scope.Private, "void", "OnGUI")
                    .AddBodyLine("m_Root.OnGUI();"));
            }
            if (m_HasService)
            {
                m_AwakeMethod.AddBodyLine("m_Service.vm.PropertyChanged += VmOnPropertyChanged;");
                m_AwakeMethod.AddBodyLine("m_Service.vm.SetPropertyChanged((ClassProperty)null);");
                m_OnDestroyMethod.AddBodyLine("m_Service.vm.PropertyChanged -= VmOnPropertyChanged;");
            }
            if (m_HasRoot || m_HasService)
            {
                var onEnableMethod = new CSMethod(Scope.Private, "void", "OnEnable");
                var cond = "m_Service == null || m_Root == null";
                if (!m_HasRoot)
                    cond = "m_Service == null";
                if (!m_HasService)
                    cond = "m_Root == null";

                onEnableMethod.AddBodyLine("if (" + cond + ")");
                onEnableMethod.AddBodyLine("    Awake();");
                m_Class.AddMethod(onEnableMethod);
            }
        }

        private void VisitIn(DOMUsing[] usings)
        {
            if (usings == null)
                return;

            for (var i = 0; i < usings.Length; i++)
            {
                var @using = usings[i];
                Assert.IsFalse(string.IsNullOrEmpty(@using.textContent), "content is required for <using />");
                m_File.AddInclude(@using.textContent);
            }
        }
    }
}
