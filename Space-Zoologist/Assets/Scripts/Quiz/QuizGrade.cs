using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizGrade
{
    #region Public Typedefs
    [System.Serializable]
    public class QuizGradeArray
    {
        public QuizGradeType[] array;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of the grades that the player got on the quiz")]
    [EditArrayWrapperOnEnum(typeof(QuizScoreType))]
    private QuizGradeArray gradeData;
    #endregion

    #region Constructors
    public QuizGrade()
    {
        string[] quizScoreTypeNames = System.Enum.GetNames(typeof(QuizScoreType));
        gradeData = new QuizGradeArray()
        {
            array = new QuizGradeType[quizScoreTypeNames.Length]
        };
    }
    #endregion

    #region Public Methods
    public QuizGradeType Get(QuizScoreType scoreType) => gradeData.array[(int)scoreType];
    public void Set(QuizScoreType scoreType, QuizGradeType gradeType) => gradeData.array[(int)scoreType] = gradeType;
    #endregion
}
