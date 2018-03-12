using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Core
{
    [RequireComponent(typeof(Camera))]
    class VerticalFog : MonoBehaviour
    {
        private static int? verticalFogColorKey;
        private static int? verticalFogParamKey;

        [SerializeField]
        private Color color = new Color(0.35f, 0.35f, 0.65f, 1.0f);

        [SerializeField]
        private float density = 0.5f;

        [SerializeField]
        private float startHeight = 0;

        [SerializeField]
        private float endHeight = -5;

        public static int VerticalFogColorKey
        {
            get
            {
                if (!verticalFogColorKey.HasValue)
                {
                    verticalFogColorKey = Shader.PropertyToID("_VerticalFogColor");
                }

                return verticalFogColorKey.Value;
            }
        }

        public static int VerticalFogParamKey
        {
            get
            {
                if (!verticalFogParamKey.HasValue)
                {
                    verticalFogParamKey = Shader.PropertyToID("_VerticalFogParam");
                }

                return verticalFogParamKey.Value;
            }
        }

        private void OnPreRender()
        {
            if (this.enabled)
            {
                this.UpdateShaderSetting();
            }
        }

        private void OnPostRender()
        {
            Shader.DisableKeyword("ENABLE_VERTICAL_FOG");
        }

        private void UpdateShaderSetting()
        {
            Shader.EnableKeyword("ENABLE_VERTICAL_FOG");
            Shader.SetGlobalColor(VerticalFogColorKey, this.color);
            var mistParam = new Vector4(this.density, this.startHeight, this.endHeight, 0.0f);
            Shader.SetGlobalVector(VerticalFogParamKey, mistParam);
        }
    }
}
