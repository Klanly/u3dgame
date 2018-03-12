using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Yifan.Core
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The scene camera.")]
        private Camera sceneCamera;

        [SerializeField]
        [Tooltip("The map texture.")]
        private Texture2D mapTexture;

        [SerializeField]
        [Tooltip("The minimap texture width.")]
        private int mapTextureWidth = 512;

        [SerializeField]
        [Tooltip("The minimap texture height.")]
        private int mapTextureHeight = 512;

        private static MinimapCamera instance;

        private Camera mapCamera;
        private Vector3 cameraOrthPos;
        private float cameraOrthSize = 0;

        public static MinimapCamera Instance
        {
            get { return instance; }
        }

        public Texture2D MapTexture
        {
            get { return this.mapTexture; }
            set { this.mapTexture = value; }
        }

        public int MapTextureWidth
        {
            get { return this.mapTextureWidth; }
        }

        public int MapTextureHeight
        {
            get { return this.mapTextureHeight; }
        }

        private void Awake()
        {
            Assert.IsNull(instance);

            instance = this;

            this.mapCamera = this.GetComponent<Camera>();
            Assert.IsNotNull(this.mapCamera);
            Assert.IsTrue(this.mapCamera.orthographic);

            this.mapCamera.enabled = false;
            this.cameraOrthPos = this.mapCamera.transform.position;
            this.cameraOrthSize = this.mapCamera.orthographicSize;
        }

        public Vector2 TransformWorldToUV(Vector3 pos)
        {
            var viewPos = new Vector3(
                (pos.x - (this.cameraOrthPos.x - this.cameraOrthSize)) / (2 * this.cameraOrthSize),
                pos.y,
                (pos.z - (this.cameraOrthPos.z - this.cameraOrthSize)) / (2 * this.cameraOrthSize));

            viewPos.x -= 0.5f;
            viewPos.z -= 0.5f;

            float angle = this.sceneCamera.transform.rotation.eulerAngles.y;
            viewPos = Quaternion.Euler(0, angle - 90, 0) * viewPos;

            return new Vector2(viewPos.x, viewPos.z);
        }

        public Vector3 TransformUVToWorld(Vector2 uv)
        {
            var viewPos = new Vector3(uv.x, 0.0f, uv.y);

            float angle = this.sceneCamera.transform.rotation.eulerAngles.y;
            viewPos = Quaternion.Euler(0, 90 - angle, 0) * viewPos;

            viewPos.x += 0.5f;
            viewPos.z += 0.5f;

            var pos = new Vector3(
                (viewPos.x * 2 * this.cameraOrthSize) + this.cameraOrthPos.x - this.cameraOrthSize,
                viewPos.y,
                (viewPos.z * 2 * this.cameraOrthSize) + this.cameraOrthPos.z - this.cameraOrthSize);

            return pos;
        }

        private void Update()
        {
            if (!Application.isPlaying && this.sceneCamera != null)
            {
                float angle = this.sceneCamera.transform.rotation.eulerAngles.y;
                this.transform.rotation = Quaternion.Euler(90, -270 - angle, 0);
            }
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
