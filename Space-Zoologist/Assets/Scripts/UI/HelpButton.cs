using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    [SerializeField] private NPCConversation HelpDialogue = default;

    public void StartConversation ()
    {
        // If something is already playing don't start
        if (GameManager.Instance.m_dialogueManager.ConversationGameObjectActive)
            return;
        
        GameManager.Instance.m_dialogueManager.SetNewDialogue (HelpDialogue);
        GameManager.Instance.m_dialogueManager.StartInteractiveConversation ();
    }
}
