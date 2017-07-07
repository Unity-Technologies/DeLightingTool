using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolCanvasExposureGizmo : IMGUIVisualContainer
    {
        static class Styles
        {
            internal const float kPositionSize = 14;
            internal const float kPositionThreshold = 10;
            internal const float kRadiusThreshold = 4;
        }

        static readonly int kCenterGizmoHash = "CenterGizmoHash".GetHashCode();
        static readonly int kRadiusGizmoHash = "RadiusGizmoHash".GetHashCode();

        public override void OnGUI()
        {
            var positionId = EditorGUIUtility.GetControlID(kCenterGizmoHash, FocusType.Passive);
            var radiusId = EditorGUIUtility.GetControlID(kRadiusGizmoHash, FocusType.Passive);

            var position = GetValue(kExposureGizmoPosition);
            var radius = GetValue(kExposureGizmoRadius);

            var cameraPosition = GetValue(kCameraPosition);
            var zoom = GetValue(kZoom);

            var actualPosition = position * zoom + cameraPosition;
            var actualRadius = radius * zoom;

            Handles.color = Handles.xAxisColor;
            actualPosition = EditorGUIX.PositionHandle2D(positionId, actualPosition, Styles.kPositionSize);
            Handles.color = Handles.yAxisColor;
            actualRadius = EditorGUIX.RadiusHandle2D(radiusId, actualPosition, actualRadius);

            var newPosition = (actualPosition - cameraPosition) / zoom;
            var newRadius = actualRadius / zoom;
            if ((position - newPosition).sqrMagnitude > Styles.kPositionThreshold
                || Mathf.Abs(newRadius - radius) > Styles.kRadiusThreshold)
            {
                SetValue(kExposureGizmoPosition, newPosition);
                SetValue(kExposureGizmoRadius, newRadius);
                ExecuteCommand(kCmdProcessFromColorCorrection);
            }
        }
    }
}
