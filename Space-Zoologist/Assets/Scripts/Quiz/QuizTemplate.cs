using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class QuizTemplate : ScriptableObject
{
    #region Public Properties
    public QuizCategory[] ImportantCategories => importantCategories;
    public QuizQuestion[] Questions => questions;
    public QuizGradingRubric GradingRubric => gradingRubric;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of categories that are considered important for this quiz")]
    private QuizCategory[] importantCategories;
    [SerializeField]
    [Tooltip("List of questions to ask in the quiz")]
    private QuizQuestion[] questions;
    [SerializeField]
    [Tooltip("Percentage to get correct to be considered a 'partial pass'")]
    private QuizGradingRubric gradingRubric;
    #endregion

    #region Public Methods
    public int GetMaximumPossibleScoreInUnimportantCategories() => GetMaximumPossibleScorePerCategory().TotalScore - GetMaximumPossibleScoreInImportantCategories();
    public int GetMaximumPossibleScoreInImportantCategories()
    {
        int maxScore = 0;
        // Add the max score for each important category
        foreach (QuizCategory category in importantCategories)
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
        foreach(QuizCategory category in categories)
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

            // Check if the array is not null or empty
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
