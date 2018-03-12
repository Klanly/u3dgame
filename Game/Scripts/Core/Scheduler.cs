using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Core
{
    class Scheduler : MonoBehaviour
    {
        private static Scheduler instance;
        private static HashSet<Action> tasks = new HashSet<Action>();
        private static List<Action> nextFrameTask = new List<Action>();

        private static Scheduler Instance
        {
            get
            {
                CheckInstance();
                return instance;
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void CheckInstance()
        {
            if (instance == null && Application.isPlaying)
            {
                var go = new GameObject("Scheduler", typeof(Scheduler));
                GameObject.DontDestroyOnLoad(go);

                instance = go.GetComponent<Scheduler>();
            }
        }

        public static void RunCoroutine(IEnumerator coroutine)
        {
            Instance.StartCoroutine(coroutine);
        }

        public static void AddTask(Action action)
        {
            tasks.Add(action);
        }

        public static void RemoveTask(Action action)
        {
            tasks.Remove(action);
        }

        public static void Delay(Action task)
        {
            nextFrameTask.Add(task);
        }

        private void Update()
        {
            foreach (var item in tasks)
            {
                item();
            }

            if (nextFrameTask.Count > 0)
            {
                foreach (var task in nextFrameTask)
                {
                    task();
                }
                nextFrameTask.Clear();
            }
        }
    }
}
