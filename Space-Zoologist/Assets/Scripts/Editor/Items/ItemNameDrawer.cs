using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemName))]
public class ItemNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditArrayOnEnum.OnGUI<ItemName.Type>(position, property.FindPropertyRelative("names"), label);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditArrayOnEnum.GetPropertyHeight(property.FindPropertyRelative("names"), label);
    }
}
