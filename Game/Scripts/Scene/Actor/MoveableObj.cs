using System;
using System.Collections.Generic;
using UnityEngine;
using Yifan.Core;

namespace Yifan.Scene
{
    class MoveableObj : MonoBehaviour
    {
        private static RaycastHit[] hits = new RaycastHit[8];

        private bool rotating;
        private Quaternion rotateTarget;
        private float rotateSpeed;
        private Action<int> rotateCallback;

        private bool moving;
        private Vector3 moveTarget;
        private float moveSpeed;
        private Action<int> moveCallback;
        private Vector3 offset;

        public void SetRotateCallback(Action<int> rotateCallback)
        {
            this.rotateCallback = rotateCallback;
        }

        public void SetMoveCallback(Action<int> moveCallback)
        {
            this.moveCallback = moveCallback;
        }

        public void SetOffset(Vector3 offset)
        {
            this.offset = offset;
            this.transform.position = FixToGround(this.transform.position) + this.offset;
        }

        public void SetPosition(Vector3 position)
        {
            this.transform.position = position;
            this.transform.position = FixToGround(this.transform.position) + this.offset;
        }

        public void RotateTo(Vector3 target, float speed)
        {
            var offset = target - transform.position;
            offset.y = 0;
            if (offset.sqrMagnitude > float.Epsilon)
            {
                this.rotateTarget = Quaternion.LookRotation(offset);
                this.rotateSpeed = speed;
                this.rotating = true;
            }
        }

        public void StopRotate()
        {
            this.rotating = false;
            if (this.rotateCallback != null)
            {
                this.rotateCallback(0);
            }
        }

        public void MoveTo(Vector3 target, float speed)
        {
            this.moveTarget = target;
            this.moveSpeed = speed;
            this.moving = true;
        }

        public void StopMove()
        {
            this.moving = false;
            if (this.moveCallback != null)
            {
                this.moveCallback(0);
            }
        }

        private void Update()
        {
            if (this.moving && this.rotating)
            {
                var position = this.DoPosition(this.transform.position);
                var rotation = this.DoRotation(this.transform.rotation);
                this.transform.SetPositionAndRotation(position, rotation);
            }
            else if (this.moving)
            {
                var position = this.DoPosition(this.transform.position);
                this.transform.position = position;
            }
            else if (this.rotating)
            {
                var rotation = this.DoRotation(this.transform.rotation);
                this.transform.rotation = rotation;
            }
        }

        private Vector3 DoPosition(Vector3 position)
        {
            var offset = this.moveTarget - position;
            offset.y = 0;
            var movement = offset.normalized * Time.unscaledDeltaTime * this.moveSpeed;
            if (movement.sqrMagnitude >= offset.sqrMagnitude)
            {
                position = this.moveTarget;
                this.moving = false;
                if (this.moveCallback != null)
                {
                    this.moveCallback(1);
                }
            }
            else
            {
                position += movement;
            }

            return FixToGround(position) + this.offset;
        }

        private Quaternion DoRotation(Quaternion rotation)
        {
            rotation = Quaternion.Slerp(
                rotation,
                this.rotateTarget,
                Time.unscaledDeltaTime * this.rotateSpeed);

            var angle = Quaternion.Angle(rotation, this.rotateTarget);
            if (angle < 0.01f)
            {
                rotation = this.rotateTarget;
                this.rotating = false;
                if (this.rotateCallback != null)
                {
                    this.rotateCallback(1);
                }
            }

            return rotation;
        }

        public static Vector3 FixToGround(Vector3 position)
        {
            var ray = new Ray(position + (10000.0f * Vector3.up), Vector3.down);
            var count = Physics.RaycastNonAlloc(ray, hits, float.PositiveInfinity, 1 << Layers.Walkable);

            if (count <= 0)
            {
                return position;
            }

            float max_height = Mathf.NegativeInfinity;
            int index = 0;
            for (int i = 0; i < count; ++i)
            {
                if (hits[i].point.y > max_height)
                {
                    max_height = hits[i].point.y;
                    index = i;
                }
            }

            return new Vector3(hits[index].point.x, max_height, hits[index].point.z);
        }
    }
}
