using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizQuestionUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Text component used to display the title of the question")]
    private TextMeshProUGUI titleText;
    [SerializeField]
    [Tooltip("Text component used to display the question")]
    private TextMeshProUGUI questionText;
    #endregion

    #region Public Methods
    public void DisplayQuestion(QuizQuestion question, int questionIndex)
    {
        titleText.text = $"Research Question {questionIndex + 1}:";
        questionText.text = question.Question;   
    }
    #endregion
}
