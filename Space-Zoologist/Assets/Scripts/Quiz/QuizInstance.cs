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
    public int TotalScore
    {
        get
        {
            int score = 0;

            for(int i = 0; i < template.Questions.Length; i++)
            {
                // Get the current question and answer
                QuizQuestion question = template.Questions[i];
                int answer = answers[i];

                // If the answer is within range of the options 
                // then add the option's weight to the total score
                if(answer >= 0 && answer < question.Options.Length)
                {
                    QuizOption option = question.Options[answer];
                    score += option.Weight;
                }
            }

            return score;
        }
    }
    public QuizGradeType TotalGrade => template.GradingRubric.Grade(TotalScore, template.GetMaximumPossibleScore().TotalScore);
    public QuizScore Score
    {
        get
        {
            QuizScore score = new QuizScore();

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
                    score.Set(question.ScoreType, score.Get(question.ScoreType) + option.Weight);
                }
            }

            return score;
        }
    }
    public QuizGrade Grade
    {
        get
        {
            QuizGrade grade = new QuizGrade();
            QuizScore score = Score;
            QuizScoreType[] types = (QuizScoreType[])System.Enum.GetValues(typeof(QuizScoreType));

            foreach(QuizScoreType type in types)
            {
                int myScore = score.Get(type);
                int maxScore = template.GetMaximumPossibleScoreOfType(type);
                QuizGradeType gradeType = template.GradingRubric.Grade(myScore, maxScore);
                grade.Set(type, gradeType);
            }

            return grade;
        }
    }
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
