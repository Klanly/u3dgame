using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Core
{
    class UI3DDisplayCamera : MonoBehaviour
    {
        public GameObject DisplayObject;

        private static void SetVisible(Transform transform, bool enabled)
        {
            var renderer = transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = enabled;
            }

            for (int i = 0; i < transform.childCount; ++i)
            {
                var child = transform.GetChild(i);
                SetVisible(child, enabled);
            }
        }

        private void OnPreCull()
        {
            if (this.DisplayObject != null)
            {
                SetVisible(this.DisplayObject.transform, true);
            }
        }

        private void OnPostRender()
        {
            if (this.DisplayObject != null)
            {
                SetVisible(this.DisplayObject.transform, false);
            }
        }
    }
}
