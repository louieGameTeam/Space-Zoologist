using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

/// <summary>
/// Handles conversations
/// </summary>
public class DialogueManager : MonoBehaviour
{
    // The dialogues of the NPC
    [SerializeField] private NPCConversation normalConversation;
    [SerializeField] private NPCConversation eventTriggeredConversation;
    
    // Update is called once per frame
    public void StartConversation()
    {
        ConversationManager.Instance.StartConversation(normalConversation);
    }
}
