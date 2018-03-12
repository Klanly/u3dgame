using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Core
{
    class PrefabLoader : IAssetLoader
    {
        private Queue<LoadItem> loadQueue = new Queue<LoadItem>();
        private Dictionary<AssetId, LoadItem> loadDic = new Dictionary<AssetId, LoadItem>();

        public PrefabLoader()
        {
            Scheduler.AddTask(this.Update);
        }

        ~PrefabLoader()
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
                AssetItem asset_item = CacheManager.Instance.GetAssetItem(load_item.asset_id, AssetType.BUNDLE);
                if (null != asset_item)
                {
                    Scheduler.RunCoroutine(
                        this.LoadPrefabFromBundle(asset_item, asset_name, load_item.load_callback));
                    return;
                }
            }

            {
                LoadItem load_bundle_item = new LoadItem();
                load_bundle_item.asset_id = load_item.asset_id;
                load_bundle_item.type = AssetType.BUNDLE;
                load_bundle_item.load_callback = (AssetItem asset_item) =>
                {
                    Scheduler.RunCoroutine(
                        this.LoadPrefabFromBundle(asset_item, asset_name, load_item.load_callback));
                };
                LoaderManager.Instance.LoadAsset(load_bundle_item);
            }
        }

        private IEnumerator LoadPrefabFromBundle(AssetItem bundle_item, string prefab_name, LoadAssetCallback load_callback)
        {
            if (null == bundle_item || AssetType.BUNDLE != bundle_item.type) yield break;

            Debug.Log(string.Format("Load Prefab {0}", prefab_name));

            AssetBundle bundle = bundle_item.obj as AssetBundle;
            AssetBundleRequest request = bundle.LoadAssetAsync(prefab_name);
            yield return request;

            GameObject prefab = request.asset as GameObject;
            if (null == prefab) yield break;

            AssetItem item = new AssetItem();
            item.assetId = new AssetId(bundle_item.assetId.bundleName, prefab_name);
            item.type = AssetType.PREFAB;
            item.obj = prefab;
            CacheManager.Instance.AddAssetItem(item);

            this.loadDic.Remove(item.assetId);
            if (null != load_callback)
            {
                load_callback(item);
            }
        }

    }
}
