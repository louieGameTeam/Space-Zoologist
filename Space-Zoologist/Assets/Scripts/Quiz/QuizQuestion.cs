using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    #region Public Properties
    public QuizCategory Category => category;
    public string Question => question;
    public QuizOption[] Options => options;
    public int MaxPossibleScore => CollectionExtensions.IsNullOrEmpty(options) ? 0 : options.Max(o => o.Weight);
    public int MinPossibleScore => CollectionExtensions.IsNullOrEmpty(options) ? 0 : options.Min(o => o.Weight);
    public int ScoreSpread => MaxPossibleScore - MinPossibleScore;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [TextArea(3, 10)]
    [Tooltip("Question to ask")]
    private string question;
    [SerializeField]
    [Tooltip("Category that this quiz questions tests")]
    private QuizCategory category;
    [SerializeField]
    [Tooltip("List of options to choose to answer the question")]
    private QuizOption[] options;
    #endregion

    #region Constructors
    public QuizQuestion(string question, QuizCategory category, params QuizOption[] options)
    {
        this.question = question;
        this.category = category;
        this.options = options;
    }
    #endregion

    #region Public Methods
    public float RelativeGrade(int optionIndex)
    {
        if (optionIndex >= 0 && optionIndex < options.Length)
        {
            return (float)options[optionIndex].Weight / ScoreSpread;
        }
        else throw new System.IndexOutOfRangeException($"{nameof(QuizQuestion)}: " +
            $"cannot get a relative grade for index '{optionIndex}'. " +
            $"Total options: {options.Length}");
    }

    public int ReceivedGrade(int optionIndex)
    {
        if (optionIndex >= 0 && optionIndex < options.Length)
        {
            return options[optionIndex].Weight;
        }
        else throw new System.IndexOutOfRangeException($"{nameof(QuizQuestion)}: " +
                                                       $"cannot get a score for index '{optionIndex}'. " +
                                                       $"Total options: {options.Length}");
    }
    #endregion
}
