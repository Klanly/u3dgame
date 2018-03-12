using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Yifan.Core
{
    public class GUILayoutEx
    {
        public static void BeginContents(GUIStyle style)
        {
            GUILayout.BeginVertical(style);
        }

        public static void BeginContents()
        {
            BeginContents(GUI.skin.textArea);
        }

        public static void EndContents()
        {
            GUILayout.EndVertical();
        }
    }
}
