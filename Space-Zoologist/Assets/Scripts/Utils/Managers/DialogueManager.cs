using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

/// <summary>
/// Handles conversations
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [SerializeField] PauseManager PauseManager = default;
    // The interactive dialogues of the NPC
    [SerializeField] private NPCConversation interactiveConversation = default;
    // The event dialogue
    [SerializeField] private NPCConversation eventConversation = default;
    // The fully-scripted dialogue
    [SerializeField] private NPCConversation scriptedConversation = default;
    // The starting dialogue
    [SerializeField] private NPCConversation startingConversation = default;
    [SerializeField] GameObject ConversationManagerGameObject = default;
    [SerializeField] public GameObject StoreButtonsGameObject = default;

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
            StoreButtonsGameObject.SetActive(false);
            ConversationManager.Instance.StartConversation(this.startingConversation);
        }
    }

    private void ConversationEnded()
    {
        ContinueSpeech = false;
        StoreButtonsGameObject.SetActive(true);
        ConversationManagerGameObject.SetActive(false);
    }

    /// <summary>
    /// Start the interactive conversation via key press or something else
    /// </summary>
    public void StartInteractiveConversation()
    {
        if (ContinueSpeech)
        {
            ConversationManagerGameObject.SetActive(!ConversationManagerGameObject.activeSelf);
            StoreButtonsGameObject.SetActive(!StoreButtonsGameObject.activeSelf);
        }
        else
        {
            if (!ConversationManagerGameObject.activeSelf)
            {
                ConversationManagerGameObject.SetActive(true);
                ConversationManager.Instance.StartConversation(this.interactiveConversation);
                ContinueSpeech = true;
                StoreButtonsGameObject.SetActive(false);
            }
            else
            {
                ConversationManagerGameObject.SetActive(false);
                StoreButtonsGameObject.SetActive(true);
            }
        }
    }
}