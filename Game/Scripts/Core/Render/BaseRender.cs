using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Yifan.Core
{
    [RequireComponent(typeof(Renderer))]
    public class BaseRender : MonoBehaviour
    {
        [SerializeField]
        private Material[] materials;

        [SerializeField]
        private int renderQueue = -1;

        private ShaderKeywords keywords;

        private MaterialPropertyBlock propertyBlock;

        private Material[] appliedMaterials;

        private Renderer unityRenderer;

        public Material[] Materials
        {
            get { return this.materials; }
            set { this.materials = value; }
        }

        public MaterialPropertyBlock PropertyBlock
        {
            get
            {
                if (null == this.propertyBlock)
                {
                    this.propertyBlock = new MaterialPropertyBlock();
                }

                return this.propertyBlock;
            }
        }

        private Material[] AppliedMaterials
        {
            get
            {
                if (this.appliedMaterials == null || this.appliedMaterials.Length != this.materials.Length)
                {
                    this.appliedMaterials = new Material[this.materials.Length];
                }

                return this.appliedMaterials;
            }
        }

        public Renderer UnityRenderer
        {
            get
            {
                if (this.unityRenderer == null)
                {
                    this.unityRenderer = this.GetComponent<Renderer>();
                }

                return this.unityRenderer;
            }
        }

        public void SetKeyword(int keyword)
        {
            this.keywords.SetKeyword(keyword);
        }

        public void UnsetKeyword(int keyword)
        {
            this.keywords.UnsetKeyword(keyword);
        }

        private void Awake()
        {
            this.OnWillRenderObject();
        }

        private void OnWillRenderObject()
        {
            if (this.materials == null)
            {
                return;
            }

            ShaderKeywords usedKeyword = this.keywords;

            var materialsApl = this.AppliedMaterials;
            for (int i = 0; i < this.materials.Length; ++i)
            {
                var mat = this.materials[i];
                if (mat != null)
                {
                    materialsApl[i] = MaterialCache.Instance.GetMaterial(mat, this.renderQueue, usedKeyword);
                }
                else
                {
                    materialsApl[i] = null;
                }
            }

            var unityRenderer = this.UnityRenderer;
            unityRenderer.sharedMaterials = materialsApl;
            if (this.propertyBlock != null)
            {
                unityRenderer.SetPropertyBlock(this.propertyBlock);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.OnWillRenderObject();
        }

        public void AddDefaultMaterial()
        {
            if (this.materials == null)
            {
                this.materials = new Material[0];
            }

            if (this.materials.Length == 0)
            {
                var renderer = this.GetComponent<Renderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    ArrayUtility.Add(ref this.materials, renderer.sharedMaterial);
                    return;
                }
            }

            ArrayUtility.Add(ref this.materials, null);
        }
#endif
    }
}
