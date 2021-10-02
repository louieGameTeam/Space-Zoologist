using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizGradingRubric
{
    #region Public Properties
    public float[] Percentages => percentages;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("List of grade percentages for each grade level")]
    private float[] percentages;
    #endregion

    #region Public Methods
    public float GetLowerBound(QuizGradeType grade)
    {
        int shiftedIndex = ((int)grade) - 1;

        if (shiftedIndex >= 0) return percentages[shiftedIndex];
        else return 0;
    }
    public QuizGradeType Grade(int score, int totalScore)
    {
        float percent = (float)score / totalScore;

        for(int i = 0; i < percentages.Length; i++)
        {
            if (percent < percentages[i]) return (QuizGradeType)i;
        }

        // If we get to this point we know we got the highest grade
        return (QuizGradeType)percentages.Length;
    }
    #endregion
}
