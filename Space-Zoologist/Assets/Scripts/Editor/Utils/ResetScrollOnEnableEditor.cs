using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ResetScrollOnEnable))]
[CanEditMultipleObjects]
public class ResetScrollOnEnableEditor : Editor
{
    #region Editor Overrides
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        // Use a button to set the scroll rect from this game object
        if (GUILayout.Button("Set scroll rect"))
        {
            // Set the scroll rect on each target
            foreach (Object obj in targets)
            {
                // Get a scroll rect on this component
                ResetScrollOnEnable script = obj as ResetScrollOnEnable;
                ScrollRect scroll = script.GetComponent<ScrollRect>();

                // If the scroll rect exists then set it on this script
                if (scroll)
                {
                    script.SetScrollRect(scroll);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #region Public Methods
    [MenuItem("GameObject/Reset Scoll Rects on Enable", false, 0)]
    public static void ApplyToAllScrollRects()
    {
        GameObject[] targets = Selection.gameObjects;

        // Iterate over all selected game objects
        foreach(GameObject target in targets)
        {
            ScrollRect[] rects = target.GetComponentsInChildren<ScrollRect>(true);

            // Iterate over all scroll rects, setting up a script reset on each
            foreach (ScrollRect rect in rects)
            {
                ResetScrollOnEnable script = rect.GetComponent<ResetScrollOnEnable>();

                // If the script does not exist then add it
                if (!script) script = Undo.AddComponent<ResetScrollOnEnable>(rect.gameObject);

                // Record changes to the script
                Undo.RecordObject(script, $"Reset Scroll View On Enable");
                script.SetScrollRect(rect);
            }
        }

        // Mark the scenes as dirty so changes can be saved
        EditorSceneManager.MarkAllScenesDirty();
    }
    #endregion
}
