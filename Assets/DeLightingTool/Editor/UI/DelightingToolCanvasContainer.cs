using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolCanvasContainer : IMGUIVisualContainer, IDisposable
    {
        static class Content
        {
            internal static readonly GUIContent dropZoneLabel = new GUIContent("Drop a folder containing the baked textures here");
        }

        static readonly int kCanvasHash = "CanvasHash".GetHashCode();
        static readonly int kDropZoneHash = "DropZoneHash".GetHashCode();

        DelightingToolCanvasSeparatorContainer m_Separator = new DelightingToolCanvasSeparatorContainer();
        DelightingToolCanvasExposureGizmo m_Gizmo = new DelightingToolCanvasExposureGizmo();

        public DelightingToolCanvasContainer()
        {
            AddChild(m_Separator);
            AddChild(m_Gizmo);
        }

        public override void OnGUI()
        {
            var controlId = EditorGUIUtility.GetControlID(kCanvasHash, FocusType.Passive);
            var dropZoneId = EditorGUIUtility.GetControlID(kDropZoneHash, FocusType.Passive);

            var texture = GetValue(kPreviewTexture);

            var hasTexture = texture != null;
            var targetTexture = (Texture)texture ?? Texture2D.whiteTexture;

            Rect canvasRectViewport;
            if (hasTexture)
            {
                var width = texture != null ? texture.width : 256;
                var height = texture != null ? texture.height : 256;

                var cameraPosition = GetValue(kCameraPosition);
                var zoom = GetValue(kZoom);
                EditorGUIXLayout.Canvas(controlId, ref cameraPosition, ref zoom, null, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                canvasRectViewport = GUILayoutUtility.GetLastRect();
                SetValue(kCameraPosition, cameraPosition);
                SetValue(kZoom, zoom);

                GUI.BeginClip(canvasRectViewport);
                GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
                GUI.DrawTexture(EditorGUIX.GetCanvasDestinationRect(cameraPosition, zoom, width, height), targetTexture, ScaleMode.ScaleToFit);
                GL.sRGBWrite = false;

                if (Event.current.type == EventType.Repaint)
                {
                    m_Separator.OnGUI();
                    if (GetValue(kOverrideReferenceZone))
                        m_Gizmo.OnGUI();
                }
                else
                {
                    if (GetValue(kOverrideReferenceZone))
                        m_Gizmo.OnGUI();
                    m_Separator.OnGUI();
                }
                GUI.EndClip();

                var fitCanvasToWindow = GetValue(kFitCanvasToWindow);
                if (fitCanvasToWindow && Event.current.type == EventType.Repaint)
                {
                    EditorGUIX.CancelCanvasZoom(controlId);
                    var widthRatio = canvasRectViewport.width / (float)width;
                    var heightRatio = canvasRectViewport.height / (float)height;
                    var widthOffset = 0f;
                    var heightOffset = 0f;
                    if (widthRatio < heightRatio)
                    {
                        zoom = widthRatio;
                        heightOffset = (canvasRectViewport.height - zoom * height) * 0.5f;
                    }
                    else
                    {
                        zoom = heightRatio;
                        widthOffset = (canvasRectViewport.width - zoom * width) * 0.5f;
                    }
                    SetValue(kZoom, zoom);
                    SetValue(kCameraPosition, new Vector2(widthOffset, heightOffset));
                    SetValue(kFitCanvasToWindow, false);
                }
            }
            else
            {
                EditorGUIXLayout.DropZoneHint(Content.dropZoneLabel);
                canvasRectViewport = GUILayoutUtility.GetLastRect();
            }

            if (EditorGUIX.DropZone(dropZoneId, canvasRectViewport, CanAcceptCallback))
            {
                var objs = DragAndDrop.objectReferences;
                if (objs.Length > 0)
                {
                    var path = AssetDatabase.GetAssetPath(objs[0]);
                    if (AssetDatabase.IsValidFolder(path))
                    {
                        SetValue(kInputFolderPath, path);
                        ExecuteCommand(kCmdLoadInputFolder);
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            RemoveChild(m_Gizmo);
            m_Gizmo.Dispose();
            m_Gizmo = null;
            RemoveChild(m_Separator);
            m_Separator.Dispose();
            m_Separator = null;
        }

        static DragAndDropVisualMode CanAcceptCallback(Object[] objs, string[] strings)
        {
            if (objs.Length > 0)
            {
                var path = AssetDatabase.GetAssetPath(objs[0]);
                if (AssetDatabase.IsValidFolder(path))
                    return DragAndDropVisualMode.Generic;
            }

            return DragAndDropVisualMode.Rejected;
        }
    }
}
