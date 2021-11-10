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
}
