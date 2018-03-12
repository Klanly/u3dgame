using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Core
{
    class ShaderProperty
    {
        private static int? rimIntensity;
        public static int RimIntensity
        {
            get
            {
                if (!rimIntensity.HasValue)
                {
                    rimIntensity = Shader.PropertyToID("_RimIntensity");
                }

                return rimIntensity.Value;
            }
        }

        private static int? mainColor;
        public static int MainColor
        {
            get
            {
                if (!mainColor.HasValue)
                {
                    mainColor = Shader.PropertyToID("_MainColor");
                }

                return mainColor.Value;
            }
        }
    }
}
