using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

/// <summary>
/// Handles conversations
/// </summary>
public class DialogueManager : MonoBehaviour
{
    // The dialogue of the NPC
    [SerializeField] private NPCConversation Conversation;

    // Update is called once per frame
    public void StartConversation()
    {
       ConversationManager.Instance.StartConversation(Conversation);
    }
}
