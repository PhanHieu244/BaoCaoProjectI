using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SkinData))]
public class SkinDrawer : PropertyDrawer
{
    private static UnityEngine.Color orange = new(250 / 255f, 100 / 255f, 0f);
    private static UnityEngine.Color grey = new(0.5f, 0.5f, 0.5f, 0.2f);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.serializedObject.Update();
        var with = position.width;
        position.width = 100;
        EditorGUI.LabelField(position, label);
        position.x += position.width + 4;
        
        if (property.boxedValue is SkinData skin)
        {

            position.width = 80;
            var p2 = property.FindPropertyRelative($"{nameof(SkinData.size)}");
            p2.enumValueIndex = (int)(RingSize)EditorGUI.EnumPopup(position, skin.size);
            
            position.x += 100;
            position.width = 30;
            var p = property.FindPropertyRelative($"{nameof(SkinData.color)}");
            p.enumValueIndex = (int)(Color)EditorGUI.EnumPopup(position, skin.color);
            EditorGUI.DrawRect(position, skin.color switch
            {
                Color.NONE => grey,
                Color.RED => UnityEngine.Color.red,
                Color.GREEN => UnityEngine.Color.green,
                Color.ORANGE => orange,
                Color.PINK => UnityEngine.Color.magenta,
                Color.YELLOW => UnityEngine.Color.yellow,
                Color.CYAN => UnityEngine.Color.cyan,
                Color.WHITE => UnityEngine.Color.white,
                Color.BLUE => UnityEngine.Color.blue,
                Color.PURPLE => UnityEngine.Color.black,
                _ => throw new ArgumentOutOfRangeException()
            });
            
            position.x += 45;
            position.width = with - position.x;
            var skinImageProperty = property.FindPropertyRelative($"{nameof(SkinData.skinImage)}");
            EditorGUI.PropertyField(position, skinImageProperty, GUIContent.none);
        }

        property.serializedObject.ApplyModifiedProperties();
    }
}