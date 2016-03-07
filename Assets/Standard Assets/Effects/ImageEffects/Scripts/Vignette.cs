using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu ("Image Effects/Camera/Vignette")]
    public class Vignette : PostEffectsBase
    {
        public float intensity = 0.036f;
        public Shader vignetteShader;
        
        private Material m_VignetteMaterial;

        public override bool CheckResources()
        {
            CheckSupport(false);

            m_VignetteMaterial = CheckShaderAndCreateMaterial(vignetteShader, m_VignetteMaterial);

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }


        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if ( CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }
            RenderTexture color = null;
            m_VignetteMaterial.SetFloat("_Intensity", (1.0f / (1.0f - intensity) - 1.0f));		// intensity for vignette
            Graphics.Blit(source, color, m_VignetteMaterial, 0);
        }
    }
}
