using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Yifan.Core
{
    class BundleLoader : IAssetLoader
    {
        private static string ASSET_BUNDLE_DIR = System.IO.Path.Combine(Application.dataPath, "../AssetBundles");
        private static AssetBundleManifest manifest;

        private Queue<LoadItem> loadQueue = new Queue<LoadItem>();
        private Dictionary<AssetId, LoadItem> loadDic = new Dictionary<AssetId, LoadItem>();
        private bool is_loading = false;

        public BundleLoader()
        {
            Scheduler.AddTask(this.Update);
        }

        ~BundleLoader()
        {
            Scheduler.RemoveTask(this.Update);
        }

        private static AssetBundleManifest LoadAssetBundleManifest()
        {
            string path = System.IO.Path.Combine(ASSET_BUNDLE_DIR, "AssetBundles");
            var bundle = AssetBundle.LoadFromFile(path);
            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            bundle.Unload(false);
            return manifest;
        }

        public void LoadAsset(LoadItem load_item)
        {
            if (null == manifest)
            {
                manifest = LoadAssetBundleManifest();
            }

            LoadItem same_load_item;
            if (loadDic.TryGetValue(load_item.asset_id, out same_load_item))
            {
                same_load_item.load_callback += load_item.load_callback;
                return;
            }

            this.LoadDependencies(load_item);
            this.loadQueue.Enqueue(load_item);
        }

        private void LoadDependencies(LoadItem load_item)
        {
            string[] dependence = manifest.GetAllDependencies(load_item.asset_id.bundleName);
            for (int i = 0; i < dependence.Length; ++i)
            {
                AssetId asset_id = new AssetId(dependence[i], "");
                if (loadDic.ContainsKey(asset_id))
                {
                    continue;
                }

                LoadItem dep_load_item = new LoadItem();
                dep_load_item.asset_id = asset_id;
                dep_load_item.type = AssetType.BUNDLE;
                this.loadQueue.Enqueue(dep_load_item);
            }
        }

        private void Update()
        {
            if (loadQueue.Count <= 0 || this.is_loading)
            {
                return;
            }


            LoadItem load_item = loadQueue.Dequeue();
            if (null != CacheManager.Instance.GetAssetItem(load_item.asset_id, AssetType.BUNDLE))
            {
                return;
            }

            Scheduler.RunCoroutine(this.AsyncLoadBundle(load_item));
        }

        private IEnumerator AsyncLoadBundle(LoadItem load_item)
        {
            Debug.Log(string.Format("Load Bundle {0}", load_item.asset_id.bundleName));

            this.is_loading = true;
            string bundle_name = load_item.asset_id.bundleName;
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(ASSET_BUNDLE_DIR, bundle_name));
            yield return request;

            if (null == request.assetBundle) yield break;

            this.is_loading = false;
            AssetItem asset_item = new AssetItem();
            asset_item.obj = request.assetBundle;
            asset_item.assetId = new AssetId(load_item.asset_id.bundleName, "");
            asset_item.type = AssetType.BUNDLE;
            CacheManager.Instance.AddAssetItem(asset_item);
            
            if (null != load_item.load_callback)
            {
                load_item.load_callback(asset_item);
            }
        }
    }
}
