using System;
using System.Collections.Generic;
using Yifan.Core;
using UnityEngine;

namespace Yifan.Scene
{
    class ActorBlinker : MonoBehaviour
    {
        [SerializeField]
        private float fadeIn = 0.01f;

        [SerializeField]
        private float fadeHold = 0.05f;

        [SerializeField]
        private float fadeOut = 0.25f;

        private float blinkFadeIn = -1.0f;
        private float blinkFadeInTotal = -1.0f;
        private float blinkFadeHold = -1.0f;
        private float blinkFadeOut = -1.0f;
        private float blinkFadeOutTotal = -1.0f;
        private List<BaseRender> renderers = new List<BaseRender>();

        public void Blink()
        {
            this.blinkFadeIn = this.fadeIn;
            this.blinkFadeInTotal = this.fadeIn;
            this.blinkFadeHold = this.fadeHold;
            this.blinkFadeOut = this.fadeOut;
            this.blinkFadeOutTotal = this.fadeOut;

            foreach (var renderer in this.renderers)
            {
                renderer.UnsetKeyword((int)ShaderKeyword.ENABLE_RIM);
            }

            this.GetComponentsInChildren(this.renderers);
            foreach (var renderer in this.renderers)
            {
                renderer.SetKeyword((int)ShaderKeyword.ENABLE_RIM);
            }
        }

        private void Update()
        {
            if (this.blinkFadeIn > 0.0f)
            {
                float value = 1 - (this.blinkFadeIn / this.blinkFadeInTotal);
                foreach (var renderer in this.renderers)
                {
                    renderer.PropertyBlock.SetFloat(ShaderProperty.RimIntensity, 3.5f * value);
                }

                this.blinkFadeIn -= Time.deltaTime;
            }
            else if (this.blinkFadeHold > 0.0f)
            {
                this.blinkFadeHold -= Time.deltaTime;
            }
            else if (this.blinkFadeOut > 0.0f)
            {
                float value = this.blinkFadeOut / this.blinkFadeOutTotal;
                foreach (var renderer in this.renderers)
                {
                    renderer.PropertyBlock.SetFloat(ShaderProperty.RimIntensity, 3.5f * value);
                }

                this.blinkFadeOut -= Time.deltaTime;
                if (this.blinkFadeOut <= 0.0f)
                {
                    foreach (var renderer in this.renderers)
                    {
                        renderer.UnsetKeyword((int)ShaderKeyword.ENABLE_RIM);
                    }

                    this.renderers.Clear();
                }
            }
        }
    }
}
