using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class QuizGradingRubric : ScriptableObject
{
    #region Private Editor Fields
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Percentage correct you must get to pass the important categories")]
    private float percentToPassImportantQuestions;
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("Percentage correct you must get to pass the unimportant categories")]
    private float percentToPassUnimportantQuestions;
    #endregion

    #region Public Methods
    public bool PassedImportantQuestions(int score, int totalScore) => Grade(score, totalScore, percentToPassImportantQuestions);
    public bool PassedUnimportantQuestions(int score, int totalScore) => Grade(score, totalScore, percentToPassUnimportantQuestions);
    #endregion

    #region Private Methods
    private bool Grade(int score, int totalScore, float percent)
    {
        if (totalScore > 0) return ((float)score / totalScore) >= percent;
        else return true;
    }
    #endregion
}
