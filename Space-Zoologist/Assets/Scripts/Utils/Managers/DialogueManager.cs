using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

/// <summary>
/// Handles conversations, it is pretty useless at this moment
/// </summary>
public class DialogueManager : MonoBehaviour
{
    // The interactive dialogues of the NPC
    [SerializeField] private NPCConversation interactiveConversation;
    // The event dialogue
    [SerializeField] private NPCConversation eventConversation;
    // The fully-scripted dialogue
    [SerializeField] private NPCConversation scriptedConversation;

    /// <summary>
    /// Initialize stuffs here
    /// </summary>
    private void Start()
    {
        if (scriptedConversation) {
            ConversationManager.Instance.StartConversation(this.scriptedConversation);
        }
    }

    /// <summary>
    /// Start the interactive conversation via key press or something else
    /// </summary>
    public void StartInteractiveConversation()
    {
        ConversationManager.Instance.StartConversation(this.interactiveConversation);
    }
}
