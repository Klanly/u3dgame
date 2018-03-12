using UnityEditor;
using UnityEngine;

namespace Yifan.Core
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerDrawer : PropertyDrawer
    {
        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label)
        {
            var tag = EditorGUI.LayerField(
                position, label.text, property.intValue);
            property.intValue = tag;
        }
    }
}
