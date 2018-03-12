using System;
using System.Collections.Generic;
using HedgehogTeam.EasyTouch;
using UnityEngine;
using Yifan.Core;

namespace Yifan.Scene
{
    [RequireComponent(typeof(Camera))]
    public sealed class SceneInteractive : MonoBehaviour
    {
        public static SceneInteractive Instance { get; private set; }
        private Camera look_camera;
        private Action<Vector3> clickGroundEvent;

        private void Awake()
        {
            Instance = this;

            this.look_camera = this.GetComponent<Camera>();
            EasyTouch.On_SimpleTap += this.OnSimpleTapHandler;
        }

        public void ListenClickGround(Action<Vector3> callback)
        {
            this.clickGroundEvent += callback;
        }

        public void UnlistenClickGround(Action<Vector3> callback)
        {
            this.clickGroundEvent -= callback;
        }

        private void OnSimpleTapHandler(Gesture gesture)
        {
            Vector3 screen_pos = new Vector3(gesture.position.x, gesture.position.y, 0);
            Ray ray = this.look_camera.ScreenPointToRay(screen_pos);

            this.ProcessSceneHit(ray);
        }

        private bool ProcessSceneHit(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, 1 << Layers.Walkable);
            if (hits.Length <= 0)
            {
                return false;
            }

            float min_distance = Mathf.Infinity;
            int hit_index = 0;

            for (int i = 0; i < hits.Length; ++ i)
            {
                if (hits[i].distance <= min_distance)
                {
                    min_distance = hits[i].distance;
                    hit_index = i;
                }
            }

            if (null != this.clickGroundEvent)
            {
                this.clickGroundEvent(hits[hit_index].point);
            }

            return true;
        }
    }
}
