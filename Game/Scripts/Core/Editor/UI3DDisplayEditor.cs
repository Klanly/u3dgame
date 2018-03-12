using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Yifan.Core
{
    [CustomEditor(typeof(UI3DDisplay))]
    class UI3DDisplayEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Display"))
            {
                (this.target as UI3DDisplay).Display();
            }
        }
    }
}
