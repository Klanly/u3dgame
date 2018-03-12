using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yifan.Core
{
    class UIImageMirror : BaseMeshEffect, ILayoutElement
    {
        public enum MirrorModeType
        {
            Horizontal,
            Vertical,
            Quad,
        }

        [SerializeField]
        [Tooltip("The mirror type.")]
        private MirrorModeType mirrorMode = MirrorModeType.Horizontal;

        private Image image;
        private Vector2 xy;

        private static List<UIVertex> vertex_stream = new List<UIVertex>();

        public float flexibleHeight
        {
            get { return -1; }
        }

        public float flexibleWidth
        {
            get { return -1; }
        }

        public int layoutPriority
        {
            get { return 0; }
        }

        public float minHeight
        {
            get { return 0; }
        }

        public float minWidth
        {
            get { return 0; }
        }

        public float preferredHeight
        {
            get
            {
                Image image = this.GetImage();
                if (null == image)
                {
                    return 0;
                }

                switch (this.mirrorMode)
                {
                    case MirrorModeType.Vertical:
                    case MirrorModeType.Quad:
                        return 2 * image.preferredHeight;
                    default:
                        return image.preferredHeight;
                }
            }
        }

        public float preferredWidth
        {
            get
            {
                Image image = this.GetImage();
                if (null == image)
                {
                    return 0;
                }

                switch (this.mirrorMode)
                {
                    case MirrorModeType.Horizontal:
                    case MirrorModeType.Quad:
                        return 2 * image.preferredWidth;
                    default:
                        return image.preferredWidth;
                }
            }
        }

        public void CalculateLayoutInputHorizontal()
        {
        }

        public void CalculateLayoutInputVertical()
        {
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive() || vh.currentVertCount <= 0)
            {
                return;
            }

            RectTransform rectTransform = this.transform as RectTransform;
            this.xy = rectTransform.rect.size;

            vertex_stream.Clear();
            vh.GetUIVertexStream(vertex_stream);
            var newVerts = this.Modify(vertex_stream);
            if (newVerts != null)
            {
                vh.Clear();
                vh.AddUIVertexTriangleStream(newVerts);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var rectTransform = this.transform as RectTransform;
            this.xy = rectTransform.sizeDelta;
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }

        private Image GetImage()
        {
            if (this.image == null)
            {
                this.image = this.GetComponent<Image>();
            }

            return this.image;
        }

        private List<UIVertex> Modify(List<UIVertex> verts)
        {
            Image image = this.GetImage();
            if (Image.Type.Simple == image.type)
            {
                return this.ModifySimple(verts);
            }

            return verts;
        }

        private List<UIVertex> ModifySimple(List<UIVertex> verts)
        {
            int start = 0;
            int end = verts.Count;
            if (MirrorModeType.Horizontal == this.mirrorMode)
            {
                var neededCpacity = verts.Count * 2;
                if (verts.Capacity < neededCpacity)
                {
                    verts.Capacity = neededCpacity;
                }

                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    0,
                    0,
                    -this.xy.x / 2,
                    0);
                start = end;
                end = verts.Count;
                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    this.xy.x,
                    0,
                    -this.xy.x / 2,
                    0,
                    true);
            }
            else if (this.mirrorMode == MirrorModeType.Vertical)
            {
                var neededCpacity = verts.Count * 2;
                if (verts.Capacity < neededCpacity)
                {
                    verts.Capacity = neededCpacity;
                }

                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    0,
                    0,
                    0,
                    this.xy.y / 2);
                start = end;
                end = verts.Count;
                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    0,
                    -this.xy.y,
                    0,
                    this.xy.y / 2,
                    true);
            }
            else
            {
                var neededCpacity = verts.Count * 4;
                if (verts.Capacity < neededCpacity)
                {
                    verts.Capacity = neededCpacity;
                }

                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    0,
                    0,
                    -this.xy.x / 2,
                    this.xy.y / 2);
                start = end;
                end = verts.Count;
                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    this.xy.x,
                    0,
                    -this.xy.x / 2,
                    this.xy.y / 2);
                start = end;
                end = verts.Count;
                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    0,
                    -this.xy.y,
                    -this.xy.x / 2,
                    this.xy.y / 2);
                start = end;
                end = verts.Count;
                this.ApplyMirrorDouble(
                    verts,
                    start,
                    end,
                    this.xy.x,
                    -this.xy.y,
                    -this.xy.x / 2,
                    this.xy.y / 2,
                    true);
            }

            return verts;
        }

        private void ApplyMirrorDouble(
            List<UIVertex> verts,
            int start,
            int end,
            float x1,
            float y1,
            float x2,
            float y2,
            bool self = false)
        {
            UIVertex vt;

            var neededCpacity = verts.Count * 2;
            if (verts.Capacity < neededCpacity)
            {
                verts.Capacity = neededCpacity;
            }

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                if (!self)
                {
                    verts.Add(vt); // add more mesh
                }

                var v = vt.position;
                int offset = i % 6;
                switch (offset)
                {
                    case 0:
                    case 1:
                    case 5:
                        v.x += x1;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        v.x += x2;
                        break;
                }

                switch (offset)
                {
                    case 1:
                    case 2:
                    case 3:
                        v.y += y1;
                        break;
                    case 0:
                    case 4:
                    case 5:
                        v.y += y2;
                        break;
                }

                vt.position = v;
                verts[i] = vt;
            }
        }

#if UNITY_EDITOR
        /// <inheritdoc/>
        protected override void OnValidate()
        {
            base.OnValidate();
            var rectTransform = this.transform as RectTransform;
            this.xy = rectTransform.sizeDelta;
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }
#endif
    }
}
