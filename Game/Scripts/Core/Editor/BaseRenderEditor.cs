using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Yifan.Core
{
    [CustomEditor(typeof(BaseRender))]
    class BaseRenderEditor : Editor
    {
        private SerializedProperty materials = null;
        private SerializedProperty renderQueue = null;
        private ReorderableList materialList;

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            this.materialList.DoLayoutList();
            EditorGUILayout.PropertyField(this.renderQueue);
            this.serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            this.materials = this.serializedObject.FindProperty("materials");
            this.renderQueue = this.serializedObject.FindProperty("renderQueue");

            this.materialList = new ReorderableList(this.serializedObject, this.materials);
            this.materialList.drawHeaderCallback =
              rect => GUI.Label(rect, "Materials:");
            this.materialList.elementHeight =
                1 * EditorGUIUtility.singleLineHeight;
            this.materialList.drawElementCallback =
                (rect, index, selected, focused) =>
                {
                    var element = this.materials.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(rect, element);
                };
            this.materialList.onAddCallback = list =>
            {
                var renderer = (BaseRender)this.target;
                renderer.AddDefaultMaterial();
                EditorUtility.SetDirty(this.target);
            };
        }
    }
}
