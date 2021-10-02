using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NormalOrQuizConversation))]
public class NormalOrQuizConversationDrawer : PropertyDrawer
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
            }
            else EditorGUI.PropertyField(position, property.FindPropertyRelative("normalConversation"));

            // Restore old indent
            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorExtensions.StandardControlHeight;
        if(property.isExpanded)
        {
            height *= 3f;
        }
        return height;
    }
    #endregion
}
