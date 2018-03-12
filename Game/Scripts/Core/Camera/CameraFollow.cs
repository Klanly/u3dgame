using UnityEngine;

namespace Yifan.Core
{
    [ExecuteInEditMode]
    public sealed class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Vector3 offset = new Vector3(0, 4.0f, -4.0f);

        [SerializeField]
        private float rotation;

        [SerializeField]
        private float pitch = 40.0f;

        private Vector3 offsetWorld;

        public Transform Target
        {
            get
            {
                return this.target;
            }

            set
            {
                if (this.target != value)
                {
                    this.target = value;
                    this.SyncRotation();
                }
            }
        }

        public Vector3 Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        public float Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        public float Pitch
        {
            get { return this.pitch; }
            set { this.pitch = value; }
        }

        public void SyncImmediate()
        {
            if (this.target == null)
            {
                return;
            }

            this.transform.position = this.target.position + this.offsetWorld;
        }

        public void SyncRotation()
        {
            this.offsetWorld = Quaternion.Euler(0, this.rotation, 0) * this.offset;
            var cameraRotation = this.transform.rotation;
            cameraRotation.SetLookRotation(-this.offsetWorld, Vector3.up);

            var eulerAngles = cameraRotation.eulerAngles;
            eulerAngles.x = this.pitch;
            cameraRotation.eulerAngles = eulerAngles;
            this.transform.rotation = cameraRotation;
        }

        private void Awake()
        {
            this.SyncRotation();
        }

        private void LateUpdate()
        {
            if (this.target != null)
            {
                this.FollowPosition(this.target.position);
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                this.SyncRotation();
            }
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.SyncRotation();
            if (this.target != null)
            {
                this.transform.position = this.target.position + this.offsetWorld;
            }
        }
#endif

        private void FollowPosition(Vector3 position)
        {
            this.transform.position = position + this.offsetWorld;
        }
    }
}
