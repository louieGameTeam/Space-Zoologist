using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizInstance
{
    #region Public Properties
    public QuizTemplate Template => template;
    public int QuestionsAnswered => ComputeQuestionsAnswered(template, answers);
    public bool Completed => ComputeCompleted(template, answers);
    public QuizGrade Grade => ComputeGrade(template, answers);
    public ItemizedQuizScore ItemizedScore => ComputeItemizedScore(template, answers);
    public int ScoreInImportantCategories => ComputeScoreInImportantCategories(template, answers);
    public int ScoreInUnimportantCategories => ComputeScoreInUnimportantCategories(template, answers);
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the template that this instance refers to")]
    private QuizTemplate template;
    [SerializeField]
    [Tooltip("Indices of the options that were chosen for each answer")]
    private int[] answers;
    #endregion

    #region Constructors
    public QuizInstance(QuizTemplate template)
    {
        this.template = template;

        // Create an answer for each question
        answers = Enumerable.Repeat(-1, template.Questions.Length).ToArray();
    }
    #endregion

    #region Public Methods
    public void AnswerQuestion(int questionIndex, int answer)
    {
        if (questionIndex >= 0 && questionIndex < template.Questions.Length)
        {
            QuizQuestion question = template.Questions[questionIndex];

            // If the answer is within bounds of the question options then set the value in the array
            if (answer >= 0 && answer < question.Options.Length)
            {
                answers[questionIndex] = answer;
            }
            // Question does not have that option so throw an exception
            else throw new System.IndexOutOfRangeException("QuizInstance: cannot answer question '" + question.Question +
                "' on quiz template '" + template.name + "' with answer '" + answer + "' because no such option exists");
        }
        else throw new System.IndexOutOfRangeException("QuizInstance: the quiz template '" + template.name +
            "' does not have a question at index '" + questionIndex + "'");
    }
    #endregion

    #region Public Static Methods
    public static int ComputeQuestionsAnswered(QuizTemplate template, int[] answers) => answers.Count(i => i >= 0 && i < template.Questions.Length);
    public static bool ComputeCompleted(QuizTemplate template, int[] answers) => ComputeQuestionsAnswered(template, answers) >= template.Questions.Length;
    // Made these methods public static so that the property drawer can get the info to display in the inspector window
    public static QuizGrade ComputeGrade(QuizTemplate template, int[] answers)
    {
        // Determine if you pass or fail important categories
        bool passImportant = PassedImportantQuestions(template, answers);

        // If you pass important, go on to check if you pass unimportant too
        if (passImportant)
        {
            bool passUnimportant = PassedUnimportantQuestions(template, answers);

            // If you pass both, your score is excellent
            if (passUnimportant) return QuizGrade.Excellent;
            // If you pass only important categories your score is acceptable
            else return QuizGrade.Acceptable;
        }
        // If you fail important categories your grade is poor
        else return QuizGrade.Poor;
    }
    public static int ComputeScoreInImportantCategories(QuizTemplate template, int[] answers)
    {
        // Get the score itemized by category
        ItemizedQuizScore itemizedScore = ComputeItemizedScore(template, answers);
        int score = 0;

        // Add up the scores in the important categories
        foreach (QuizCategory category in template.ImportantCategories)
        {
            score += itemizedScore.GetOrElse(category, 0);
        }
        return score;
    }
    public static bool PassedImportantQuestions(QuizTemplate template, int[] answers) => template.GradingRubric
        .PassedImportantQuestions(ComputeScoreInImportantCategories(template, answers));
    public static int ComputeScoreInUnimportantCategories(QuizTemplate template, int[] answers)
    {
        return ComputeItemizedScore(template, answers).TotalScore - ComputeScoreInImportantCategories(template, answers);
    }
    public static bool PassedUnimportantQuestions(QuizTemplate template, int[] answers) => template.GradingRubric
        .PassedUnimportantQuestions(ComputeScoreInUnimportantCategories(template, answers));
    public static ItemizedQuizScore ComputeItemizedScore(QuizTemplate template, int[] answers)
    {
        ItemizedQuizScore itemizedScore = new ItemizedQuizScore();

        for (int i = 0; i < template.Questions.Length; i++)
        {
            // Get the current question and answer
            QuizQuestion question = template.Questions[i];
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
