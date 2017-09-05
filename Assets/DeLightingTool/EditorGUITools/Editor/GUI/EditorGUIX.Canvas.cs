using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class EditorGUIXLayout
    {
        public static void Canvas(int controlId, ref Vector2 cameraPosition, ref float zoom, float[] zoomLevels = null, params GUILayoutOption[] options)
        {
            var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, options);
            EditorGUIX.Canvas(controlId, rect, ref cameraPosition, ref zoom, zoomLevels);
        }
    }

    public static partial class EditorGUIX
    {
        struct CanvasData
        {
            internal float zoomTarget;
            internal Vector2 cameraPositionTarget;
            internal bool panning;
            internal bool zooming;
            internal float lastUpdateTime;
        }

        public static partial class Styles
        {
            static readonly Color kProBackgroundColor = new Color(100f / 255f, 100f / 255f, 100f / 255f);
            static readonly Color kProGridMajorLineColor = new Color(80f / 255f, 80f / 255f, 80f / 255f);
            static readonly Color kProGridMinorLineColor = new Color(170f / 255f, 170f / 255f, 170f / 255f);

            static readonly Color kPersoBackgroundColor = new Color(194f / 255f, 194f / 255f, 194f / 255f);
            static readonly Color kPersoGridMajorLineColor = new Color(125f / 255f, 125f / 255f, 125f / 255f);
            static readonly Color kPersoGridMinorLineColor = new Color(85f / 255f, 85f / 255f, 85f / 255f);

            internal static Color canvasGridBackgroundColor { get { return EditorGUIUtility.isProSkin ? kProBackgroundColor : kPersoBackgroundColor; } }
            internal static Color canvasGridMajorLineColor { get { return EditorGUIUtility.isProSkin ? kProGridMajorLineColor : kPersoGridMajorLineColor; } }
            internal static Color canvasGridMinorLineColor { get { return EditorGUIUtility.isProSkin ? kProGridMinorLineColor : kPersoGridMinorLineColor; } }

            public const float canvasGridMinorSize = 64f;
            public const float canvasGridMajorSize = 256f;
        }

        static Material s_GridMaterial = null;
        static Material gridMaterial { get { return s_GridMaterial ?? (s_GridMaterial = new Material(Shader.Find("Hidden/EditorGUITools/Grid")) { hideFlags = HideFlags.HideAndDontSave }); } }
        static Dictionary<int, CanvasData> s_CanvasDatas = new Dictionary<int, CanvasData>();

        static readonly float[] kZoomScrollLevels = { 1 / 16f, 1 / 8f, 1 / 4f, 1 / 2f, 1, 2, 4, 8, 16 };
        const float kLerpSpeed = 0.5f;

        public static Rect GetCanvasDestinationRect(Vector2 cameraPosition, float zoom, float width, float height)
        {
            return new Rect(cameraPosition.x, cameraPosition.y, zoom * width, zoom * height);
        }

        public static void CancelCanvasZoom(int controlId)
        {
            if (s_CanvasDatas.ContainsKey(controlId))
            {
                var canvasData = s_CanvasDatas[controlId];
                canvasData.zooming = false;
                s_CanvasDatas[controlId] = canvasData;
            }
        }

        public static void Canvas(int controlId, Rect rect, ref Vector2 cameraPosition, ref float zoom, float[] zoomLevels = null)
        {
            zoomLevels = zoomLevels ?? kZoomScrollLevels;
            CanvasData canvasData;
            if (!s_CanvasDatas.TryGetValue(controlId, out canvasData))
            {
                canvasData = new CanvasData { cameraPositionTarget = cameraPosition, zoomTarget = zoom };
                s_CanvasDatas[controlId] = canvasData;
            }

            var evt = Event.current;
            switch (evt.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (evt.button == 2 && rect.Contains(evt.mousePosition))
                    {
                        EditorGUIUtility.hotControl = controlId;
                        EditorGUIUtility.keyboardControl = 0;
                        canvasData.panning = true;
                        canvasData.zooming = false;
                        evt.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (evt.button == 2 && EditorGUIUtility.hotControl == controlId)
                    {
                        cameraPosition += evt.delta;
                        evt.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (evt.button == 2 && EditorGUIUtility.hotControl == controlId)
                    {
                        EditorGUIUtility.hotControl = 0;
                        canvasData.panning = false;
                        canvasData.zooming = false;
                        evt.Use();
                    }
                    break;
                case EventType.ScrollWheel:
                    {
                        canvasData.zooming = true;
                        var oldZoomTarget = canvasData.zoomTarget;
                        canvasData.zoomTarget = IncrementZoomLevel(canvasData.zoomTarget, evt.delta.y > 0 ? -1 : 1, zoomLevels);

                        if (!Mathf.Approximately(canvasData.zoomTarget, oldZoomTarget))
                        {
                            var ratio = canvasData.zoomTarget / oldZoomTarget;
                            var mousePositionRectSpace = evt.mousePosition - rect.position;
                            var mousePositionCanvasSpace = mousePositionRectSpace - canvasData.cameraPositionTarget;
                            canvasData.cameraPositionTarget += mousePositionCanvasSpace * (1 - ratio);
                        }
                        evt.Use();
                        break;
                    }
                case EventType.Repaint:
                    {
                        DrawGrid(rect, cameraPosition, zoom);
                        break;
                    }
            }

            if (canvasData.zooming)
            {
                if (Mathf.Abs(canvasData.zoomTarget - zoom) < 0.0001f)
                {
                    zoom = canvasData.zoomTarget;
                    cameraPosition = canvasData.cameraPositionTarget;
                    canvasData.zooming = false;
                }
                else
                {
                    var deltaTime = Mathf.Min(Time.realtimeSinceStartup - canvasData.lastUpdateTime, 0.1f);
                    zoom = Mathf.Lerp(zoom, canvasData.zoomTarget, deltaTime * kLerpSpeed);
                    cameraPosition = Vector2.Lerp(cameraPosition, canvasData.cameraPositionTarget, deltaTime * kLerpSpeed);
                }
            }
            else
            {
                canvasData.zoomTarget = zoom;
                canvasData.cameraPositionTarget = cameraPosition;
            }

            s_CanvasDatas[controlId] = canvasData;
        }

        static void DrawGrid(Rect rect, Vector2 cameraPosition, float zoom)
        {
            cameraPosition *= -1;
            cameraPosition -= rect.position;
            // TODO: Should convert camera position to screen space instead
            cameraPosition.y -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            gridMaterial.SetPass(0);
            gridMaterial.SetColor("_BackgroundColor", Styles.canvasGridBackgroundColor);
            gridMaterial.SetColor("_GridMajorColor", Styles.canvasGridMajorLineColor);
            gridMaterial.SetColor("_GridMinorColor", Styles.canvasGridMinorLineColor);
            gridMaterial.SetFloat("_GridMajorSize", Styles.canvasGridMajorSize);
            gridMaterial.SetFloat("_GridMinorSize", Styles.canvasGridMinorSize);
            gridMaterial.SetFloat("_Zoom", zoom);
            gridMaterial.SetFloat("_CameraX", cameraPosition.x);
            gridMaterial.SetFloat("_CameraY", cameraPosition.y);

            Graphics.DrawTexture(rect, Texture2D.whiteTexture, gridMaterial);
        }

        static float IncrementZoomLevel(float zoom, int increment, float[] zoomLevels)
        {
            var level = 0;
            if (zoom >= zoomLevels[kZoomScrollLevels.Length - 1])
                level = zoomLevels.Length - 1;
            else
            {
                for (int i = 1; i < zoomLevels.Length; ++i)
                {
                    if (zoom <= zoomLevels[i])
                    {
                        level = i;
                        break;
                    }
                }
            }

            level = Mathf.Clamp(level + increment, 0, zoomLevels.Length - 1);

            return zoomLevels[level];
        }
    }
}
