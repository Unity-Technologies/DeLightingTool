using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingViewModel : IDisposable
    {
        [Flags]
        enum PendingOperation
        {
            None = 0,
            LoadInputFolder = 1 << 0,
            Process = 1 << 1,
            RenderPreview = 1 << 2,
            FitPreviewToWindow = 1 << 3
        }

        static readonly DelightingUI.Display[] kModeDisplays =
        {
            DelightingUI.Display.Normal,
            DelightingUI.Display.Normal, 
            DelightingUI.Display.Normal, 
        };

        static DelightingViewModel()
        {
            // Normal
            kModeDisplays[0] |= DelightingUI.Display.SectionInput
                | DelightingUI.Display.FieldBaseTexture
                | DelightingUI.Display.FieldNormalsTexture
                | DelightingUI.Display.FieldBentNormalsTexture
                | DelightingUI.Display.FieldAmbientOcclusionTexture
                | DelightingUI.Display.FieldPositionTexture
                | DelightingUI.Display.FieldMaskTexture
                | DelightingUI.Display.ButtonAutoCompute
                | DelightingUI.Display.ButtonCompute
                | DelightingUI.Display.ButtonExport;

            // Advanced
            kModeDisplays[1] = kModeDisplays[0]
                | DelightingUI.Display.FieldSwitchYZAxes
                | DelightingUI.Display.SectionDelighting
                | DelightingUI.Display.SectionImport
                | DelightingUI.Display.FieldNoiseReduction
                | DelightingUI.Display.FieldRemoveHighlights
                | DelightingUI.Display.FieldRemoveDarkNoise
                | DelightingUI.Display.FieldSeparateDarkAreas
                | DelightingUI.Display.FieldForceLocalDelighting
                | DelightingUI.Display.FieldViewMode;

            // Debug
            kModeDisplays[2] = kModeDisplays[1]
                | DelightingUI.Display.SectionDebug;
        }

        static Dictionary<string, Delighting.ProcessStep> s_PropertiesTriggeringProcess = new Dictionary<string, Delighting.ProcessStep>()
        {
            { "autoCompute", Delighting.ProcessStep.Gather },
            { "baseTexture", Delighting.ProcessStep.Gather },
            { "normalsTexture", Delighting.ProcessStep.Gather },
            { "bentNormalsTexture", Delighting.ProcessStep.Gather },
            { "ambientOcclusionTexture", Delighting.ProcessStep.Gather },
            { "positionTexture", Delighting.ProcessStep.Gather },
            { "maskTexture", Delighting.ProcessStep.Gather },
            { "switchYZaxes", Delighting.ProcessStep.Gather },
            { "safetyZoneParams", Delighting.ProcessStep.ColorCorrection },
            { "removeHighlights", Delighting.ProcessStep.ColorCorrection },
            { "removeDarkNoise", Delighting.ProcessStep.ColorCorrection },
            { "overrideReferenceZone", Delighting.ProcessStep.ColorCorrection },
            { "forceLocalDelighting", Delighting.ProcessStep.Delight },
            { "separateDarkAreas", Delighting.ProcessStep.Gather },
        };

        PendingOperation pendingOperation
        {
            get { return m_PendingOperations; }
            set
            {
                m_PendingOperations = value;
                if (m_PendingOperations != PendingOperation.None && !m_UpdateRequested)
                {
                    m_UpdateRequested = true;
                    EditorApplication.delayCall += UpdatePendingOperations;
                }
            }
        }

        DelightingService m_Service = null;
        PendingOperation m_PendingOperations = PendingOperation.FitPreviewToWindow;
        bool m_UpdateRequested = false;
        AsyncOperationEnumerator<Delighting.ILoadAssetFolderOperation> m_LoadAssetFolderOperation = null;
        AsyncOperationEnumerator<Delighting.IProcessOperation> m_ProcessOperation = null;
        Delighting.ProcessStep m_FromProcessStep = Delighting.ProcessStep.None;
        bool m_FitToPreviewAfterProcess = false;

        public DelightingViewModel(DelightingService service)
        {
            m_Service = service;
            EditorApplication.update += Update;
            PropertyChanged += OnPropertyChanged;
        }

        public void Dispose()
        {
            EditorApplication.update -= Update;
            DelightingHelpers.DeleteRTIfRequired(latLongA);
            latLongA = null;
            DelightingHelpers.DeleteRTIfRequired(bakedLUT);
            bakedLUT = null;
            PropertyChanged -= OnPropertyChanged;
        }

        public void CmdResetExposureZone()
        {
            m_Service.ResetReferenceGizmo();
        }

        public void CmdExport()
        {
            m_Service.ExportDelightedTextureDialog();
        }

        public void CmdExportMainEnvMap()
        {
            m_Service.ExportMainEnvMapDialog();
        }

        public void CmdLoadInputFolder()
        {
            pendingOperation |= PendingOperation.LoadInputFolder;
        }

        public void CmdProcessFromGather()
        {
            SetProcessStep(Delighting.ProcessStep.Gather);
            pendingOperation |= PendingOperation.Process;
        }

        public void CmdProcessFromColorCorrection()
        {
            SetProcessStep(Delighting.ProcessStep.ColorCorrection);
            pendingOperation |= PendingOperation.Process;
        }

        public void CmdRenderPreview()
        {
            pendingOperation |= PendingOperation.RenderPreview;
        }

        static DelightingUI.Display GetDisplaysFor(DelightingUI.Mode mode)
        {
            return kModeDisplays[(int)mode];
        }

        void SetProcessStep(Delighting.ProcessStep fromStep)
        {
            if ((int)fromStep < (int)m_FromProcessStep && fromStep != Delighting.ProcessStep.None || m_FromProcessStep == Delighting.ProcessStep.None)
                m_FromProcessStep = fromStep;
        }

        void UpdatePendingOperations()
        {
            m_UpdateRequested = false;
            if ((m_PendingOperations & PendingOperation.LoadInputFolder) != 0)
            {
                m_PendingOperations &= ~PendingOperation.LoadInputFolder;
                if (m_LoadAssetFolderOperation != null)
                    Debug.LogWarning("A folder is already being loaded, ignoring request to load a folder");
                else
                {
                    m_LoadAssetFolderOperation = m_Service.LoadInputFolderAsync(inputFolderPath);
                    m_LoadAssetFolderOperation.OnProgress(progress =>
                    {
                        loadingShow = true;
                        loadingProgress = progress;
                        loadingContent = string.Format("Loading folder: {0:P2}", progress);
                    });
                    m_LoadAssetFolderOperation.OnComplete(result =>
                    {
                        m_Service.Reset();
                        m_Service.SetInput(result);

                        m_FitToPreviewAfterProcess = true;

                        SetProcessStep(Delighting.ProcessStep.Gather);
                        pendingOperation |= PendingOperation.Process;
                    });
                    m_LoadAssetFolderOperation.OnError(e =>
                    {
                        loadingShow = false;
                        Debug.LogException(e);
                    });
                }
            }
            if ((m_PendingOperations & PendingOperation.Process) != 0 && m_ProcessOperation == null)
            {
                m_PendingOperations &= ~PendingOperation.Process;
                if (m_FromProcessStep != Delighting.ProcessStep.None)
                {
                    m_ProcessOperation = m_Service.ProcessAsync(new Delighting.ProcessArgs { fromStep = m_FromProcessStep, calculateResult = false });
                    m_FromProcessStep = Delighting.ProcessStep.None;

                    m_ProcessOperation.OnProgress(progress =>
                    {
                        loadingShow = true;
                        loadingProgress = progress;
                        loadingContent = string.Format("De-Lighting: {0:P2}", progress);
                    });
                    m_ProcessOperation.OnComplete(result =>
                    {
                        loadingShow = false;
                        loadingProgress = 1;
                    });
                    m_ProcessOperation.OnError(e =>
                    {
                        loadingShow = false;
                        loadingProgress = 1;
                        Debug.LogException(e);
                    });
                }
            }
        }

        void Update()
        {
            if (m_LoadAssetFolderOperation != null && !m_LoadAssetFolderOperation.MoveNext())
                m_LoadAssetFolderOperation = null;

            if (m_ProcessOperation != null && !m_ProcessOperation.MoveNext())
            {
                if (m_ProcessOperation.error != null)
                {
                    m_PendingOperations &= ~PendingOperation.RenderPreview;

                    var errors = GetErrorMessagesFrom(((Delighting.ProcessException)m_ProcessOperation.error).errorCode);
                    for (int i = 0; i < errors.Count; i++)
                        Debug.LogError(errors[i]);
                }
                else
                {
                    if (m_FitToPreviewAfterProcess)
                    {
                        m_PendingOperations |= PendingOperation.FitPreviewToWindow;
                        m_FitToPreviewAfterProcess = false;
                    }
                    m_PendingOperations |= PendingOperation.RenderPreview;
                }
                m_ProcessOperation = null;
                // Trigger the process operation with current process step
                SetProcessStep(m_FromProcessStep);
            }

            if ((m_PendingOperations & PendingOperation.RenderPreview) != 0)
            {
                m_PendingOperations &= ~PendingOperation.RenderPreview;
                if (m_Service.ValidateInputs() == Delighting.ErrorCode.NoErrors)
                    m_Service.RenderPreview();
            }
            if ((m_PendingOperations & PendingOperation.FitPreviewToWindow) != 0)
            {
                m_PendingOperations &= ~PendingOperation.FitPreviewToWindow;
                fitCanvasToWindow = true;
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            Delighting.ProcessStep fromStep;
            if (autoCompute 
                && s_PropertiesTriggeringProcess.TryGetValue(propertyChangedEventArgs.PropertyName, out fromStep)
                && m_Service.ValidateInputs() == Delighting.ErrorCode.NoErrors)
            {
                SetProcessStep(fromStep);
                pendingOperation |= PendingOperation.Process;
            } 
        }

        List<string> GetErrorMessagesFrom(Delighting.ErrorCode errorCode)
        {
            int width = 0, height = 0;
            if (baseTexture != null)
            {
                width = baseTexture.width;
                height = baseTexture.height;
            }
            var errors = new List<string>();
            DelightingHelpers.GetErrorMessagesFrom(errorCode, errors, width, height);
            return errors;
        }
    }
}
