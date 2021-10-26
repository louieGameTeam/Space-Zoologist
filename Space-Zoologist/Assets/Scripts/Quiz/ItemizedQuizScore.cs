using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A quiz score maps quiz categories to the scores associated with the category
/// </summary>
public class ItemizedQuizScore
{
    #region Public Properties
    public int TotalScore => scores.Select(kvp => kvp.Value).Sum();
    #endregion

    #region Private Editor Fields
    // Maps the quiz category to the score received for that category
    private Dictionary<QuizCategory, int> scores = new Dictionary<QuizCategory, int>();
    #endregion

    #region Public Methods
    public int Get(QuizCategory category) => scores[category];
    public void Set(QuizCategory category, int score) => scores[category] = score;
    #endregion
}
