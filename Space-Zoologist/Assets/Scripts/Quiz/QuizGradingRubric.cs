using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizGradingRubric
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Points correct you must get to pass the important categories")]
    private int scoreToPassImportantQuestions;
    [SerializeField]
    [Tooltip("Points correct you must get to pass the unimportant categories")]
    private int scoreToPassUnimportantQuestions;
    #endregion

    #region Public Methods
    public bool PassedImportantQuestions(int score) => Grade(score, scoreToPassImportantQuestions);
    public bool PassedUnimportantQuestions(int score) => Grade(score, scoreToPassUnimportantQuestions);
    #endregion

    #region Private Methods
    private bool Grade(int score, int scoreToPass) => score >= scoreToPass;
    #endregion
}
