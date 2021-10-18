using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class QuizTemplate : ScriptableObject
{
    #region Public Typedefs
    // So that the examples show up in the editor
    [System.Serializable]
    public class QuizScoreBoundary
    {
        [WrappedProperty("scoreData")]
        public QuizScore[] scores;
    }
    #endregion

    #region Public Properties
    public QuizQuestion[] Questions => questions;
    public QuizGradingRubric GradingRubric => gradingRubric;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of questions to ask in the quiz")]
    private QuizQuestion[] questions;
    [SerializeField]
    [Tooltip("Percentage to get correct to be considered a 'partial pass'")]
    private QuizGradingRubric gradingRubric;

    [Space]

    [SerializeField]
    [Tooltip("Bound scores for each grade level. The score must be at least this much " +
        "to be in the grade level shown")]
    [EditArrayWrapperOnEnum("scores", typeof(QuizGradeType))]
    private QuizScoreBoundary scoreBoundaries;
    #endregion

    #region Object Messages
    private void OnValidate()
    {
        scoreBoundaries.scores = GetScoreLowerBounds();
    }
    #endregion

    #region Public Methods
    public QuizScore[] GetScoreLowerBounds()
    {
        if (!CollectionExtensions.IsNullOrEmpty(gradingRubric.Percentages))
        {
            // Get the array of possible grades
            QuizGradeType[] grades = (QuizGradeType[])System.Enum.GetValues(typeof(QuizGradeType));
            QuizScore[] boundaries = new QuizScore[grades.Length];
            QuizScore max = GetMaximumPossibleScore();

            // Loop through each grade level
            for (int i = 0; i < grades.Length; i++)
            {
                // Create the lower bound for this score level
                QuizScore gradeScore = new QuizScore();
                // Get a list of all score types
                QuizScoreType[] scoreTypes = (QuizScoreType[])System.Enum.GetValues(typeof(QuizScoreType));

                // Set the score for each score type
                foreach (QuizScoreType type in scoreTypes)
                {
                    int score = (int)(max.Get(type) * gradingRubric.GetLowerBound(grades[i]));
                    gradeScore.Set(type, score);
                }

                boundaries[i] = gradeScore;
            }

            return boundaries;
        }
        else return null;
    }
    public QuizScore GetMaximumPossibleScore()
    {
        // Create the score to return
        QuizScore maxScore = new QuizScore();
        int numScores = System.Enum.GetNames(typeof(QuizScoreType)).Length;

        // Set each score to the max score
        for (int i = 0; i < numScores; i++)
        {
            maxScore.Set((QuizScoreType)i, GetMaximumPossibleScoreOfType((QuizScoreType)i));
        }

        return maxScore;
    }
    public int GetMaximumPossibleScoreOfType(QuizScoreType type)
    {
        if (!CollectionExtensions.IsNullOrEmpty(questions))
        {
            // Get the questions with this type
            QuizQuestion[] questions = GetQuestionsWithType(type);
            // Initialize the max score
            int maxScore = 0;

            // Check if the array is not null or empty
            if (!CollectionExtensions.IsNullOrEmpty(questions))
            {
                foreach (QuizQuestion q in questions)
                {
                    maxScore += q.MaxPossibleScore;
                }
            }

            return maxScore;
        }
        else return 0;
    }
    public QuizQuestion[] GetQuestionsWithType(QuizScoreType type) => questions.Where(q => q.ScoreType == type).ToArray();
    #endregion
}
