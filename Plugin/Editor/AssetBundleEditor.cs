using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Yifan.Plugin
{
    class AssetBundleEditor : Editor
    {
        [MenuItem("Yifan/AssetBundle")]
        public static void CreateAssetBundle()
        {
            BuildPipeline.BuildAssetBundles("AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        }
    }
}
