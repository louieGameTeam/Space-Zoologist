using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(OptionalResizeData2D))]
public class OptionalResizeData2DDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get will resize property
        SerializedProperty willResize = property.FindPropertyRelative("willResize");

        // If will resize is true, then edit the resize data
        if(willResize.boolValue)
        {
            // Set the height for only one control when we edit the checkbox
            position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, willResize, label);
            
            EditorGUI.PropertyField(position, property.FindPropertyRelative("resizeData"), GUIContent.none, true);
        }
        // If will resize is false, edit only the toggle
        else
        {
            EditorGUI.PropertyField(position, willResize, label);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty resizeData = property.FindPropertyRelative("resizeData");
        float height;

        // If will resize and data is expanded, this is the height of the data
        if(property.FindPropertyRelative("willResize").boolValue && resizeData.isExpanded)
        {
            height = EditorGUI.GetPropertyHeight(resizeData);
        }
        // If either is false, this just has one line
        else height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        return height;
    }
}
