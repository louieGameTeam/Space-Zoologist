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
    [SerializeField] private NPCConversation startingConversation = default;
    [SerializeField] private NPCConversation defaultConversation = default;
    [SerializeField] GameObject ConversationManagerGameObject = default;
    [SerializeField] private GameObject DialogueButton = default;
    [SerializeField] private Image DefaultImage = default;
    [SerializeField] private Image NotificationImage = default;
    private Queue<NPCConversation> queuedConversations = new Queue<NPCConversation>();

    private bool ContinueSpeech = false;

    /// <summary>
    /// Initialize stuffs here
    /// </summary>
    private void Start()
    {
        ConversationManager.OnConversationEnded = ConversationEnded;
        if (startingConversation) {
            ContinueSpeech = true;
            currentDialogue = this.startingConversation;
            ConversationManager.Instance.StartConversation(this.startingConversation);
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
        this.DialogueButton.GetComponent<Button>().targetGraphic = NotificationImage;
        NotificationImage.gameObject.SetActive(true);
        queuedConversations.Enqueue(newDialogue);
    }

    public void UpdateCurrentDialogue()
    {
        if (queuedConversations.Count > 0)
        {
            currentDialogue = queuedConversations.Dequeue();
            if (queuedConversations.Count == 0)
            {
                this.DialogueButton.GetComponent<Button>().targetGraphic = DefaultImage;
                NotificationImage.gameObject.SetActive(false);
            }
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