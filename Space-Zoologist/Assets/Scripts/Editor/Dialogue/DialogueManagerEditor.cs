using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DialogueEditor;

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    #region Private Fields
    private DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();
    #endregion

    #region Public Methods
    public override void OnInspectorGUI()
    {
        // Update the object and draw the default inspector
        serializedObject.Update();
        DrawDefaultInspector();

        // Get the manager
        ConversationManager manager = FindObjectOfType<ConversationManager>();

        // If a manager was found, setup the tracker to drive the rect transforms of the name text and dialogue text
        if (manager)
        {
            tracker = new DrivenRectTransformTracker();
            tracker.Add(target, manager.NameText.rectTransform, DrivenTransformProperties.AnchoredPositionY);
            tracker.Add(target, manager.DialogueText.rectTransform, DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta);
            tracker.Add(target, manager.DialoguePanel, DrivenTransformProperties.SizeDeltaY);

            // Use a button to toggle the npc
            if (GUILayout.Button("Toggle NPC"))
            {
                manager.NpcIcon.enabled = !manager.NpcIcon.enabled;

                // Validate the dialogue manager when the button is pressed
                DialogueManager dialogue = target as DialogueManager;
                dialogue.OnValidate();
            }
        }

        // Apply modified properties
        serializedObject.ApplyModifiedProperties();
    }
    #endregion
}
