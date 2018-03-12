#define SIMULATE_LOAD

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yifan.Core
{
    public delegate void LoadAssetCallback(AssetItem asset_item);

    public class LoadItem
    {
        public AssetId asset_id;
        public AssetType type;
        public LoadAssetCallback load_callback;
    }

    public class SceneLoadItem : LoadItem
    {
        public LoadSceneMode load_mode;
    }

    class LoaderManager : Singleton<LoaderManager>
    {
        private BundleLoader bundleLoader = new BundleLoader();

        private PrefabLoader prefabLoader = new PrefabLoader();
        private SimulatePrefabLoader simulatePrfabLoader = new SimulatePrefabLoader();

        private SceneLoader sceneLoader = new SceneLoader();
        private SimulateSceneLoader simulateSceneLoader = new SimulateSceneLoader();

        private IAssetLoader GetLoader(AssetType type)
        {
            if (AssetType.BUNDLE == type)
            {
                return bundleLoader;
            }

            else if (AssetType.PREFAB == type)
            {
#if SIMULATE_LOAD
                return simulatePrfabLoader;
#else
                return prefabLoader;   
#endif
            }

            else if (AssetType.SCENE == type)
            {
#if SIMULATE_LOAD
                return simulateSceneLoader;
#else
                return sceneLoader;
#endif
            }

            return null;
        }

        public void LoadAsset(LoadItem load_item)
        {
            IAssetLoader loader = this.GetLoader(load_item.type);
            if (null == loader) return;

            loader.LoadAsset(load_item);
        }
    }
}
