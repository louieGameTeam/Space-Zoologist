using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using UnityEngine.UI;

/// <summary>
/// Handles conversations
/// </summary>
public class DialogueManager : MonoBehaviour
{
    private NPCConversation currentDialogue = default;
    [SerializeField] private bool HideNPC = default;
    [SerializeField] private NPCConversation startingConversation = default;
    [SerializeField] private NPCConversation defaultConversation = default;
    [SerializeField] GameObject ConversationManagerGameObject = default;
    [SerializeField] private GameObject DialogueButton = default;
    private Queue<NPCConversation> queuedConversations = new Queue<NPCConversation>();

    private bool ContinueSpeech = false;

    /// <summary>
    /// Initialize stuffs here
    /// </summary>
    public void Initialize()
    {
        ConversationManager.OnConversationEnded = ConversationEnded;
        if (this.startingConversation != null)
        {
            currentDialogue = this.startingConversation;
        }
        else
        {
            UpdateCurrentDialogue();
        }
        if (ConversationManager.Instance != null)
        {
            ConversationManager.Instance.StartConversation(currentDialogue);
            ContinueSpeech = true;
        }
    }

    private void ConversationEnded()
    {
        ContinueSpeech = false;
        ConversationManagerGameObject.SetActive(false);
    }

    public void SetNewDialogue(NPCConversation newDialogue)
    {
        if (queuedConversations.Contains(newDialogue))
        {
            return;
        }
        queuedConversations.Enqueue(newDialogue);
    }

    public void UpdateCurrentDialogue()
    {
        if (queuedConversations.Count > 0)
        {
            currentDialogue = queuedConversations.Dequeue();
        }
        else
        {
            currentDialogue = defaultConversation;
        }
    }

    /// <summary>
    /// Start the interactive conversation via key press or something else
    /// </summary>
    public void StartInteractiveConversation()
    {
        if (ContinueSpeech)
        {
            ConversationManagerGameObject.SetActive(!ConversationManagerGameObject.activeSelf);
        }
        else
        {
            if (!ConversationManagerGameObject.activeSelf)
            {
                StartNewConversation();
            }
            else
            {
                ConversationManagerGameObject.SetActive(false);
            }
        }
    }

    private void StartNewConversation()
    {
        ConversationManagerGameObject.SetActive(true);
        UpdateCurrentDialogue();
        ConversationManager.Instance.StartConversation(currentDialogue);
        ContinueSpeech = true;
    }
}