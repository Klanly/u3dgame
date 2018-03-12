using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Yifan.Core
{
    public class UI3DDisplay : MonoBehaviour
    {
        [SerializeField]
        private RawImage displayImage;

        [SerializeField]
        private Camera displayCamera;

        [SerializeField]
        private GameObject displayObject;

        [SerializeField]
        [Layer]
        [Tooltip("The display layer.")]
        private int displayLayer = 0;

        [SerializeField]
        private int resolutionX = 100;

        [SerializeField]
        private int resolutionY = 100;

        [SerializeField]
        private Vector3 displayOffset = Vector3.zero;

        [SerializeField]
        private Vector3 displayRotation = Vector3.zero;

        [SerializeField]
        private Vector3 displayScale = Vector3.one;

        [SerializeField]
        private float dragSpeed = 10.0f;

        private UI3DDisplayCamera displayCameraCtrl;
        private RenderTexture displayTexture;
        private float dragRotation;

        public void Display()
        {
            if (null == this.displayObject)
            {
                return;
            }

            Transform ui_transform = this.displayObject.transform.Find("UICamera");
            if (null == ui_transform)
            {
                return;
            }

            this.Display(this.displayObject, ui_transform.GetComponent<Camera>());
        }

        public void Display(GameObject display_obj, Camera look_camera = null)
        {
            if (null == display_obj || null == this.displayCameraCtrl)
            {
                return;
            }

            this.displayObject = display_obj;

            if (null == this.displayTexture)
            {
                this.displayTexture = RenderTexture.GetTemporary(this.resolutionX, this.resolutionY, 32);
            }

            this.displayImage.texture = this.displayTexture;
            this.displayCamera.targetTexture = this.displayTexture;

            this.displayCamera.cullingMask = 1 << this.displayLayer;
            this.SetupLayer(this.displayObject, this.displayLayer);

            this.displayCamera.enabled = true;
            this.displayCamera.gameObject.SetActive(true);

            this.displayCameraCtrl.DisplayObject = display_obj;
            this.displayImage.enabled = true;

            var displayTransfrom = this.displayObject.transform;
            displayTransfrom.SetParent(this.transform, true);
            displayTransfrom.position = Vector3.zero;
            displayTransfrom.rotation = Quaternion.identity;
            displayTransfrom.localScale = Vector3.one;
            displayTransfrom.position = this.displayOffset;
            displayTransfrom.rotation = Quaternion.Euler(this.displayRotation);
            displayTransfrom.localScale = this.displayScale;

            if (null != look_camera)
            {
                this.displayCamera.transform.position = look_camera.transform.position;
                this.displayCamera.transform.rotation = look_camera.transform.rotation;
                this.displayCamera.fieldOfView = look_camera.fieldOfView;
                this.displayCamera.nearClipPlane = look_camera.nearClipPlane;
                this.displayCamera.farClipPlane = look_camera.farClipPlane;
                this.displayCamera.orthographic = look_camera.orthographic;
                this.displayCamera.orthographicSize = look_camera.orthographicSize;
            }
        }

        private void SetupLayer(GameObject go, int layer)
        {
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                this.SetupLayer(go.transform.GetChild(i).gameObject, layer);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (this.displayObject && this.dragSpeed > 0.0f)
            {
                float delta = -this.dragSpeed * eventData.delta.x * Time.deltaTime;
                this.dragRotation += delta;
                this.displayObject.transform.rotation = Quaternion.Euler(this.displayRotation) * Quaternion.AngleAxis(this.dragRotation, Vector3.up);
            }
        }

        private void Awake()
        {
            if (this.displayImage == null)
            {
                this.displayImage = this.GetComponent<RawImage>();
            }

            if (this.displayImage != null)
            {
                this.displayImage.enabled = false;
            }

            if (this.displayCamera != null)
            {
                this.displayCamera.enabled = false;
                this.displayCameraCtrl = this.displayCamera.GetOrAddComponent<UI3DDisplayCamera>();
            }
        }

        private void OnDestroy()
        {
            if (this.displayTexture != null)
            {
                RenderTexture.ReleaseTemporary(this.displayTexture);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.displayObject != null)
            {
                var displayTransfrom = this.displayObject.transform;
                displayTransfrom.position = this.displayOffset;
                displayTransfrom.rotation = Quaternion.Euler(this.displayRotation) * Quaternion.AngleAxis(this.dragRotation, Vector3.up);
                displayTransfrom.localScale = this.displayScale;
            }
        }
#endif
    }
}
