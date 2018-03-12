using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Yifan.Core
{
    class SimulateSceneLoader : IAssetLoader
    {
        public struct WaitLoadSceneItem
        {
            public LoadItem load_item;
            public AsyncOperation asyncOperation;
        }

        private List<WaitLoadSceneItem> wait_list = new List<WaitLoadSceneItem>();

        public SimulateSceneLoader()
        {
            Scheduler.AddTask(this.Update);
        }

        ~SimulateSceneLoader()
        {
            Scheduler.RemoveTask(this.Update);
        }

        public void LoadAsset(LoadItem load_item)
        {
            this.AsyncLoadScene(load_item as SceneLoadItem);
        }

        private void Update()
        {
            this.CheckLoadSceneComplete();
        }

        private void CheckLoadSceneComplete()
        {
            if (this.wait_list.Count <= 0)
            {
                return;
            }

            foreach (WaitLoadSceneItem item in this.wait_list)
            {
                if (item.asyncOperation.isDone)
                {
                    LoadItem load_item = item.load_item;
                    if (null != load_item.load_callback)
                    {
                        AssetItem asset_item = new AssetItem();
                        asset_item.assetId = load_item.asset_id;
                        asset_item.type = AssetType.SCENE;
                        asset_item.obj = null;
                        load_item.load_callback(asset_item);
                    }

                    this.wait_list.Remove(item);
                    break;
                }
            }
        }

        private void AsyncLoadScene(SceneLoadItem load_item)
        {
#if UNITY_EDITOR
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(
              load_item.asset_id.bundleName,
              load_item.asset_id.assetName);

            if (paths.Length <= 0)
            {
                return;
            }

            AsyncOperation asyncOperation;
            if (LoadSceneMode.Additive == load_item.load_mode)
            {
                asyncOperation = EditorApplication.LoadLevelAdditiveAsyncInPlayMode(paths[0]);
            }
            else
            {
                asyncOperation = EditorApplication.LoadLevelAsyncInPlayMode(paths[0]);
            }

            WaitLoadSceneItem wait_item = new WaitLoadSceneItem();
            wait_item.load_item = load_item;
            wait_item.asyncOperation = asyncOperation;
            this.wait_list.Add(wait_item);
#endif
        }
    }
}
