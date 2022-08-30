using DialogueEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    [SerializeField] private NPCConversation HelpDialogue = default;

    public void StartConversation () {
        GameManager.Instance.m_dialogueManager.SetNewDialogue (HelpDialogue);
        GameManager.Instance.m_dialogueManager.StartInteractiveConversation ();
    }
}
