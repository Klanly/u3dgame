using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Yifan.Core
{
    [RequireComponent(typeof(Camera))]
    class CameraEnvLighting : MonoBehaviour
    {
        [SerializeField]
        private AmbientMode ambientMode2 = AmbientMode.Trilight;

        [SerializeField]
        private float ambientIntensity = 1.0f;

        [SerializeField]
        private Color ambientLight = Color.gray;

        [SerializeField]
        private Color ambientSkyColor = Color.gray;

        [SerializeField]
        private Color ambientEquatorColor = Color.white;

        [SerializeField]
        private Color ambientGroundColor = Color.gray;

        [SerializeField]
        private Cubemap customReflection;

        [SerializeField]
        private bool fog = true;

        private AmbientMode originAmbientMode;
        private float originAmbientIntensity;
        private Color originAmbientLight;
        private Color originAmbientSkyColor;
        private Color originAmbientEquatorColor;
        private Color originAmbientGroundColorr;
        private Cubemap originCustomReflection;
        private bool originFog;

        private void OnPreRender()
        {
            this.originAmbientMode = RenderSettings.ambientMode;
            this.originAmbientIntensity = RenderSettings.ambientIntensity;
            this.originAmbientLight = RenderSettings.ambientLight;
            this.originAmbientSkyColor = RenderSettings.ambientSkyColor;
            this.originAmbientEquatorColor = RenderSettings.ambientEquatorColor;
            this.originAmbientGroundColorr = RenderSettings.ambientGroundColor;
            this.originCustomReflection = RenderSettings.customReflection;
            this.originFog = RenderSettings.fog;

            RenderSettings.ambientMode = this.ambientMode2;
            RenderSettings.ambientIntensity = this.ambientIntensity;
            RenderSettings.ambientLight = this.ambientLight;
            RenderSettings.ambientSkyColor = this.ambientSkyColor;
            RenderSettings.ambientEquatorColor = this.ambientEquatorColor;
            RenderSettings.ambientGroundColor = this.ambientGroundColor;
            RenderSettings.customReflection = this.customReflection;
            RenderSettings.fog = this.fog;
        }

        private void OnPostRender()
        {
            RenderSettings.ambientMode = this.originAmbientMode;
            RenderSettings.ambientIntensity = this.originAmbientIntensity;
            RenderSettings.ambientLight = this.originAmbientLight;
            RenderSettings.ambientSkyColor = this.originAmbientSkyColor;
            RenderSettings.ambientEquatorColor = this.originAmbientEquatorColor;
            RenderSettings.ambientGroundColor = this.originAmbientGroundColorr;
            RenderSettings.customReflection = this.originCustomReflection;
            RenderSettings.fog = this.originFog;
        }
    }
}
