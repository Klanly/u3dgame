namespace Yifan.Core
{
    using UnityEngine;
    using UnityEngine.Assertions;

    [RequireComponent(typeof(RectTransform))]
    public sealed class UIFollowTarget : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private Canvas canvas;

        private RectTransform rectTransform;

        public Transform Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        public Canvas Canvas
        {
            get { return this.canvas; }
            set { this.canvas = value; }
        }

        public static Vector3 CalculateScreenPosition(Vector3 position,  Camera camera, Canvas canvas,  RectTransform transform)
        {
            Vector3 screenPos = camera.WorldToScreenPoint(position);

            Vector3 pos = Vector3.zero;
            switch (canvas.renderMode)
            {
            case RenderMode.ScreenSpaceOverlay:
                RectTransformUtility.ScreenPointToWorldPointInRectangle(transform, screenPos, null, out pos);
                break;

            case RenderMode.ScreenSpaceCamera:
            case RenderMode.WorldSpace:
                RectTransformUtility.ScreenPointToWorldPointInRectangle(transform, screenPos, canvas.worldCamera, out pos);
                break;
            }

            return pos;
        }

        private void Awake()
        {
            this.rectTransform = this.GetComponent<RectTransform>();
            if (this.gameCamera == null)
            {
                this.gameCamera = Camera.main;
            }
        }

        private void Start()
        {
            this.SyncPosition();
        }

        private void Update()
        {
            if (this.gameCamera == null)
            {
                this.gameCamera = Camera.main;
            }

            this.SyncPosition();
        }

        private void SyncPosition()
        {
            if (this.target == null || 
                this.canvas == null || 
                this.gameCamera == null || 
                this.rectTransform == null)
            {
                return;
            }

            Vector3 pos = CalculateScreenPosition(
                this.target.position,
                this.gameCamera,
                this.canvas,
                this.rectTransform);
            this.transform.position = pos;
        }
    }
}
