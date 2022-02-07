using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizQuestionOrPool
{
    #region Public Properties
    public bool IsPool => isPool;
    public int QuestionCount => isPool ? pool.QuestionsToPick : 1;
    public bool Static => isPool ? pool.Static : true;
    public bool Dynamic => !Static;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("If true, this represents a pool of questions to randomly pick from. " +
        "If false, this is just a single question")]
    private bool isPool;
    [SerializeField]
    [Tooltip("Question to ask in the quiz")]
    private QuizQuestion question;
    [SerializeField]
    [Tooltip("Pool of questions to pick from")]
    private QuizQuestionPool pool;
    #endregion

    #region Public Methods
    public QuizQuestion[] GetQuestions()
    {
        // If this is a pool, return what the pool randomly picks
        if (isPool) return pool.PickQuestions();
        // If this is not a pool, return an array with the question as the only element inside
        else return new QuizQuestion[] { question };
    }
    #endregion
}
