using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizInstanceUI : MonoBehaviour
{
    #region Public Properties
    public Transform AnswerUIParent => answerUIParent;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Prefab to instantiate for each quiz answer")]
    private QuizAnswerUI answerUIPrefab;
    [SerializeField]
    [Tooltip("Transform parent for each answer ui")]
    private Transform answerUIParent;
    #endregion

    #region Private Fields
    private List<QuizAnswerUI> currentAnswerUIs = new List<QuizAnswerUI>();
    #endregion

    #region Public Methods
    public void DisplayQuizInstance(QuizInstance quiz)
    {
        // Remove all existing answer uis
        foreach(QuizAnswerUI ui in currentAnswerUIs)
        {
            Destroy(ui.gameObject);
        }
        currentAnswerUIs.Clear();

        // Create an answer ui for each question in the runtime template
        for(int i = 0; i < quiz.RuntimeTemplate.Questions.Length; i++)
        {
            QuizAnswerUI ui = Instantiate(answerUIPrefab, answerUIParent);
            ui.DisplayAnswer(quiz, i);
            currentAnswerUIs.Add(ui);
        }
    }
    #endregion
}
