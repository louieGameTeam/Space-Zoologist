﻿using System.Collections;
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
    private NPCConversation startingConversation = default;
    private NPCConversation defaultConversation = default;
    [SerializeField] GameObject ConversationManagerGameObject = default;
    [SerializeField] private GameObject DialogueButton = default;
    [SerializeField] private MenuManager menuManager = default;
    private Queue<NPCConversation> queuedConversations = new Queue<NPCConversation>();

    private bool ContinueSpeech = false;


    /// <summary>
    /// Initialize stuffs here
    /// </summary>
    public void Initialize()
    {
        startingConversation = GameManager.Instance.LevelData.StartingConversation;
        defaultConversation = GameManager.Instance.LevelData.DefaultConversation;
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
            StartNewConversation();
        }
    }

    private void ConversationEnded()
    {
        ContinueSpeech = false;
        ConversationManagerGameObject.SetActive(false);
        menuManager.ToggleUI(true);
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
                UpdateCurrentDialogue();
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
        menuManager.ToggleUI(false);
        ConversationManagerGameObject.SetActive(true);
        ConversationManager.Instance.StartConversation(currentDialogue);
        ContinueSpeech = true;
    }
}