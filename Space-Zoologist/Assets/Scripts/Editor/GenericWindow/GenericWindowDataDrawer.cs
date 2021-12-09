using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(GenericWindowData))]
public class GenericWindowDataDrawer : PropertyDrawer
{
    #region Override Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty message = property.FindPropertyRelative(nameof(message));
        SerializedProperty primaryButtonData = property.FindPropertyRelative(nameof(primaryButtonData));
        SerializedProperty hasSecondaryButton = property.FindPropertyRelative(nameof(hasSecondaryButton));
        SerializedProperty secondaryButtonData = property.FindPropertyRelative(nameof(secondaryButtonData));

        // Set height for only one control
        position.height = EditorExtensions.StandardControlHeight;

        // Layout the foldout
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;

        if (property.isExpanded)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Edit the message
            EditorGUI.PropertyField(position, message);
            position.y += position.height;

            // Edit the primary button data
            EditorGUI.PropertyField(position, primaryButtonData, true);
            position.y += EditorGUI.GetPropertyHeight(primaryButtonData);

            // Layout has secondary button property
            EditorGUI.PropertyField(position, hasSecondaryButton);
            position.y += position.height;

            if (hasSecondaryButton.boolValue)
            {
                // Increase indent for property that depends on bool value
                EditorGUI.indentLevel++;
                // Layout secondary button data
                EditorGUI.PropertyField(position, secondaryButtonData, true);
                EditorGUI.indentLevel--;
            }

            // Remove indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get properties
        SerializedProperty primaryButtonData = property.FindPropertyRelative(nameof(primaryButtonData));
        SerializedProperty hasSecondaryButton = property.FindPropertyRelative(nameof(hasSecondaryButton));
        SerializedProperty secondaryButtonData = property.FindPropertyRelative(nameof(secondaryButtonData));

        // Set height for one control
        float height = EditorExtensions.StandardControlHeight;

        if (property.isExpanded)
        {
            // Add height for message
            height += EditorExtensions.StandardControlHeight;
            // Add height for primary button data
            height += EditorGUI.GetPropertyHeight(primaryButtonData);
            // Add height for has secondary button bool
            height += EditorExtensions.StandardControlHeight;

            // Add height for secondary button data if showing
            if (hasSecondaryButton.boolValue) height += EditorGUI.GetPropertyHeight(secondaryButtonData);
        }

        return height;
    }
    #endregion
}
