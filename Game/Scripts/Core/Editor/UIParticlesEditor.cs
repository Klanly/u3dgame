using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Yifan.Core
{
    [CustomEditor(typeof(UIParticles))]
    [CanEditMultipleObjects]
    public class UIParticlesEditor : GraphicEditor
    {
        private SerializedProperty renderMode;
        private SerializedProperty stretchedSpeedScale;
        private SerializedProperty stretchedLenghScale;
        private SerializedProperty isIgnoreTimescale;
        private SerializedProperty isSupportClip;

        protected override void OnEnable()
        {
            base.OnEnable();

            renderMode = serializedObject.FindProperty("renderMode");
            stretchedSpeedScale = serializedObject.FindProperty("stretchedSpeedScale");
            stretchedLenghScale = serializedObject.FindProperty("stretchedLenghScale");
            isIgnoreTimescale = serializedObject.FindProperty("isIgnoreTimescale");
            isSupportClip = serializedObject.FindProperty("isSupportClip");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(isSupportClip);
            UIParticles ui_ps = (UIParticles)target;
            EditorGUILayout.PropertyField(renderMode);

            if (UIParticles.UiParticleRenderMode.StreachedBillboard == ui_ps.RenderMode)
            {
                EditorGUILayout.PropertyField(stretchedSpeedScale);
                EditorGUILayout.PropertyField(stretchedLenghScale);
            }

            EditorGUILayout.PropertyField(isIgnoreTimescale);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Apply to nested particle systems"))
            {
                ParticleSystem[] nested = ui_ps.gameObject.GetComponentsInChildren<ParticleSystem>();
                foreach (var particleSystem in nested)
                {
                    if (null == particleSystem.GetComponent<UIParticles>())
                    {
                        particleSystem.gameObject.AddComponent<UIParticles>();
                    }
                }
            }
        }
    }
}
