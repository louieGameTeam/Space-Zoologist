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
    public float GetLowerBound(QuizGrade grade)
    {
        int shiftedIndex = ((int)grade) - 1;

        if (shiftedIndex >= 0) return percentages[shiftedIndex];
        else return 0;
    }
    #endregion
}
