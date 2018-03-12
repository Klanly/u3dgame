using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Yifan.Core
{
    public class AssetManager : Singleton<AssetManager>
    {
        public delegate void CreateAssetCallback(AssetId asset_id);
        public void CreateScene(AssetId asset_id, LoadSceneMode mode, CreateAssetCallback create_callback)
        {
            SceneLoadItem load_item = new SceneLoadItem();
            load_item.asset_id = asset_id;
            load_item.type = AssetType.SCENE;
            load_item.load_mode = mode;
            load_item.load_callback = (AssetItem item) =>
            {
                create_callback(asset_id);
            };

            LoaderManager.Instance.LoadAsset(load_item);
        }

        public delegate void CreateGoCallback(GameObject go);
        public void CreateGameObject(AssetId asset_id, CreateGoCallback create_callback)
        {
            // 从缓存里取prefab
            {
                AssetItem asset_item = CacheManager.Instance.GetAssetItem(asset_id, AssetType.PREFAB);
                if (null != asset_item)
                {
                    GameObject obj = UnityEngine.Object.Instantiate(asset_item.obj as GameObject);
                    if (null != create_callback ) create_callback(obj);
                    return;
                }
            }

            // 加载prefab
            {
                LoadItem load_item = new LoadItem();
                load_item.asset_id = asset_id;
                load_item.type = AssetType.PREFAB;
                load_item.load_callback = (AssetItem item) =>
                {
                    GameObject obj = UnityEngine.Object.Instantiate(item.obj as GameObject);
                    if (null != create_callback) create_callback(obj);
                };

                LoaderManager.Instance.LoadAsset(load_item);
            }
        }
    }
}

