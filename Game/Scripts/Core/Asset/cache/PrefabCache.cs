using System;
using System.Collections.Generic;

namespace Yifan.Core
{
    class PrefabCache : ICacheInterface
    {
        private Dictionary<AssetId, AssetItem> cache = new Dictionary<AssetId, AssetItem>();

        public AssetItem GetAssetItem(AssetId asset_id)
        {
            AssetItem item;
            if (!cache.TryGetValue(asset_id, out item))
            {
                return null;
            }

            return item;
        }

        public void AddAssetItem(AssetItem asset)
        {
            cache.Add(asset.assetId, asset);
        }

        public bool IsExistAsset(AssetId asset_id)
        {
            return cache.ContainsKey(asset_id);
        }
    }
}

