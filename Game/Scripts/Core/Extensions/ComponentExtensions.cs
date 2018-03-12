using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Core
{
    public static class ComponentExtensions
    {
        public static Component GetOrAddComponent(this Component obj, System.Type type)
        {
            var component = obj.GetComponent(type);
            if (component == null)
            {
                component = obj.gameObject.AddComponent(type);
            }

            return component;
        }

        public static T GetOrAddComponent<T>(this Component obj) where T : Component
        {
            var component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.gameObject.AddComponent<T>();
            }

            return component;
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            var component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }
    }
}
