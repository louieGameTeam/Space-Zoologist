using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuizTemplate))]
public class QuizTemplateEditor : Editor
{
    #region Editor Overrides
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the default inspector
        DrawDefaultInspector();

        // Setup the template on the example quiz
        SerializedProperty exampleQuiz = serializedObject.FindProperty(nameof(exampleQuiz));
        SerializedProperty template = exampleQuiz.FindPropertyRelative(nameof(template));
        template.objectReferenceValue = target;

        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
