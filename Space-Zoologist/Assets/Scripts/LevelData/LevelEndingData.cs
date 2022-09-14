using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

[System.Serializable]
public class LevelEndingData
{
    #region Public Typdefs
    [System.Serializable]
    public class LevelIDArray
    {
        public LevelID[] levelIDs;
        public LevelID Get(QuizGrade grade) => levelIDs[(int)grade];
    }
    #endregion

    #region Public Properties
    public bool IsQuiz => isQuiz;
    public QuizConversation QuizConversation => quizConversation;
    public QuizConversation ActiveQuizConversation => activeQuizConversation;
    public NPCConversation ActiveConversation => activeConversation;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("If true, this uses a quiz conversation, otherwise use a normal NPCConversation")]
    private bool isQuiz = false;
    
    [SerializeField]
    [Tooltip("The normal NPCConversation to speak")]
    private NPCConversation normalConversation = null;
    [SerializeField]
    [Tooltip("Next level to load after the normal conversation finishes")]
    private LevelID nextLevelID = LevelID.Invalid;
    
    [SerializeField]
    [Tooltip("Conversation to speak if this is a quiz conversation")]
    private QuizConversation quizConversation = null;
    [SerializeField]
    [Tooltip("Level to load after this level given the score on the quiz")]
    [EditArrayWrapperOnEnum("levelIDs", typeof(QuizGrade))]
    private LevelIDArray nextLevelIDs = null;
    #endregion

    #region Private Fields
    private QuizConversation activeQuizConversation;
    private NPCConversation activeConversation;
    #endregion

    #region Public Methods
    public void SayEndingConversation()
    {
        if (isQuiz)
        {
            // The quiz conversation won't work unless it is instantiated,
            // because the conversation might modify the object it is on,
            // which Unity cannot allow for a prefab
            activeQuizConversation = Object.Instantiate(quizConversation);
            activeQuizConversation.Setup();
        }
        else
        {
            // This is used so that the game over controller can dynamically add
            // game over events to a non-prefab instance of the NPCConversation
            activeConversation = normalConversation.InstantiateAndSay();
        }
    }
    public LevelID GetNextLevelID()
    {
        if (isQuiz)
        {
            // If you have an active quiz with a completed quiz instance
            // then get the next level for that quiz grade
            if (activeQuizConversation && activeQuizConversation.CurrentQuiz != null && activeQuizConversation.CurrentQuiz.Completed)
            {
                return nextLevelIDs.Get(activeQuizConversation.CurrentQuiz.Grade);
            }
            else return LevelID.Invalid;
        }
        else return nextLevelID;
    }
    #endregion
}
