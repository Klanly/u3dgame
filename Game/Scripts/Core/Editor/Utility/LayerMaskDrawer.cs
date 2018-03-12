using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Yifan.Core
{
    [CustomPropertyDrawer(typeof(LayerMaskAttribute))]
    public class LayerMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var layer = property.intValue;
            var layerMasksOptions = Enumerable.Range(0, 31)
                .Select(i => LayerMask.LayerToName(i))
                .Where(m => !string.IsNullOrEmpty(m)).ToArray();
            var currentMask = 0;
            for (var i = 0; i < layerMasksOptions.Length; ++i)
            {
                var mask = 1 << LayerMask.NameToLayer(layerMasksOptions[i]);
                if ((layer & mask) != 0)
                {
                    currentMask |= 1 << i;
                }
            }

            int newMask = EditorGUI.MaskField(
                position, label, currentMask, layerMasksOptions);
            if (newMask != currentMask)
            {
                var finalMask = 0;
                if (newMask != -1)
                {
                    for (var i = 0; i < layerMasksOptions.Length; ++i)
                    {
                        if ((newMask & (1 << i)) != 0)
                        {
                            finalMask |= 
                                1 << LayerMask.NameToLayer(layerMasksOptions[i]);
                        }
                    }
                }
                else
                {
                    finalMask = -1;
                }

                layer = finalMask;
            }

            if (layer != property.intValue)
            {
                property.intValue = layer;
            }
        }
    }
}
