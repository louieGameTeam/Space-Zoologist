using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TestAndMetricsConfig))]
public class TestAndMetricsModelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty scaffold = property.FindPropertyRelative(nameof(scaffold));
        SerializedProperty initialTexts = property.FindPropertyRelative(nameof(initialTexts));

        // Set height for only control
        position.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Do a foldout
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
        position.y += position.height;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            NotebookEditorUtility.EnclosureScaffoldAndArrayField(position, scaffold, initialTexts);
            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty scaffold = property.FindPropertyRelative(nameof(scaffold));
        SerializedProperty initialTexts = property.FindPropertyRelative(nameof(initialTexts));

        // Set height for one control
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            // Add in height for the scaffold and the foldout of the initial texts
            height += EditorGUI.GetPropertyHeight(scaffold);
            height += NotebookEditorUtility.GetSizeControlledArrayHeight(initialTexts);
        }

        return height;
    }
}
