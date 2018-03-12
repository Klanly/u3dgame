using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Yifan.Core
{
    [CustomEditor(typeof(MinimapCamera))]
    class sealedMinimapCameraEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Take Texture"))
            {
                this.TakeTexture();
            }
        }

        private void TakeTexture()
        {
            var minimapCamera = this.target as MinimapCamera;
            var camera = minimapCamera.GetComponent<Camera>();

            var miniMapTexture = new RenderTexture(
                minimapCamera.MapTextureWidth,
                minimapCamera.MapTextureHeight,
                32);
            miniMapTexture.hideFlags = HideFlags.DontSave;
            miniMapTexture.name = "Mimimap";
            miniMapTexture.anisoLevel = 9;
            miniMapTexture.filterMode = FilterMode.Trilinear;
            miniMapTexture.format = RenderTextureFormat.ARGB32;
            miniMapTexture.antiAliasing = 8;
            camera.targetTexture = miniMapTexture;

            var holdEnabled = camera.enabled;
            var holdActive = camera.gameObject.activeSelf;
            camera.enabled = true;
            camera.gameObject.SetActive(true);

            EditorApplication.delayCall += () =>
            {
                var scene = EditorSceneManager.GetActiveScene();
                var scenePath = scene.path;
                if (string.IsNullOrEmpty(scenePath))
                {
                    EditorUtility.DisplayDialog(
                        "Notice", "Please save current scene first.", "OK");
                    return;
                }

                var dir = scenePath.Substring(0, scenePath.LastIndexOf("."));
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }

                var filePath = Path.Combine(dir, "minimap.png");
                if (camera.SaveRenderTextureAsPNG(filePath))
                {
                    Debug.Log("Save the minimap to: " + dir);

                    AssetDatabase.Refresh();

                    Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
                    minimapCamera.MapTexture = savedTexture;
                    EditorSceneManager.MarkSceneDirty(scene);
                }
                else
                {
                    Debug.LogError("Can not save the minimap to: " + dir);
                }

                camera.enabled = holdEnabled;
                camera.gameObject.SetActive(holdActive);
                camera.targetTexture = null;
            };
        }

    }
}
