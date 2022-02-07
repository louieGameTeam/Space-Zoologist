using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizTemplateUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Reference to the prefab to use to display each question")]
    private QuizQuestionUI questionUIPrefab;
    [SerializeField]
    [Tooltip("Parent to instantiate all children into")]
    private Transform questionUIParent;
    #endregion

    #region Private Fields
    private List<QuizQuestionUI> currentQuestionUIs = new List<QuizQuestionUI>(); 
    #endregion

    #region Public Methods
    public void DisplayQuizTemplate(QuizTemplate template)
    {
        // Destroy any existing question uis
        foreach(QuizQuestionUI questionUI in currentQuestionUIs)
        {
            Destroy(questionUI.gameObject);
        }
        currentQuestionUIs.Clear();

        // Get all the questions
        QuizQuestion[] allQuestions = template.AllQuestions;

        // Create a question ui for each question
        for(int i = 0; i < allQuestions.Length; i++)
        {
            QuizQuestionUI ui = Instantiate(questionUIPrefab, questionUIParent);
            ui.DisplayQuestion(allQuestions[i], i);
            currentQuestionUIs.Add(ui);
        }
    }
    #endregion
}
