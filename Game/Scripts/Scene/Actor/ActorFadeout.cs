using System;
using System.Collections.Generic;
using UnityEngine;
using Yifan.Core;

namespace Yifan.Scene
{
    class ActorFadeout : MonoBehaviour
    {
        [Serializable]
        private class RenderItem
        {
            public BaseRender Renderer;

            public Material[] Materials;

            [NonSerialized]
            public Material[] Origins;

            [NonSerialized]
            public GameObject Effect;

            [NonSerialized]
            public ParticleSystem[] Particles;
        }

        [SerializeField]
        private RenderItem[] items;

        [SerializeField]
        private GameObject effect;

        private float fadeout = -1.0f;
        private float fadeoutTotal = -1.0f;
        private Action fadeoutCallback;

        private void Awake()
        {
            foreach (var i in this.items)
            {
                i.Origins = i.Renderer.Materials;
            }
        }
        
        public void Fadeout(float time, Action callback)
        {
            this.fadeout = time;
            this.fadeoutTotal = time;
            this.fadeoutCallback = callback;

            foreach (var i in this.items)
            {
                i.Renderer.Materials = i.Materials;

                if (this.effect != null)
                {
                    var skinned = i.Renderer.GetComponent<SkinnedMeshRenderer>();
                    if (skinned != null)
                    {
                        i.Effect = GameObject.Instantiate(effect);
                        i.Particles = i.Effect.GetComponentsInChildren<ParticleSystem>();
                        foreach (var ps in i.Particles)
                        {
                            var shape = ps.shape;
                            shape.skinnedMeshRenderer = skinned;
                        }
                    }
                }
            }
        }

        private void ClearCache()
        {
            foreach (var i in this.items)
            {
                i.Renderer.Materials = i.Origins;
                i.Renderer.PropertyBlock.SetColor(ShaderProperty.MainColor, Color.white);
                if (i.Particles != null)
                {
                    foreach (var ps in i.Particles)
                    {
                        if (null != ps)
                        {
                            ps.Stop();
                        }
                    }

                    if (null != i.Effect)
                    {
                        GameObject.Destroy(i.Effect, 0.5f);
                    }

                    i.Particles = null;
                }
            }
        }

        private void Update()
        {
            if (this.fadeout > 0.0f)
            {
                float value = this.fadeout / this.fadeoutTotal;
                foreach (var i in this.items)
                {
                    i.Renderer.PropertyBlock.SetColor(
                        ShaderProperty.MainColor,
                        new Color(1, 1, 1, value));
                }

                this.fadeout -= Time.deltaTime;
                if (this.fadeout < 0)
                {
                    this.ClearCache();

                    this.fadeout = -1.0f;
                    this.fadeoutTotal = -1.0f;
                    if (this.fadeoutCallback != null)
                    {
                        this.fadeoutCallback();
                        this.fadeoutCallback = null;
                    }
                }
            }
        }
    }
}
