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
        SerializedProperty background = property.FindPropertyRelative(nameof(background));
        SerializedProperty message = property.FindPropertyRelative(nameof(message));
        SerializedProperty startingAnchorPosition = property.FindPropertyRelative(nameof(startingAnchorPosition));
        SerializedProperty startingAnimationEase = property.FindPropertyRelative(nameof(startingAnimationEase));
        SerializedProperty endingAnimationEase = property.FindPropertyRelative(nameof(endingAnimationEase));
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

            // Edit the background
            EditorGUI.PropertyField(position, background);
            position.y += position.height;

            // Edit the message
            EditorGUI.PropertyField(position, message);
            position.y += position.height;

            // Edit starting anchor position
            EditorGUI.PropertyField(position, startingAnchorPosition);
            position.y += EditorGUI.GetPropertyHeight(startingAnchorPosition);

            // Edit the starting animation ease
            EditorGUI.PropertyField(position, startingAnimationEase);
            position.y += position.height;

            // Edit the ending animation ease
            EditorGUI.PropertyField(position, endingAnimationEase);
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
        SerializedProperty startingAnchorPosition = property.FindPropertyRelative(nameof(startingAnchorPosition));
        SerializedProperty primaryButtonData = property.FindPropertyRelative(nameof(primaryButtonData));
        SerializedProperty hasSecondaryButton = property.FindPropertyRelative(nameof(hasSecondaryButton));
        SerializedProperty secondaryButtonData = property.FindPropertyRelative(nameof(secondaryButtonData));

        // Set height for one control
        float height = EditorExtensions.StandardControlHeight;

        if (property.isExpanded)
        {
            // Add height for background
            height += EditorExtensions.StandardControlHeight;
            // Add height for message
            height += EditorExtensions.StandardControlHeight;
            // Add height for starting anchor position
            height += EditorGUI.GetPropertyHeight(startingAnchorPosition);
            // Add height for starting animation ease
            height += EditorExtensions.StandardControlHeight;
            // Add height for ending animation ease
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
