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
    public QuizQuestionOrPool[] QuestionData => questionData;
    public QuizGradingRubric GradingRubric => gradingRubric;
    // Quiz template is "static" if non of the question datas
    // is a pool of randomly selected questions
    public bool Static => questionData.All(data => data.Static);
    public bool Dynamic => !Static;
    public int QuestionCount => questionData.Sum(data => data.QuestionCount);
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of categories that are considered important for this quiz")]
    private QuizCategory[] importantCategories;
    [SerializeField]
    [Tooltip("List of questions to ask in the quiz")]
    [FormerlySerializedAs("questions")]
    private QuizQuestionOrPool[] questionData;
    [SerializeField]
    [Tooltip("Percentage to get correct to be considered a 'partial pass'")]
    private QuizGradingRubric gradingRubric;

    [Space]

    [SerializeField]
    [Tooltip("Example quiz instance. Use this to test the parameters of this template")]
    private QuizInstance exampleQuiz;
    #endregion
}
