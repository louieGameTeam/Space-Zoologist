using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuizInstance
{
    #region Public Properties
    public QuizTemplate Template => template;
    public int QuestionsAnswered => answers.Count(i => i >= 0);
    public bool Completed => QuestionsAnswered >= template.Questions.Length;
    public QuizGrade Grade
    {
        get
        {
            // Determine if you pass or fail important categories
            bool passImportant = template.GradingRubric.GradeImportantScore(
                ScoreInImportantCategories, 
                template.GetMaximumPossibleScoreInImportantCategories());

            // If you pass important, go on to check if you pass unimportant too
            if (passImportant)
            {
                bool passUnimportant = template.GradingRubric.GradeUnimportantScore(
                    ScoreInUnimportantCategories, 
                    template.GetMaximumPossibleScoreInUnimportantCategories());

                // If you pass both, your score is excellent
                if (passUnimportant) return QuizGrade.Excellent;
                // If you pass only important categories your score is acceptable
                else return QuizGrade.Acceptable;
            }
            // If you fail important categories your grade is poor
            else return QuizGrade.Poor;
        }
    }
    public ItemizedQuizScore ItemizedScore
    {
        get
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
                    itemizedScore.Set(question.Category, itemizedScore.Get(question.Category) + option.Weight);
                }
            }

            return itemizedScore;
        }
    }
    public int ScoreInImportantCategories
    {
        get
        {
            // Get the score itemized by category
            ItemizedQuizScore itemizedScore = ItemizedScore;
            int score = 0;

            // Add up the scores in the important categories
            foreach(QuizCategory category in template.ImportantCategories)
            {
                score += itemizedScore.Get(category);
            }
            return score;
        }
    }
    public int ScoreInUnimportantCategories => ItemizedScore.TotalScore - ScoreInImportantCategories;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the template that this instance refers to")]
    private QuizTemplate template;
    #endregion

    #region Private Fields
    // Indices of the options that were chosen for each answer
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
}
