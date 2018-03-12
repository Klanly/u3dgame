using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Yifan.Scene
{
    [CustomEditor(typeof(ActorController))]
    class ActorControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
            {
                this.DrawPreview();
            }
            else
            {
                GUILayout.Label("Preview must in playing mode");
            }
        }

        private void DrawPreview()
        {
            if (GUILayout.Button("ActorBlink"))
            {
                ((ActorController)this.target).Blink();
            }

            if (GUILayout.Button("ActorFadeOut"))
            {
                ((ActorController)this.target).FadeOut(1.0f, null);
            }
        }
    }
}
