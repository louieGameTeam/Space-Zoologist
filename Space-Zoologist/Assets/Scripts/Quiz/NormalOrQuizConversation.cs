using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

[System.Serializable]
public class NormalOrQuizConversation
{
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
            QuizConversation quiz = Object.Instantiate(quizConversation);
            quiz.Setup();
        }
        else dialogueManager.SetNewDialogue(normalConversation);
    }
    #endregion
}
