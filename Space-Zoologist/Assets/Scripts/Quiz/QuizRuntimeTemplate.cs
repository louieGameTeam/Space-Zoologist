using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizRuntimeTemplate
{
    #region Public Properties
    public QuizQuestion[] Questions => questions;
    public QuizTemplate Template => template;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Template to use to generate quistions for this runtime template")]
    private QuizTemplate template;
    #endregion

    #region Private Fields
    [SerializeField]
    [HideInInspector]
    private QuizQuestion[] questions;
    #endregion

    #region Constructors
    public QuizRuntimeTemplate(QuizTemplate template, params QuizQuestion[] additionalQuestions)
    {
        this.template = template;

        // Check if additional quiz questions were provided
        if (additionalQuestions != null && additionalQuestions.Length > 0)
        {
            // Create space for the template questions and additional questions
            QuizQuestion[] templateQuestions = template.GenerateQuestions();
            questions = new QuizQuestion[templateQuestions.Length + additionalQuestions.Length];

            // Copy the template questions into these questions
            System.Array.Copy(templateQuestions, questions, templateQuestions.Length);
            // Copy the additional questions into these questions
            System.Array.Copy(additionalQuestions, 0, questions, templateQuestions.Length, additionalQuestions.Length);
        }
        // If no additional questions were provided 
        else questions = template.GenerateQuestions();
    }
    #endregion

    #region Public Methods
    public int GetMaximumPossibleScoreInUnimportantCategories() => GetMaximumPossibleScorePerCategory().TotalScore - GetMaximumPossibleScoreInImportantCategories();
    public int GetMaximumPossibleScoreInImportantCategories()
    {
        int maxScore = 0;
        // Add the max score for each important category
        foreach (QuizCategory category in template.ImportantCategories)
        {
            maxScore += GetMaximumPossibleScoreInCategory(category);
        }
        return maxScore;
    }
    public ItemizedQuizScore GetMaximumPossibleScorePerCategory()
    {
        // Create the score to return
        ItemizedQuizScore maxScore = new ItemizedQuizScore();
        // Get a list of all categories
        IEnumerable<QuizCategory> categories = GetTestedCategories();

        // Set the max score for each category
        foreach (QuizCategory category in categories)
        {
            maxScore.Set(category, GetMaximumPossibleScoreInCategory(category));
        }
        return maxScore;
    }
    public IEnumerable<QuizCategory> GetTestedCategories() => questions.Select(q => q.Category).Distinct();
    public int GetMaximumPossibleScoreInCategory(QuizCategory category)
    {
        if (!CollectionExtensions.IsNullOrEmpty(questions))
        {
            // Get the questions with this type
            IEnumerable<QuizQuestion> questions = GetQuestionsWithCategory(category);
            // Initialize the max score
            int maxScore = 0;
            foreach (QuizQuestion q in questions)
            {
                maxScore += q.MaxPossibleScore;
            }

            return maxScore;
        }
        else return 0;
    }
    public IEnumerable<QuizQuestion> GetQuestionsWithCategory(QuizCategory category) => questions.Where(q => q.Category == category);
    #endregion
}
