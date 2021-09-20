using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public static class EditArrayOnEnum
{
    #region Custom Property Drawer Helpers
    public static void OnGUI(Rect position, SerializedProperty list, GUIContent label, System.Type enumType)
    {
        // Set height for only one control
        position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        
        // Use a foldout for the list
        list.isExpanded = EditorGUI.Foldout(position, list.isExpanded, label);
        position.y += position.height;

        if(list.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Get the enum labels and set the length of the list to the length of the enum
            string[] enumLabels = System.Enum.GetNames(enumType);
            if (enumLabels.Length != list.arraySize) list.arraySize = enumLabels.Length;
            
            // Edit each property in the array
            for (int i = 0; i < list.arraySize; i++)
            {
                // Get the current element and setup the element label to be the same as the enum name
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                GUIContent elementLabel = new GUIContent(ObjectNames.NicifyVariableName(enumLabels[i]));
                // Edit the element and advance the position down
                EditorGUI.PropertyField(position, element, elementLabel, true);
                position.y += EditorGUI.GetPropertyHeight(element, true);
            }

            EditorGUI.indentLevel--;
        }
    }

    public static void OnGUI<EnumType>(Rect position, SerializedProperty list, GUIContent label) 
        where EnumType : System.Enum
    {
        OnGUI(position, list, label, typeof(EnumType));
    }

    public static float GetPropertyHeight(SerializedProperty list, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if(list.isExpanded)
        {
            // Edit each property in the array
            for (int i = 0; i < list.arraySize; i++)
            {
                // Edit the element and advance the position down
                height += EditorGUI.GetPropertyHeight(list.GetArrayElementAtIndex(i), true);
            }
        }

        return height;
    }
    #endregion

    #region Custom Editor Helpers
    public static void OnInspectorGUI(SerializedProperty list, System.Type enumType)
    {
        list.isExpanded = EditorGUILayout.Foldout(list.isExpanded, list.displayName);

        if(list.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Get the enum labels and set the length of the list to the length of the enum
            string[] enumLabels = System.Enum.GetNames(enumType);
            if (enumLabels.Length != list.arraySize) list.arraySize = enumLabels.Length;

            // Edit each property in the array
            for (int i = 0; i < list.arraySize; i++)
            {
                // Get the current element and setup the element label to be the same as the enum name
                SerializedProperty element = list.GetArrayElementAtIndex(i);
                GUIContent elementLabel = new GUIContent(ObjectNames.NicifyVariableName(enumLabels[i]));

                // Edit the element and advance the position down
                EditorGUILayout.PropertyField(element, elementLabel, true);
            }

            EditorGUI.indentLevel--;
        }
    }
    public static void OnInspectorGUI<EnumType>(SerializedProperty list)
    {
        OnInspectorGUI(list, typeof(EnumType));
    }
    #endregion
}
