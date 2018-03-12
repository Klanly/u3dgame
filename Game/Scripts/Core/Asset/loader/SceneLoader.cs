using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yifan.Core
{
    class SceneLoader : IAssetLoader
    {
        public struct WaitLoadSceneItem
        {
            public LoadItem load_item;
            public AsyncOperation asyncOperation;
        }

        private List<WaitLoadSceneItem> wait_list = new List<WaitLoadSceneItem>();

        public SceneLoader()
        {
            Scheduler.AddTask(this.Update);
        }

        ~SceneLoader()
        {
            Scheduler.RemoveTask(this.Update);
        }

        public void LoadAsset(LoadItem load_item)
        {
            {
                if (CacheManager.Instance.IsExistAsset(load_item.asset_id, AssetType.BUNDLE))
                {
                    this.AsyncLoadScene(load_item as SceneLoadItem);
                    return;
                }
            }
 
            {
                LoadItem load_bundle_item = new LoadItem();
                load_bundle_item.asset_id = load_item.asset_id;
                load_bundle_item.type = AssetType.BUNDLE;
                load_bundle_item.load_callback = (AssetItem asset_item) =>
                {
                    this.AsyncLoadScene(load_item as SceneLoadItem);
                };
                LoaderManager.Instance.LoadAsset(load_bundle_item);
            }
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
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(load_item.asset_id.assetName, load_item.load_mode);

            WaitLoadSceneItem wait_item = new WaitLoadSceneItem();
            wait_item.load_item = load_item;
            wait_item.asyncOperation = asyncOperation;
            this.wait_list.Add(wait_item);
        }
    }
}
