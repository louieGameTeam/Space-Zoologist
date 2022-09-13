using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class QuizTemplate : ScriptableObject
{
    #region Public Properties
    public QuizCategory[] ImportantCategories => importantCategories;
    public QuizQuestion[] FixedQuestions => fixedQuestions;
    public QuizQuestionPool RandomQuestionPool => randomQuestionPool;
    public QuizGradingRubric GradingRubric => gradingRubric;
    // Quiz template is "static" if none of the question datas
    // is a pool of randomly selected questions
    public bool Static => randomQuestionPool.Static;
    public bool Dynamic => !Static;
    public int QuestionCount => fixedQuestions.Length + randomQuestionPool.QuestionsToPick;
    public QuizQuestion[] AllQuestions
    {
        get
        {
            QuizQuestion[] questions = new QuizQuestion[fixedQuestions.Length + randomQuestionPool.QuestionPool.Length];

            // Copy fixed questions into the array
            System.Array.Copy(fixedQuestions, questions, fixedQuestions.Length);
            // Copy all random questions into the array
            System.Array.Copy(randomQuestionPool.QuestionPool, 0, questions, fixedQuestions.Length, randomQuestionPool.QuestionPool.Length);

            return questions;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of categories that are considered important for this quiz")]
    private QuizCategory[] importantCategories = null;
    [SerializeField]
    [Tooltip("List of questions to ask in the quiz")]
    [FormerlySerializedAs("questions")]
    private QuizQuestion[] fixedQuestions = null;
    [SerializeField]
    [Tooltip("A pool of random questions that will be asked last in the quiz")]
    private QuizQuestionPool randomQuestionPool = null;
    [SerializeField]
    [Tooltip("Percentage to get correct to be considered a 'partial pass'")]
    private QuizGradingRubric gradingRubric = null;

    [Space]

    [SerializeField]
    [Tooltip("Example quiz instance. Use this to test the parameters of this template")]
    private QuizInstance exampleQuiz = null;
    #endregion

    #region Public Methods
    public QuizQuestion[] GenerateQuestions()
    {
        // Create the array to hold all generated questions
        QuizQuestion[] generatedQuestions = new QuizQuestion[fixedQuestions.Length + randomQuestionPool.QuestionsToPick];
        int currentIndex;

        // Add each question in the static list to the list to return
        for(currentIndex = 0; currentIndex < fixedQuestions.Length; currentIndex++)
        {
            generatedQuestions[currentIndex] = fixedQuestions[currentIndex];
        }

        // Randomly generate some questions
        QuizQuestion[] randomlyGeneratedQuestions = randomQuestionPool.PickQuestions();

        // Add each randomly generated question to the list
        if (randomlyGeneratedQuestions.Length > 0)
        {
            foreach (QuizQuestion question in randomlyGeneratedQuestions)
            {
                generatedQuestions[currentIndex] = question;
                currentIndex++;
            }
        }

        return generatedQuestions;
    }
    #endregion
}
