using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NotebookTabMask))]
public class NotebookTabMaskDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditArrayOnEnum.OnGUI<NotebookTab>(position, property.FindPropertyRelative("mask"), label);
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditArrayOnEnum.GetPropertyHeight(property.FindPropertyRelative("mask"), label);
    }
}
