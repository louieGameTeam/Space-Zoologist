using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

[System.Serializable]
public class NormalOrQuizConversation
{
    #region Public Properties
    public QuizInstance CurrentQuiz => isQuiz ? quizConversation.CurrentQuiz : null;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("If true, this uses a quiz conversation, otherwise use a normal NPCConversation")]
    private bool isQuiz;
    [SerializeField]
    [Tooltip("The normal NPCConversation to speak")]
    private NPCConversation normalConversation;
    [SerializeField]
    [Tooltip("Conversation to speak if this is a quiz conversation")]
    private QuizConversation quizConversation;
    #endregion

    #region Public Methods
    public void Speak(DialogueManager dialogueManager)
    {
        if (isQuiz)
        {
            QuizConversation conversation = Object.Instantiate(quizConversation);
            conversation.Setup();
        }
        else dialogueManager.SetNewDialogue(normalConversation);
    }
    #endregion
}
