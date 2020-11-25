using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

/// <summary>
/// Handles conversations, it is pretty useless at this moment
/// </summary>
public class DialogueManager : MonoBehaviour
{
    // The starting dialogue
    [SerializeField] private NPCConversation startingConversation = default;
    // The interactive dialogues of the NPC
    [SerializeField] private NPCConversation interactiveConversation = default;
    // The event dialogue
    [SerializeField] private NPCConversation eventConversation = default;
    // The fully-scripted dialogue
    [SerializeField] private NPCConversation scriptedConversation = default;

    [SerializeField] PauseManager pauseManager = default;
    /// <summary>
    /// Initialize stuffs here
    /// </summary>
    private void Start()
    {
        if (startingConversation) {
            pauseManager.TryToPause();
            ConversationManager.Instance.StartConversation(this.startingConversation);
        }
    }

    /// <summary>
    /// Start the interactive conversation via key press or something else
    /// </summary>
    public void StartInteractiveConversation()
    {
        pauseManager.TryToPause();
        ConversationManager.Instance.StartConversation(this.interactiveConversation);
    }
}
