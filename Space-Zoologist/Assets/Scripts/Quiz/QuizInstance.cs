using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizInstance
{
    #region Public Properties
    public QuizTemplate Template => template;
    public QuizRuntimeTemplate RuntimeTemplate => runtimeTemplate;
    public int QuestionsAnswered => ComputeQuestionsAnswered(runtimeTemplate, answers);
    public bool Completed => ComputeCompleted(runtimeTemplate, answers);
    public QuizGrade Grade => ComputeGrade(runtimeTemplate, answers);
    public ItemizedQuizScore ItemizedScore => ComputeItemizedScore(runtimeTemplate, answers);
    public int ScoreInImportantCategories => ComputeScoreInImportantCategories(runtimeTemplate, answers);
    public int ScoreInUnimportantCategories => ComputeScoreInUnimportantCategories(runtimeTemplate, answers);
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the template that this instance refers to")]
    private QuizTemplate template;
    [SerializeField]
    [Tooltip("Indices of the options that were chosen for each answer")]
    private int[] answers;
    #endregion

    #region Private Fields
    private QuizRuntimeTemplate runtimeTemplate;
    #endregion

    #region Constructors
    public QuizInstance(QuizTemplate template)
    {
        // Assign the template
        this.template = template;
        // Create a new runtime template
        runtimeTemplate = new QuizRuntimeTemplate(template);

        // Create an answer for each question
        answers = Enumerable.Repeat(-1, runtimeTemplate.Questions.Length).ToArray();
    }
    #endregion

    #region Public Methods
    public void AnswerQuestion(int questionIndex, int answer)
    {
        if (questionIndex >= 0 && questionIndex < runtimeTemplate.Questions.Length)
        {
            QuizQuestion question = runtimeTemplate.Questions[questionIndex];

            // If the answer is within bounds of the question options then set the value in the array
            if (answer >= 0 && answer < question.Options.Length)
            {
                answers[questionIndex] = answer;
            }
            // Question does not have that option so throw an exception
            else throw new System.IndexOutOfRangeException("QuizInstance: cannot answer question '" + question.Question +
                "' on quiz template '" + runtimeTemplate.Template.name + "' with answer '" + answer + "' because no such option exists");
        }
        else throw new System.IndexOutOfRangeException("QuizInstance: the quiz template '" + runtimeTemplate.Template.name +
            "' does not have a question at index '" + questionIndex + "'");
    }
    #endregion

    #region Public Static Methods
    // Made these methods public static so that the property drawer can get the info to display in the inspector window
    public static int ComputeQuestionsAnswered(QuizRuntimeTemplate runtimeTemplate, int[] answers) => answers
        // Include the index in the enumeration
        .WithIndex()
        // Count every answer greater than zero and less than the number of options for the question with the same index
        .Count(answer => answer.item >= 0 && answer.item < runtimeTemplate.Questions[answer.index].Options.Length);
    public static bool ComputeCompleted(QuizRuntimeTemplate runtimeTemplate, int[] answers) => ComputeQuestionsAnswered(runtimeTemplate, answers) >= runtimeTemplate.Questions.Length;
    public static QuizGrade ComputeGrade(QuizRuntimeTemplate runtimeTemplate, int[] answers)
    {
        // Determine if you pass or fail important categories
        bool passImportant = PassedImportantQuestions(runtimeTemplate, answers);

        // If you pass important, go on to check if you pass unimportant too
        if (passImportant)
        {
            bool passUnimportant = PassedUnimportantQuestions(runtimeTemplate, answers);

            // If you pass both, your score is excellent
            if (passUnimportant) return QuizGrade.Excellent;
            // If you pass only important categories your score is acceptable
            else return QuizGrade.Acceptable;
        }
        // If you fail important categories your grade is poor
        else return QuizGrade.Poor;
    }
    public static int ComputeScoreInImportantCategories(QuizRuntimeTemplate runtimeTemplate, int[] answers)
    {
        // Get the score itemized by category
        ItemizedQuizScore itemizedScore = ComputeItemizedScore(runtimeTemplate, answers);
        int score = 0;

        // Add up the scores in the important categories
        foreach (QuizCategory category in runtimeTemplate.Template.ImportantCategories)
        {
            score += itemizedScore.GetOrElse(category, 0);
        }
        return score;
    }
    public static bool PassedImportantQuestions(QuizRuntimeTemplate runtimeTemplate, int[] answers) => runtimeTemplate.Template.GradingRubric
        .PassedImportantQuestions(ComputeScoreInImportantCategories(runtimeTemplate, answers));
    public static int ComputeScoreInUnimportantCategories(QuizRuntimeTemplate runtimeTemplate, int[] answers)
    {
        return ComputeItemizedScore(runtimeTemplate, answers).TotalScore - ComputeScoreInImportantCategories(runtimeTemplate, answers);
    }
    public static bool PassedUnimportantQuestions(QuizRuntimeTemplate runtimeTemplate, int[] answers) => runtimeTemplate.Template.GradingRubric
        .PassedUnimportantQuestions(ComputeScoreInUnimportantCategories(runtimeTemplate, answers));
    public static ItemizedQuizScore ComputeItemizedScore(QuizRuntimeTemplate runtimeTemplate, int[] answers)
    {
        ItemizedQuizScore itemizedScore = new ItemizedQuizScore();

        for (int i = 0; i < runtimeTemplate.Questions.Length; i++)
        {
            // Get the current question and answer
            QuizQuestion question = runtimeTemplate.Questions[i];
            int answer = answers[i];

            // If the answer is within range of the options 
            // then add the option's weight to the total score
            if (answer >= 0 && answer < question.Options.Length)
            {
                QuizOption option = question.Options[answer];
                int currentScore = itemizedScore.GetOrElse(question.Category, 0);
                itemizedScore.Set(question.Category, currentScore + option.Weight);
            }
        }

        return itemizedScore;
    }
    #endregion
}
