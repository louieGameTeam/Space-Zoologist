using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenericWindow))]
public class GenericWindowEditor : Editor
{
    #region Editor Overrides
    public override void OnInspectorGUI()
    {
        // Get all of the serialized property on the object
        SerializedProperty window = serializedObject.FindProperty(nameof(window));
        SerializedProperty overlay = serializedObject.FindProperty(nameof(overlay));
        SerializedProperty fadeTime = serializedObject.FindProperty(nameof(fadeTime));
        SerializedProperty windowAnimateTime = serializedObject.FindProperty(nameof(windowAnimateTime));
        SerializedProperty openingPosition = serializedObject.FindProperty(nameof(openingPosition));
        SerializedProperty restingPosition = serializedObject.FindProperty(nameof(restingPosition));
        SerializedProperty openingEase = serializedObject.FindProperty(nameof(openingEase));
        SerializedProperty closingEase = serializedObject.FindProperty(nameof(closingEase));
        SerializedProperty primaryButton = serializedObject.FindProperty(nameof(primaryButton));
        SerializedProperty hasSecondaryButton = serializedObject.FindProperty(nameof(hasSecondaryButton));
        SerializedProperty secondaryButton = serializedObject.FindProperty(nameof(secondaryButton));
        SerializedProperty windowClosedEvent = serializedObject.FindProperty(nameof(windowClosedEvent));

        // Update the object
        serializedObject.Update();

        // Edit all the properties
        EditorGUILayout.PropertyField(window);
        EditorGUILayout.PropertyField(overlay);
        EditorGUILayout.PropertyField(fadeTime);
        EditorGUILayout.PropertyField(windowAnimateTime);
        EditorGUILayout.PropertyField(openingPosition);
        EditorGUILayout.PropertyField(restingPosition);
        EditorGUILayout.PropertyField(openingEase);
        EditorGUILayout.PropertyField(closingEase);
        EditorGUILayout.PropertyField(primaryButton);
        EditorGUILayout.PropertyField(hasSecondaryButton);

        // If we have a secondary button then layout the property for the secondary button
        if(hasSecondaryButton.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(secondaryButton);
            EditorGUI.indentLevel--;
        }

        // Edit the window closed event
        EditorGUILayout.PropertyField(windowClosedEvent);

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
