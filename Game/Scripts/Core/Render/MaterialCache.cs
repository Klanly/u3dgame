using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Yifan.Core
{
    internal sealed class MaterialCache : Singleton<MaterialCache>
    {
        private struct CacheKey
        {
            public int MaterialID;
            public int RenderQueue;
            public ShaderKeywords Keywords;
        }

        private Dictionary<CacheKey, Material> cache = new Dictionary<CacheKey, Material>(new CacheKeyComparer());

        public MaterialCache()
        {
#if UNITY_EDITOR
            EditorApplication.playmodeStateChanged += () =>
            {
                if (!EditorApplication.isPaused)
                {
                    this.cache.Clear();
                }
            };
#endif
        }

        internal Material GetMaterial(
            Material origin,
            int renderQueue,
            ShaderKeywords keywords)
        {
            CacheKey key;
            key.MaterialID = origin.GetInstanceID();
            key.RenderQueue = renderQueue;
            key.Keywords = keywords;

            Material material;
            if (this.cache.TryGetValue(key, out material))
            {
                if (material != null)
                {
                    return material;
                }

                this.cache.Remove(key);
            }

            material = new Material(origin);
            material.hideFlags = HideFlags.DontSave;
            if (renderQueue != -1)
            {
                material.renderQueue = (int)renderQueue;
            }

            foreach (var k in key.Keywords)
            {
                string keyword = ShaderKeywords.GetKeywordName(k);
                material.EnableKeyword(keyword);
#if UNITY_EDITOR
                material.name += "[" + keyword + "]";
#endif
            }

            this.cache.Add(key, material);

            return material;
        }

#if UNITY_EDITOR
        private static void ClearCache()
        {
            foreach (var kv in Instance.cache)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(kv.Value);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(kv.Value);
                }
            }

            Instance.cache.Clear();
            SceneView.RepaintAll();
        }
#endif

        private class CacheKeyComparer : IEqualityComparer<CacheKey>
        {
            public bool Equals(CacheKey x, CacheKey y)
            {
                if (x.MaterialID != y.MaterialID)
                {
                    return false;
                }

                if (x.RenderQueue != y.RenderQueue)
                {
                    return false;
                }

                if (!x.Keywords.Equals(y.Keywords))
                {
                    return false;
                }

                return true;
            }

            public int GetHashCode(CacheKey obj)
            {
                int hash = obj.MaterialID.GetHashCode();
                hash = (397 * hash) ^ ((int)obj.RenderQueue).GetHashCode();
                hash = (397 * hash) ^ obj.Keywords.GetHashCode();
                return hash;
            }
        }
    }
}