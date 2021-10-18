using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuizConversationDebugger))]
public class QuizConversationDebuggerEditor : Editor
{
    #region Public Methods
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button(new GUIContent("Setup NPCConversation")))
        {
            QuizConversationDebugger debugger = target as QuizConversationDebugger;
            debugger.CreateConversation();
        }
    }
    #endregion
}
