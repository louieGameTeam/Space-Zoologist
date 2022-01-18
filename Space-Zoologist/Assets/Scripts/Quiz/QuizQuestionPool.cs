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
    private int questionsToPick = 0;
    [SerializeField]
    [Tooltip("Pool of questions to pick from")]
    private QuizQuestion[] questionPool;
    #endregion

    #region Public Methods
    public QuizQuestion[] PickQuestions()
    {
        if (questionPool != null && questionPool.Length > 0)
        {
            // Create an array of picked indices
            QuizQuestion[] questionPicks = new QuizQuestion[questionsToPick];
            // Skip any questions in the question pool that already exist 
            // in the list of picked questions
            List<QuizQuestion> remainingPool = new List<QuizQuestion>(questionPool);

            // Randomly pick a question for each of the number of questions to pick
            for (int i = 0; i < questionsToPick; i++)
            {
                int pick = UnityEngine.Random.Range(0, remainingPool.Count);
                questionPicks[i] = remainingPool[pick];
                remainingPool.RemoveAt(pick);
            }

            return questionPicks;
        }
        else return new QuizQuestion[0];
    }
    #endregion
}
