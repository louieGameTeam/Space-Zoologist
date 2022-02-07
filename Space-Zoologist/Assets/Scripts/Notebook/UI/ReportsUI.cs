using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ReportsUI : NotebookUIChild
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Object used to display current research questions")]
    [FormerlySerializedAs("quizUI")]
    private QuizTemplateUI quizTemplateUI;
    [SerializeField]
    [Tooltip("Object used to select a level id")]
    private LevelIDPicker levelPicker;
    [SerializeField]
    [Tooltip("Object used to display the results of a quiz that the player took")]
    private QuizInstanceUI quizInstanceUI;
    [SerializeField]
    [Tooltip("Text displayed if there is no quiz to display for the current level id")]
    private GameObject noQuizText;
    #endregion

    #region Public Methods
    public override void Setup()
    {
        // Use the base setup method
        base.Setup();

        GameManager gameManager = GameManager.Instance;

        // If game manager exists then get the quiz of the current level
        if (gameManager)
        {
            QuizTemplate template = gameManager.LevelData.Quiz;

            // If the template exists then display it
            if (template)
            {
                quizTemplateUI.DisplayQuizTemplate(template);
            }
        }

        // Listen for level id picker change
        levelPicker.Setup();
        levelPicker.OnLevelIDPicked.AddListener(OnLevelPickerChanged);
        OnLevelPickerChanged(levelPicker.CurrentLevelID);
    }
    #endregion

    #region Event Listeners
    private void OnLevelPickerChanged(LevelID level)
    {
        QuizInstance quiz = UIParent.Data.Reports.GetQuiz(level);

        // If there is a quiz then display it
        if (quiz != null)
        {
            quizInstanceUI.DisplayQuizInstance(quiz);
        }

        // Enable/disable objects based off of whether there is a quiz for this level yet
        quizInstanceUI.AnswerUIParent.gameObject.SetActive(quiz != null);
        noQuizText.gameObject.SetActive(quiz == null);
    }
    #endregion
}
