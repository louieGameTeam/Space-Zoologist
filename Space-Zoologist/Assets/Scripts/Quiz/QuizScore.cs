using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizScore
{
    #region Public Typedef
    // Wrapper for the array on enum attribute
    [System.Serializable]
    public class ScoreData
    {
        public int[] scores;
        public ScoreData()
        {
            scores = new int[System.Enum.GetNames(typeof(QuizScoreType)).Length];
        }
    }
    #endregion

    #region Public Properties
    public int TotalScore => scoreData.scores.Sum();
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Score data for this quiz score")]
    [EditArrayWrapperOnEnum("scores", typeof(QuizScoreType))]
    private ScoreData scoreData;
    #endregion

    #region Constructors
    public QuizScore()
    {
        scoreData = new ScoreData();
    }
    #endregion

    #region Public Methods
    public int Get(QuizScoreType type) => scoreData.scores[(int)type];
    public void Set(QuizScoreType type, int score) => scoreData.scores[(int)type] = score;
    #endregion
}
