using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Pattern))]
public class PatternDrawer : PropertyDrawer
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
        var p = property.FindPropertyRelative($"{nameof(Pattern.value)}");
        if (property.boxedValue is Pattern pattern)
        {
            for (var i = 0; i < pattern.value.Length; i++)
            {
                position.width = position.height;
                p.GetArrayElementAtIndex(i).enumValueIndex = (int)(Color)EditorGUI.EnumPopup(position, pattern.value[i]);
                
                EditorGUI.DrawRect(position, pattern.value[i] switch
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
                position.x += position.height + 2;
            }

            position.x += 50;
            position.width = 50;
            EditorGUI.LabelField(position, $"{nameof(Pattern.ratio)}");
            position.x += position.width;
            position.width = with - position.x;
            pattern.ratio = EditorGUI.FloatField(position, pattern.ratio);
        }

        property.serializedObject.ApplyModifiedProperties();
    }
}