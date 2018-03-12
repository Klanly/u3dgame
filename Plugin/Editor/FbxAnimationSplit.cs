using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Yifan.Plugin
{
    class FbxAnimationSplit : AssetPostprocessor
    {
        private struct AnimiClipCfg
        {
            public string name;
            public int start;
            public int end;
            public bool is_loop;
        }

        private List<AnimiClipCfg> cfg_list = new List<AnimiClipCfg>();

        public void OnPreprocessModel()
        {
            if (!this.ReadAnimationSplitCfg())
            {
                return;
            }

            this.SplitFbx();
        }

        private bool ReadAnimationSplitCfg()
        {
            ModelImporter modelImporter = (ModelImporter)this.assetImporter;
            if (!this.assetPath.Contains("_Animation"))
            {
                return false;
            }

            string cfg_path = this.assetPath.Replace(".FBX", ".split");
            if (!File.Exists(cfg_path))
            {
                return false;
            }

            this.cfg_list.Clear();
            using (StreamReader readr = new StreamReader(cfg_path, Encoding.Default))
            {
                string line;
                while ((line = readr.ReadLine()) != null)
                {
                    MatchCollection match_list = Regex.Matches(line, "\\b\\S*\\s?");
                    List<string> list = new List<string>();

                    foreach (Match m in match_list)
                    {
                        string val = m.ToString();
                        if (!string.IsNullOrEmpty(val))
                        {
                            list.Add(val);
                        }
                    }

                    if (list.Count != 4)
                    {
                        return false;
                    }

                    AnimiClipCfg cfg;
                    cfg.name = list[0];
                    cfg.start = int.Parse(list[1]);
                    cfg.end = int.Parse(list[2]);
                    cfg.is_loop = list[3].Equals("true") ? true : false;
                    this.cfg_list.Add(cfg);

                    Debug.Log("auto split animation succ");
                }
            }

            return true;
        }

        private void SplitFbx()
        {
            if (this.cfg_list.Count <= 0)
            {
                return;
            }

            ModelImporterClipAnimation[] clipAnimations = new ModelImporterClipAnimation[this.cfg_list.Count];
            int index = 0;

            for (int i = 0; i < this.cfg_list.Count; ++ i)
            {
                AnimiClipCfg item = this.cfg_list[i];
                ModelImporterClipAnimation clip = new ModelImporterClipAnimation();
                clip.name = item.name;
                clip.firstFrame = item.start;
                clip.lastFrame = item.end;
                clip.loop = item.is_loop;

                clipAnimations[index++] = clip;
            }

            (this.assetImporter as ModelImporter).clipAnimations = clipAnimations;
        }
    }
}
