using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuizTemplate))]
public class QuizTemplateEditor : Editor
{
    #region Public Methods
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("questions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gradingRubric"));

        // Disable editing of the bounds
        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreBoundaries"));
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
