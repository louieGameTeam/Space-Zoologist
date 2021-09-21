using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParallelArrayEditor<TElement>
{
    #region Public Fields
    public Func<SerializedProperty, TElement, GUIContent> content = (s, e) => new GUIContent(e.ToString());
    public Action<Rect, SerializedProperty, GUIContent, TElement> propertyField = (r, s, g, e) => EditorGUI.PropertyField(r, s, g, true);
    public Action<SerializedProperty, GUIContent, TElement> propertyFieldLayout = (s, g, e) => EditorGUILayout.PropertyField(s, g, true);
    public Func<SerializedProperty, float> propertyHeight = s => EditorGUI.GetPropertyHeight(s, true);
    #endregion

    #region Custom Property Drawer
    public virtual void OnGUI(Rect position, SerializedProperty array, GUIContent label, TElement[] parallelArray)
    {
        // Set height for only one control
        position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Use a foldout for the list
        array.isExpanded = EditorGUI.Foldout(position, array.isExpanded, label);
        position.y += position.height;

        if (array.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Get the enum labels and set the length of the list to the length of the enum
            if (parallelArray.Length != array.arraySize) array.arraySize = parallelArray.Length;

            // Edit each property in the array
            for (int i = 0; i < array.arraySize; i++)
            {
                // Get the current element and setup the element label to be the same as the enum name
                SerializedProperty element = array.GetArrayElementAtIndex(i);
                GUIContent elementLabel = content.Invoke(element, parallelArray[i]);

                // Edit the element and advance the position down
                propertyField.Invoke(position, element, elementLabel, parallelArray[i]);
                position.y += propertyHeight(element);
            }

            EditorGUI.indentLevel--;
        }
    }
    public virtual void OnGUI(Rect position, SerializedProperty array, TElement[] parallelArray)
    {
        OnGUI(position, array, new GUIContent(array.displayName), parallelArray);
    }
    public virtual float GetPropertyHeight(SerializedProperty array)
    {
        float height = EditorExtensions.StandardControlHeight;

        // If array is expanded, add heights for all 
        if(array.isExpanded)
        {
            for (int i = 0; i < array.arraySize; i++)
            {
                SerializedProperty element = array.GetArrayElementAtIndex(i);
                height += propertyHeight(element);
            }
        }

        return height;
    }
    #endregion

    #region Custom Editor Helpers
    public virtual void OnInspectorGUI(SerializedProperty array, TElement[] parallelArray)
    {
        // Use a foldout for the list
        array.isExpanded = EditorGUILayout.Foldout(array.isExpanded, array.displayName);

        if (array.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Get the enum labels and set the length of the list to the length of the enum
            if (parallelArray.Length != array.arraySize) array.arraySize = parallelArray.Length;

            // Edit each property in the array
            for (int i = 0; i < array.arraySize; i++)
            {
                // Get the current element and setup the element label to be the same as the enum name
                SerializedProperty element = array.GetArrayElementAtIndex(i);
                GUIContent elementLabel = content.Invoke(element, parallelArray[i]);

                // Edit the element
                propertyFieldLayout.Invoke(element, elementLabel, parallelArray[i]);
            }

            EditorGUI.indentLevel--;
        }
    }
    #endregion
}
