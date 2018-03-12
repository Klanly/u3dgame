using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Core
{
    public class Layers
    {
        private static int? walkable;
        public static int Walkable
        {
            get
            {
                if (!walkable.HasValue)
                {
                    walkable = LayerMask.NameToLayer("Walkable");
                }

                return walkable.Value;
            }
        }

        private static int? clickable;
        public static int Clickable
        {
            get
            {
                if (!clickable.HasValue)
                {
                    clickable = LayerMask.NameToLayer("Clickable");
                }

                return clickable.Value;
            }
        }
    }
    
}
