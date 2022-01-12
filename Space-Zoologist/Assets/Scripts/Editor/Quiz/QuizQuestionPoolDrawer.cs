using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(QuizQuestionPool))]
public class QuizQuestionPoolDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the properties
        SerializedProperty questionsToPick = property.FindPropertyRelative(nameof(questionsToPick));
        SerializedProperty questionPool = property.FindPropertyRelative(nameof(questionPool));

        // Layout the foldout
        property.isExpanded = EditorGUIAuto.Foldout(ref position, property.isExpanded, label);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            // Display the slider only if there are questions to pick
            if (questionPool.arraySize >= 1)
            {
                questionsToPick.intValue = Mathf.Clamp(questionsToPick.intValue, 1, questionPool.arraySize);
                EditorGUIAuto.IntSlider(ref position, questionsToPick, 1, questionPool.arraySize);
            }
            // If there are no questions to pick then do not display an int slider
            else
            {
                questionsToPick.intValue = 0;
                EditorGUIAuto.PrefixedLabelField(ref position, new GUIContent(questionsToPick.displayName), "(No questions to pick)");
            }
            // Layout the question pool
            EditorGUIAuto.PropertyField(ref position, questionPool, true);

            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get the properties
        SerializedProperty questionsToPick = property.FindPropertyRelative(nameof(questionsToPick));
        SerializedProperty questionPool = property.FindPropertyRelative(nameof(questionPool));

        // Set height for the foldout
        float height = EditorExtensions.StandardControlHeight;

        // Add height for sub properties when expanded
        if (property.isExpanded)
        {
            height += EditorGUI.GetPropertyHeight(questionsToPick, true);
            height += EditorGUI.GetPropertyHeight(questionPool, true);
        }

        return height;
    }
    #endregion
}
