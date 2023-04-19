using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of rubric that ignores important questions and grades using simple percentages
/// </summary>
[CreateAssetMenu(menuName = "Quiz Rubric/Simple Rubric", fileName = "SimpleRubric")]
public class QuizSimpleRubric : QuizGradingRubric
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Percentage required to receive an 'Excellent'")]
    [Range(0f, 1f)]
    private float excellentPercentage = 0.5f;
    [SerializeField]
    [Tooltip("Percentage required to receive an 'Acceptable'")]
    [Range(0f, 1f)]
    private float acceptablePercentage = 0.5f;
    #endregion
    
    public override QuizGrade EvaluateGrade(int importantScore, int importantMaxScore, int unimportantScore, int unimportantMaxScore)
    {
        float percentage = GetTotalPercentage(importantScore, importantMaxScore, unimportantScore, unimportantMaxScore);

        if (percentage >= excellentPercentage)
        {
            return QuizGrade.Excellent;
        }
        else if (percentage >= acceptablePercentage)
        {
            return QuizGrade.Acceptable;
        }

        return QuizGrade.Poor;
    }

    public float GetTotalPercentage(int importantScore, int importantMaxScore, int unimportantScore, int unimportantMaxScore)
    {
        int totalScore = importantScore + unimportantScore;
        int totalMaxScore = importantMaxScore + unimportantMaxScore;

        // If no max score, give 100 percent
        if (totalMaxScore == 0)
            return 1;

        return (float)totalScore / totalMaxScore;
    }
}
