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
    public QuizQuestion[] Questions => questions;
    public QuizQuestionPool RandomQuestionPool => randomQuestionPool;
    public QuizGradingRubric GradingRubric => gradingRubric;
    // Quiz template is "static" if non of the question datas
    // is a pool of randomly selected questions
    public bool Static => randomQuestionPool.Static;
    public bool Dynamic => !Static;
    public int QuestionCount => questions.Length + randomQuestionPool.QuestionsToPick;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of categories that are considered important for this quiz")]
    private QuizCategory[] importantCategories;
    [SerializeField]
    [Tooltip("List of questions to ask in the quiz")]
    private QuizQuestion[] questions;
    [SerializeField]
    [Tooltip("A pool of random questions that will be asked last in the quiz")]
    private QuizQuestionPool randomQuestionPool;
    [SerializeField]
    [Tooltip("Percentage to get correct to be considered a 'partial pass'")]
    private QuizGradingRubric gradingRubric;

    [Space]

    [SerializeField]
    [Tooltip("Example quiz instance. Use this to test the parameters of this template")]
    private QuizInstance exampleQuiz;
    #endregion

    #region Public Methods
    public QuizQuestion[] GenerateQuestions()
    {
        // Create the array to hold all generated questions
        QuizQuestion[] generatedQuestions = new QuizQuestion[questions.Length + randomQuestionPool.QuestionsToPick];
        int currentIndex;

        // Add each question in the static list to the list to return
        for(currentIndex = 0; currentIndex < questions.Length; currentIndex++)
        {
            generatedQuestions[currentIndex] = questions[currentIndex];
        }

        // Randomly generate some questions
        QuizQuestion[] randomlyGeneratedQuestions = randomQuestionPool.PickQuestions();
        Debug.Log($"Randomly generated questions: {randomlyGeneratedQuestions.Length}");

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
