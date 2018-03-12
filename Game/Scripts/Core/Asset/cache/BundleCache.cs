using System;
using System.Collections.Generic;

namespace Yifan.Core
{
    class BundleCache : ICacheInterface
    {
        private Dictionary<string, AssetItem> cache = new Dictionary<string, AssetItem>();

        public AssetItem GetAssetItem(AssetId asset_id)
        {
            AssetItem item;
            if (!cache.TryGetValue(asset_id.bundleName, out item))
            {
                return null;
            }

            return item;
        }

        public void AddAssetItem(AssetItem asset)
        {
            cache.Add(asset.assetId.bundleName, asset);
        }

        public bool IsExistAsset(AssetId asset_id)
        {
            return cache.ContainsKey(asset_id.bundleName);
        }
    }
}

