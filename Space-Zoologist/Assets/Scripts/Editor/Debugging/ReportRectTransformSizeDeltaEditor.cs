using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ReportRectTransformSizeDelta))]
public class ReportRectTransformSizeDeltaEditor : Editor
{
    #region Public Methods
    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();

        // If a button is pressed then report size delta
        if (GUILayout.Button("Report"))
        {
            (target as ReportRectTransformSizeDelta).Report();
        }
    }
    #endregion
}
