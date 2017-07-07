using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    class DelightingToolWindow : EditorWindow
    {
        internal static DelightingUI.Mode prefsDisplayedUIMode
        {
            get { return (DelightingUI.Mode)EditorPrefs.GetInt("DelightingViewModel.displayedUIMode", (int)DelightingUI.Mode.Normal); }
            set { EditorPrefs.SetInt("DelightingViewModel.displayedUIMode", (int)value); }
        }

        internal static bool prefsAutoCompute
        {
            get { return EditorPrefs.GetInt("DelightingViewModel.autoCompute", 0) == 1; }
            set { EditorPrefs.SetInt("DelightingViewModel.autoCompute", value ? 1 : 0); }
        }

        internal static string prefsBaseTextureSuffix
        {
            get { return EditorPrefs.GetString("DelightingViewModel.baseTextureSuffix", DelightingService.kDefaultBaseTextureSuffix); }
            set { EditorPrefs.SetString("DelightingViewModel.baseTextureSuffix", value); }
        }

        internal static string prefsNormalsTextureSuffix
        {
            get { return EditorPrefs.GetString("DelightingViewModel.normalsTextureSuffix", DelightingService.kDefaultNormalsTextureSuffix); }
            set { EditorPrefs.SetString("DelightingViewModel.normalsTextureSuffix", value); }
        }

        internal static string prefsBentNormalsTextureSuffix
        {
            get { return EditorPrefs.GetString("DelightingViewModel.bentNormalsTextureSuffix", DelightingService.kDefaultBentNormalsTextureSuffix); }
            set { EditorPrefs.SetString("DelightingViewModel.bentNormalsTextureSuffix", value); }
        }

        internal static string prefsAmbientOcclusionTextureSuffix
        {
            get { return EditorPrefs.GetString("DelightingViewModel.ambientOcclusionTextureSuffix", DelightingService.kDefaultAmbientOcclusionTextureSuffix); }
            set { EditorPrefs.SetString("DelightingViewModel.ambientOcclusionTextureSuffix", value); }
        }

        internal static string prefsPositionsTextureSuffix
        {
            get { return EditorPrefs.GetString("DelightingViewModel.positionsTextureSuffix", DelightingService.kDefaultPositionTextureSuffix); }
            set { EditorPrefs.SetString("DelightingViewModel.positionsTextureSuffix", value); }
        }

        internal static string prefsMaskTextureSuffix
        {
            get { return EditorPrefs.GetString("DelightingViewModel.maskTextureSuffix", DelightingService.kDefaultMaskTextureSuffix); }
            set { EditorPrefs.SetString("DelightingViewModel.maskTextureSuffix", value); }
        }

        static readonly GUIContent kTitle = new GUIContent("Delighting Tool");

        public IDelighting service { get { return m_Service; } }
        DelightingService m_Service = null;

        DelightingToolVisualContainer m_Root = new DelightingToolVisualContainer();

        internal static DelightingToolWindow CreateWindow()
        {
            var window = GetWindow<DelightingToolWindow>();
            window.titleContent = kTitle;
            return window;
        }

        void Awake()
        {
            m_Service = new DelightingService();
            m_Service.vm.PropertyChanged += VmOnPropertyChanged;
            m_Root.dataContext = m_Service.vm;
            m_Root.RepaintRequested += Repaint;

            m_Service.vm.displayedUIMode = prefsDisplayedUIMode;
            m_Service.vm.autoCompute = prefsAutoCompute;
            m_Service.vm.baseTextureSuffix = prefsBaseTextureSuffix;
            m_Service.vm.normalsTextureSuffix = prefsNormalsTextureSuffix;
            m_Service.vm.bentNormalsTextureSuffix = prefsBentNormalsTextureSuffix;
            m_Service.vm.ambientOcclusionTextureSuffix = prefsAmbientOcclusionTextureSuffix;
            m_Service.vm.positionsTextureSuffix = prefsPositionsTextureSuffix;
            m_Service.vm.maskTextureSuffix = prefsMaskTextureSuffix;
            m_Service.log = new Logger(Debug.logger.logHandler);
            m_Service.log.filterLogType = LogType.Warning;
        }

        void OnEnable()
        {
            if (m_Service == null || m_Root == null) 
                Awake();
        }

        void OnDestroy()
        {
            m_Service.vm.PropertyChanged -= VmOnPropertyChanged;
            m_Root.RepaintRequested -= Repaint;
            m_Root.Dispose();
            m_Root = null;
        }

        void OnGUI()
        {
            m_Root.OnGUI();
        }

        void VmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Repaint();

            switch (propertyChangedEventArgs.PropertyName)
            {
                case "displayedUIMode": prefsDisplayedUIMode = m_Service.vm.displayedUIMode; break;
                case "autoCompute": prefsAutoCompute = m_Service.vm.autoCompute; break;
                case "baseTextureSuffix": prefsBaseTextureSuffix = m_Service.vm.baseTextureSuffix; break;
                case "normalsTextureSuffix": prefsNormalsTextureSuffix = m_Service.vm.normalsTextureSuffix; break;
                case "bentNormalsTextureSuffix": prefsBentNormalsTextureSuffix = m_Service.vm.bentNormalsTextureSuffix; break;
                case "ambientOcclusionTextureSuffix":  prefsAmbientOcclusionTextureSuffix = m_Service.vm.ambientOcclusionTextureSuffix; break;
                case "positionsTextureSuffix": prefsPositionsTextureSuffix = m_Service.vm.positionsTextureSuffix; break;
                case "maskTextureSuffix": prefsMaskTextureSuffix = m_Service.vm.maskTextureSuffix; break;
            }
        }
    }
}
