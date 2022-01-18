using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LevelEndingData))]
public class LevelEndingDataDrawer : PropertyDrawer
{
    #region Public Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Set height for just one control
        position.height = EditorExtensions.StandardControlHeight;

        // Put in a foldout for the property
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;

        if(property.isExpanded)
        {
            // Increase indent
            EditorGUI.indentLevel++;

            // Get the is quiz property
            SerializedProperty isQuiz = property.FindPropertyRelative(nameof(isQuiz));

            // Edit is quiz boolean
            EditorGUI.PropertyField(position, isQuiz);
            position.y += position.height;

            if (isQuiz.boolValue)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("quizConversation"));
                position.y += position.height;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("nextLevelIDs"));
            }
            else
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("normalConversation"));
                position.y += position.height;

                EditorGUI.PropertyField(position, property.FindPropertyRelative("nextLevelID"));
            }

            // Restore old indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get the is quiz property
        SerializedProperty isQuiz = property.FindPropertyRelative(nameof(isQuiz));

        float height = EditorExtensions.StandardControlHeight;
        if(property.isExpanded)
        {
            height *= 3f;

            // If this is a quiz then add height for the list of level names
            if (isQuiz.boolValue)
            {
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("nextLevelIDs"));
            }
            else height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("nextLevelID"));
        }
        return height;
    }
    #endregion
}
