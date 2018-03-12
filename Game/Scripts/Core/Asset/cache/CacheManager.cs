using System;
using System.Collections.Generic;

namespace Yifan.Core
{
    class CacheManager : Singleton<CacheManager>
    {
        private BundleCache bundleCache = new BundleCache();
        private PrefabCache prefabCache = new PrefabCache();

        private ICacheInterface GetCache(AssetType type)
        {
            if (AssetType.BUNDLE == type)
            {
                return bundleCache;
            }
            else if (AssetType.PREFAB == type)
            {
                return prefabCache;
            }

            return null;
        }

        public AssetItem GetAssetItem(AssetId asset_id, AssetType type)
        {
            ICacheInterface cache = this.GetCache(type);
            if (null == cache) return null;

            return cache.GetAssetItem(asset_id);
        }

        public void AddAssetItem(AssetItem asset)
        {
            ICacheInterface cache = this.GetCache(asset.type);
            if (null == cache) return;

            cache.AddAssetItem(asset);
        }

        public bool IsExistAsset(AssetId asset_id, AssetType type)
        {
            ICacheInterface cache = this.GetCache(type);
            if (null == cache) return false;

            return cache.IsExistAsset(asset_id);
        }
    }
}

