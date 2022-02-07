using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(QuizQuestionOrPool))]
public class QuizQuestionOrPoolDrawer : PropertyDrawer
{
    #region Property Drawer Overrides
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the sub properties of this property
        SerializedProperty isPool = property.FindPropertyRelative(nameof(isPool));
        SerializedProperty question = property.FindPropertyRelative(nameof(question));
        SerializedProperty pool = property.FindPropertyRelative(nameof(pool));

        // Layout the foldout
        property.isExpanded = EditorGUIAuto.Foldout(ref position, property.isExpanded, label);

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUIAuto.PropertyField(ref position, isPool, true);

            // Display either the pool or the question depending on the is pool bool value
            if (isPool.boolValue) EditorGUIAuto.PropertyField(ref position, pool, true);
            else EditorGUIAuto.PropertyField(ref position, question, true);

            EditorGUI.indentLevel--;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get the sub properties of this property
        SerializedProperty isPool = property.FindPropertyRelative(nameof(isPool));
        SerializedProperty question = property.FindPropertyRelative(nameof(question));
        SerializedProperty pool = property.FindPropertyRelative(nameof(pool));

        // Set the height for just the foldout control
        float height = EditorExtensions.StandardControlHeight;

        if (property.isExpanded)
        {
            // Add space for is pool bool
            height += EditorExtensions.StandardControlHeight;

            if (isPool.boolValue) height += EditorGUI.GetPropertyHeight(pool, true);
            else height += EditorGUI.GetPropertyHeight(question, true);
        }

        return height;
    }
    #endregion
}
