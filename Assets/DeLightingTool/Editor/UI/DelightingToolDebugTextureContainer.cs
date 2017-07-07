using System;
using UnityEditor.Experimental.VisualElements;
using UnityEngine;

namespace UnityEditor.Experimental.DelightingInternal
{
    partial class DelightingToolDebugTextureContainer : IMGUIVisualContainer, IDisposable
    {
        static class Content
        {
            internal static readonly GUIContent kLatLongALabel = new GUIContent("Main Env Map");
            internal static readonly GUIContent kBakedLUTLabel = new GUIContent("Baked LUT");
            internal static readonly GUIContent kLatLongExposureLabel = new GUIContent("Main Env Map Exposure");
            internal static readonly GUIContent kExportLabel = new GUIContent("Export");
        }

        RenderTexture m_latLongExposed = null;

        public override void OnGUI()
        {
            SetValue(kLatLongExposure, EditorGUILayout.Slider(Content.kLatLongExposureLabel, GetValue(kLatLongExposure), 0, 1));

            var latLongA = GetValue(kLatLongA);
            if (latLongA != null)
            {
                var mat = DelightingService.GetLatLongMaterial(
                    latLongA,
                    GetValue(kNormalsTexture),
                    GetValue(kLatLongAverage),
                    GetValue(kOverrideReferenceZone),
                    GetValue(kLatLongExposure),
                    GetValue(kSafetyZoneParams),
                    false);

                var oldRt = RenderTexture.active;
                m_latLongExposed = DelightingHelpers.InstantiateRTIfRequired(m_latLongExposed, latLongA.width, latLongA.height, false, TextureWrapMode.Clamp);
                Graphics.Blit(null, m_latLongExposed, mat);
                RenderTexture.active = oldRt;

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Content.kLatLongALabel, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Content.kExportLabel))
                    ExecuteCommand(kCmdExportMainEnvMap);
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                var rect = GUILayoutUtility.GetAspectRect(latLongA.width / (float)latLongA.height);
                GUI.DrawTexture(rect, m_latLongExposed);
                EditorGUILayout.Space();
            }




            var bakedLut = GetValue(kBakedLUT);
            if (bakedLut != null)
            {

                EditorGUILayout.Space();

                EditorGUILayout.LabelField(Content.kBakedLUTLabel, EditorStyles.boldLabel);

                EditorGUILayout.Space();

                var rect = GUILayoutUtility.GetAspectRect(bakedLut.width / (float)bakedLut.height);
                GUI.DrawTexture(rect, bakedLut);
                EditorGUILayout.Space();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            DelightingHelpers.DeleteRTIfRequired(m_latLongExposed);
            m_latLongExposed = null;
        }
    }
}
