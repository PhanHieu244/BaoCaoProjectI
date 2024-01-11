using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Puzzle.UI
{
    [CustomPropertyDrawer(typeof(SortingLayerSelectionAttribute))]
    public class SortingLayerSelectionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var layers = SortingLayer.layers.Select(layer => layer.name).ToArray();
            var selectIndex = 0;
            
            // EditorGUI.PropertyField(position,property);
            if (!string.IsNullOrEmpty(property.stringValue))
            {
                if (layers.Contains(property.stringValue))
                {
                    selectIndex = Array.IndexOf(layers, property.stringValue);
                }
            }
            var newSelectIndex = EditorGUI.Popup(position, property.displayName, selectIndex, layers);
            
            if (newSelectIndex != selectIndex)
            {
                property.stringValue = layers[newSelectIndex];
            }
        }
    }
}