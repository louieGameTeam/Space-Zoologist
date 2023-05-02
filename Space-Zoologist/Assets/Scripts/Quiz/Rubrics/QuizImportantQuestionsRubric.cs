using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Type of rubric that considers important questions into grading scheme
/// </summary>
[CreateAssetMenu(menuName = "Quiz Rubric/Important Questions Included Rubric", fileName = "ImportantQuestionsRubric")]
public class QuizImportantQuestionsRubric : QuizGradingRubric
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Points must be more than this total percentage to pass")]
    [Range(0f, 1f)]
    private float percentageToPassImportantQuestions = 0.5f;
    [SerializeField]
    [Tooltip("Points correct you must get to pass the unimportant categories")]
    [Range(0f, 1f)]
    private float percentageToPassUnimportantQuestions = 0.65f;
    #endregion
    
    public override QuizGrade EvaluateGrade(int importantScore, int importantMaxScore, int unimportantScore, int unimportantMaxScore)
    {
        if (PassedImportantQuestions(importantScore, importantMaxScore))
        {
            if (PassedUnimportantQuestions(unimportantScore, unimportantMaxScore))
            {
                return QuizGrade.Excellent;
            }

            return QuizGrade.Acceptable;
        }
        else
        {
            return QuizGrade.Poor;
        }
    }
    
    public bool PassedImportantQuestions(int score, int maxScore) => Grade(score, maxScore, percentageToPassImportantQuestions);
    public bool PassedUnimportantQuestions(int score, int maxScore) => Grade(score, maxScore, percentageToPassUnimportantQuestions);

    #region Private Methods
    

    private bool Grade(int score, int maxScore, float percentageToPass)
    {
        if (maxScore > 0)
        {
            float percentageCorrect = (float)score / maxScore;
            return percentageCorrect > percentageToPass;
        }
        // Assume you passed if the max score is zero
        else return true;
    }
    #endregion

}
