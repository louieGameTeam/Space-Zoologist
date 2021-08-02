using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ResizeData2D))]
public class ResizeData2DDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Set the position to have room for one control
        position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

        // Edit the foldout
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;
        
        if (property.isExpanded)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // If property is expanded, edit type
            SerializedProperty type = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(position, type);
            position.y += position.height;

            // Edit differently based on the enum value
            switch(type.enumValueIndex)
            {
                // Exact edits both fields
                case 0:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("width"));
                    position.y += position.height;
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("height"));
                    break;
                // Preserve aspect horizontal edits only width
                case 1:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("width"));
                    break;
                // Preserve aspect vertical edits only height
                case 2:
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("height"));
                    break;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if(property.isExpanded)
        {
            // If property is expanded, we need at least 2 more spaces
            height += 2f * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            // If the type is "Exact", we need one more space
            if(property.FindPropertyRelative("type").enumValueIndex == 0)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        return height;
    }
}
