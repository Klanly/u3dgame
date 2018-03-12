using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Yifan.Core
{
    class SimulatePrefabLoader : IAssetLoader
    {
        private Queue<LoadItem> loadQueue = new Queue<LoadItem>();
        private Dictionary<AssetId, LoadItem> loadDic = new Dictionary<AssetId, LoadItem>();

        public SimulatePrefabLoader()
        {
            Scheduler.AddTask(this.Update);
        }

        ~SimulatePrefabLoader()
        {
            Scheduler.RemoveTask(this.Update);
        }

        public void LoadAsset(LoadItem load_item)
        {
            LoadItem same_load_item;
            if (loadDic.TryGetValue(load_item.asset_id, out same_load_item))
            {
                same_load_item.load_callback += load_item.load_callback;
                return;
            }

            loadDic.Add(load_item.asset_id, load_item);
            this.loadQueue.Enqueue(load_item);
        }

        private void Update()
        {
            if (this.loadQueue.Count <= 0)
            {
                return;
            }

            LoadItem load_item = this.loadQueue.Dequeue();
            string asset_name = load_item.asset_id.assetName;

            {
                AssetItem asset_item = CacheManager.Instance.GetAssetItem(load_item.asset_id, AssetType.PREFAB);
                if (null != asset_item)
                {
                    this.loadDic.Remove(asset_item.assetId);
                    load_item.load_callback(asset_item);
                    return;
                }
            }

            {
                this.LoadLocalPrefab(load_item);
            }
        }

        private void LoadLocalPrefab(LoadItem load_item)
        {
#if UNITY_EDITOR
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(
                  load_item.asset_id.bundleName,
                  load_item.asset_id.assetName);

            if (paths.Length <= 0)
            {
                return;
            }

            Debug.Log(string.Format("Load Local Prefab {0}", paths[0]));

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(paths[0], typeof(GameObject));
            if (null == obj)
            {
                return;
            }

            AssetItem item = new AssetItem();
            item.assetId = load_item.asset_id;
            item.type = AssetType.PREFAB;
            item.obj = obj;
            CacheManager.Instance.AddAssetItem(item);

            this.loadDic.Remove(item.assetId);
            if (null != load_item.load_callback)
            {
                load_item.load_callback(item);
            }
#endif
        }

    }
}
