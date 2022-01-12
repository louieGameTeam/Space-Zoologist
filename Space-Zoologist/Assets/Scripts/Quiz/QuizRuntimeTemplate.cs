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

    #region Private Fields
    private QuizQuestion[] questions;
    private QuizTemplate template;
    #endregion

    #region Constructors
    public QuizRuntimeTemplate(QuizTemplate template)
    {
        this.template = template;

        // Initialize the questions
        questions = new QuizQuestion[template.QuestionCount];
        int currentIndex = 0;

        // Go through each question set in the template
        foreach(QuizQuestionOrPool data in template.QuestionData)
        {
            // Go through each question in this question set
            foreach(QuizQuestion question in data.GetQuestions())
            {
                questions[currentIndex] = question;
                currentIndex++;
            }
        }
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
