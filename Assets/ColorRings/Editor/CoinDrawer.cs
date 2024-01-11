using System.Reflection;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Coin))]
public class CoinDrawer : PropertyDrawer {
    private static PropertyInfo CoinValue = typeof(Coin).GetProperty(nameof(Coin.Value));
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var coin = (Coin)property.boxedValue;
        CoinValue.SetValue(coin, (ObscuredInt) EditorGUI.IntField(position, label, coin.Value));
        property.boxedValue = coin;
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();
    }
}