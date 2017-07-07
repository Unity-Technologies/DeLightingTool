using System;
using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class Delighting
    {
        public class Input : IInput
        {
            public bool hasBaseTexture { get; private set; }
            public Texture2D baseTexture { get; private set; }
            public bool hasNormalsTexture { get; private set; }
            public Texture2D normalsTexture { get; private set; }
            public bool hasBentNormalsTexture { get; private set; }
            public Texture2D bentNormalsTexture { get; private set; }
            public bool hasAmbientOcclusionTexture { get; private set; }
            public Texture2D ambientOcclusionTexture { get; private set; }
            public bool hasPositionTexture { get; private set; }
            public Texture2D positionTexture { get; private set; }
            public bool hasMaskTexture { get; private set; }
            public Texture2D maskTexture { get; private set; }
            public bool? switchYZ { get; private set; }
            public float? separateDarkAreas { get; private set; }
            public float? forceLocalDelighting { get; private set; }
            public float? removeHighlights { get; private set; }
            public float? removeDarkNoise { get; private set; }

            public Input SetBaseTexture(Texture2D value)
            {
                hasBaseTexture = true;
                baseTexture = value;
                return this;
            }
            public Input SetBaseTexture(string value)
            {
                SetTextureFromPath(value, SetBaseTexture);
                return this;
            }
            public Input SetNormalsTexture(Texture2D value)
            {
                hasNormalsTexture = true;
                normalsTexture = value;
                return this;
            }
            public Input SetNormalsTexture(string value)
            {
                SetTextureFromPath(value, SetNormalsTexture);
                return this;
            }
            public Input SetBentNormalsTexture(Texture2D value)
            {
                hasBentNormalsTexture = true;
                bentNormalsTexture = value;
                return this;
            }
            public Input SetBentNormalsTexture(string value)
            {
                SetTextureFromPath(value, SetBentNormalsTexture);
                return this;
            }
            public Input SetAmbientOcclusionTexture(Texture2D value)
            {
                hasAmbientOcclusionTexture = true;
                ambientOcclusionTexture = value;
                return this;
            }
            public Input SetAmbientOcclusionTexture(string value)
            {
                SetTextureFromPath(value, SetAmbientOcclusionTexture);
                return this;
            }
            public Input SetPositionTexture(Texture2D value)
            {
                hasPositionTexture = true;
                positionTexture = value;
                return this;
            }
            public Input SetPositionTexture(string value)
            {
                SetTextureFromPath(value, SetPositionTexture);
                return this;
            }
            public Input SetMaskTexture(Texture2D value)
            {
                hasMaskTexture = true;
                maskTexture = value;
                return this;
            }
            public Input SetMaskTexture(string value)
            {
                SetTextureFromPath(value, SetMaskTexture);
                return this;
            }
            public Input SetSwitchYZ(bool value)
            {
                switchYZ = value;
                return this;
            }

            public Input SetSwitchYZ(bool? value)
            {
                if (value.HasValue)
                    SetSwitchYZ(value.Value);
                return this;
            }
            public Input SetNoiseReduction(float? value)
            {
                if (value.HasValue)
                    SetNoiseReduction(value.Value);
                return this;
            }
            public Input SetRemoveHighlights(float value)
            {
                removeHighlights = value;
                return this;
            }
            public Input SetRemoveHighlights(float? value)
            {
                if (value.HasValue)
                    SetRemoveHighlights(value.Value);
                return this;
            }
            public Input SetRemoveDarkNoise(float value)
            {
                removeDarkNoise = value;
                return this;
            }
            public Input SetRemoveDarkNoise(float? value)
            {
                if (value.HasValue)
                    SetRemoveDarkNoise(value.Value);
                return this;
            }
            public Input SetForceLocalDelighting(float? value)
            {
                if (value.HasValue)
                    SetForceLocalDelighting(value.Value);

                return this;
            }
            public Input SetForceLocalDelighting(float value)
            {
                forceLocalDelighting = value;
                return this;
            }
            public Input SetSeparateDarkAreas(float? value)
            {
                if (value.HasValue)
                    SetSeparateDarkAreas(value.Value);
                return this;
            }
            public Input SetSeparateDarkAreas(float value)
            {
                separateDarkAreas = value;
                return this;
            }

            static void SetTextureFromPath(string path, Func<Texture2D, Input> setter)
            {
                if (string.IsNullOrEmpty(path))
                    return;

                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex == null)
                    return;

                setter(tex);
                return;
            }
        }
    }
}
