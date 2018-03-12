using System;
using System.Collections.Generic;

namespace Yifan.Core
{
    interface ICacheInterface
    {
        AssetItem GetAssetItem(AssetId asset_id);
        void AddAssetItem(AssetItem asset);
        bool IsExistAsset(AssetId asset_id);
    }
}
