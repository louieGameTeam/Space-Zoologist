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
    [SerializeField] PauseManager PauseManager = default;
    // The interactive dialogues of the NPC
    [SerializeField] private NPCConversation interactiveConversation = default;
    private NPCConversation currentDialogue = default;
    // The starting dialogue
    [SerializeField] private NPCConversation startingConversation = default;
    [SerializeField] GameObject ConversationManagerGameObject = default;
    [SerializeField] private GameObject DialogueButton = default;
    [SerializeField] private Sprite DefaultImage = default;
    [SerializeField] private Sprite NotificationImage = default;

    private bool ContinueSpeech = false;

    /// <summary>
    /// Initialize stuffs here
    /// </summary>
    private void Start()
    {
        ConversationManager.OnConversationEnded = ConversationEnded;
        if (startingConversation) {
            PauseManager.Pause();
            ContinueSpeech = true;
            currentDialogue = this.startingConversation;
            ConversationManager.Instance.StartConversation(this.startingConversation);
        }
    }

    private void ConversationEnded()
    {
        ContinueSpeech = false;
        ConversationManagerGameObject.SetActive(false);
        SetDefaultConversation();
    }

    public void SetNewDialogue(NPCConversation newDialogue)
    {
        this.DialogueButton.GetComponent<Image>().sprite = NotificationImage;
        currentDialogue = newDialogue;
    }

    public void SetDefaultConversation()
    {
        currentDialogue = interactiveConversation;
    }

    /// <summary>
    /// Start the interactive conversation via key press or something else
    /// </summary>
    public void StartInteractiveConversation()
    {
        this.DialogueButton.GetComponent<Image>().sprite = DefaultImage;
        if (ContinueSpeech)
        {
            ConversationManagerGameObject.SetActive(!ConversationManagerGameObject.activeSelf);
        }
        else
        {
            if (!ConversationManagerGameObject.activeSelf)
            {
                ConversationManagerGameObject.SetActive(true);
                ConversationManager.Instance.StartConversation(currentDialogue);
                ContinueSpeech = true;
            }
            else
            {
                ConversationManagerGameObject.SetActive(false);
            }
        }
    }
}