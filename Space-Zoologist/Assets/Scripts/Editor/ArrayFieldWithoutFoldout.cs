using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ArrayFieldWithoutFoldout
{
    #region Custom Property Drawer Helpers
    public static void OnGUI(Rect position, SerializedProperty array, GUIContent label)
    {
        // Set height for jsut one control
        position.height = EditorExtensions.StandardControlHeight;

        // Layout the array size
        EditorGUI.PropertyField(position, array.FindPropertyRelative("Array.size"), label);
        position.y += position.height;

        for (int i = 0; i < array.arraySize; i++)
        {
            // Get the element to edit
            SerializedProperty element = array.GetArrayElementAtIndex(i);

            // Layout this element
            EditorGUI.PropertyField(position, element);
            position.y += EditorGUI.GetPropertyHeight(element);
        }
    }
    public static float GetPropertyHeight(SerializedProperty array, GUIContent label)
    {
        float height = EditorExtensions.StandardControlHeight;

        // Add heights for all children
        for (int i = 0; i < array.arraySize; i++)
        {
            SerializedProperty element = array.GetArrayElementAtIndex(i);
            height += EditorGUI.GetPropertyHeight(element);
        }

        return height;
    }
    #endregion

    #region Custom Editor Helpers

    #endregion
}
