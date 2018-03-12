using System;
using System.Collections.Generic;

namespace Yifan.Core
{
    public enum AssetType
    {
        BUNDLE = 0,
        PREFAB,
        SCENE,
        GAMEOBJECT,
    }

    public struct AssetId : IEquatable<AssetId>
    {
        public string bundleName;
        public string assetName;

        public AssetId(string bundle_name, string asset_name)
        {
            this.bundleName = bundle_name;
            this.assetName = asset_name;
        }

        public bool Equals(AssetId other)
        {
            return this.bundleName == other.bundleName &&
                this.assetName == other.assetName;
        }
    }

    public class AssetItem : IEquatable<AssetItem>
    {
        public AssetId assetId;
        public AssetType type;
        public object obj;

        public bool Equals(AssetItem other)
        {
            return null != this.obj && this.obj == other.obj;
        }
    }
}