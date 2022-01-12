using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizQuestionPool
{
    #region Public Properties
    public int QuestionsToPick => questionsToPick;
    // The question pool is static if each random selection 
    // always yeilds the same questions (though there is no guarantee
    // that they will always be in the same order)
    public bool Static => questionsToPick == questionPool.Length;
    public bool Dynamic => !Static;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Number of questions to randomly pick from this question pool")]
    private int questionsToPick = 1;
    [SerializeField]
    [Tooltip("Pool of questions to pick from")]
    private QuizQuestion[] questionPool;
    #endregion

    #region Public Methods
    public QuizQuestion[] PickQuestions()
    {
        // Create an array of picked indices
        QuizQuestion[] questionPicks = new QuizQuestion[questionsToPick];
        // Skip any questions in the question pool that already exist 
        // in the list of picked questions
        List<QuizQuestion> questionPool = new List<QuizQuestion>(this.questionPool);

        // Randomly pick a question for each of the number of questions to pick
        for (int i = 0; i < questionsToPick; i++)
        {
            int pick = UnityEngine.Random.Range(0, questionPool.Count);
            questionPicks[i] = questionPool[pick];
            questionPool.RemoveAt(pick);
        }

        return questionPicks;
    }
    #endregion
}
