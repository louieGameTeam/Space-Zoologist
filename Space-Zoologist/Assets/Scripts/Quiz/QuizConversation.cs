using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class QuizConversation : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Conversation to say at the beginning")]
    private NPCConversation openingConversation;
    [SerializeField]
    [Tooltip("Object used to build the NPCConversation object for the quiz")]
    private QuizConversationBuilder quizBuilder;
    [SerializeField]
    [Tooltip("Game object that the quiz conversation will be attached to")]
    private GameObject quizObject;
    #endregion

    #region Public Methods
    public void Setup()
    {
        if (GameManager.Instance)
        {
            DialogueManager dialogueManager = GameManager.Instance.m_dialogueManager;

            // First, say the opening conversation
            dialogueManager.SetNewDialogue(openingConversation);

            // Then, say the quiz part of the conversation
            NPCConversation quiz = quizBuilder.Create(quizObject, dialogueManager).conversation;
            dialogueManager.SetNewDialogue(quiz);
        }
    }
    // This needs to be directly referenced by the NPCConversation events on the last nodes of each conversation
    public void LoadLevel(string levelName)
    {
        LevelDataLoader levelLoader = FindObjectOfType<LevelDataLoader>();
        if (levelLoader) levelLoader.LoadLevel(levelName);
    }
    #endregion
}
