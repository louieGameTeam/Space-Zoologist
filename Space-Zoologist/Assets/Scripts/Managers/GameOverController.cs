using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using DialogueEditor;

public class GameOverController : MonoBehaviour
{
    #region Private Properties
    private GenericWindow Window
    {
        get
        {
            if(!window)
            {
                // Get the root canvas
                Canvas parent = FindObjectOfType<Canvas>();

                // Instantiate window under root canvas
                window = GenericWindow.InstantiateFromResource(parent.transform);
                window.transform.SetAsLastSibling();

                // Disable the window when any button is pressed
                window.AnyButtonPressedEvent.AddListener(() => window.gameObject.SetActive(false));
            }
            return window;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Data for the window displayed when all objectives finish, before final NPC dialogue")]
    private GenericWindowData objectiveFinishedWindow;
    [SerializeField]
    [Tooltip("Data for the window displayed when the enclosure is finished")]
    private GenericWindowData successWindow;
    [SerializeField]
    [Tooltip("Data for the window displayed when the level fails")]
    private GenericWindowData failWindow;
    #endregion

    #region Private Fields
    private GenericWindow window;
    // True when the main objectives are completed
    private bool mainObjectivesCompleted = false;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        //GameManager.Instance.LevelData.PassedConversation.

        // Subscribe to events on the event manager
        // NOTE: we're depending on "MainObjectivesCompleted" firing before the "GameOver" event,
        // if the order switches we'll think we lost!
        EventManager.Instance.SubscribeToEvent(EventType.MainObjectivesCompleted, OnMainObjectivesCompleted);
        EventManager.Instance.SubscribeToEvent(EventType.GameOver, OnGameOver);

        // Say the level passed conversation when level is passed
        objectiveFinishedWindow.PrimaryButtonData.ButtonAction.AddListener(LevelPassedConversation);

        // Setup the success window to load the next level or go back to level select
        LevelDataLoader levelLoader = FindObjectOfType<LevelDataLoader>();
        successWindow.PrimaryButtonData.ButtonAction.AddListener(() => levelLoader.LoadNextLevel());
        successWindow.SecondaryButtonData.ButtonAction.AddListener(() => SceneManager.LoadScene("LevelMenu"));

        // Reload level or load the level select in the fail window
        failWindow.PrimaryButtonData.ButtonAction.AddListener(() => levelLoader.ReloadLevel());
        failWindow.SecondaryButtonData.ButtonAction.AddListener(() => SceneManager.LoadScene("LevelMenu"));

        // Subscribe to event raised when any conversation is started
        ConversationManager.OnConversationStarted += OnAnyConversationStarted;
    }
    #endregion

    #region Private Methods
    private void OnMainObjectivesCompleted()
    {
        mainObjectivesCompleted = true;
    }
    private void OnGameOver()
    {
        if (mainObjectivesCompleted)
        {
            Window.Setup(objectiveFinishedWindow);
        }
        else
        {
            // Instantiate a copy of the restart conversation and say it
            GameManager gameManager = GameManager.Instance;
            NPCConversation restartConversation = gameManager.LevelData.RestartConversation.InstantiateAndSay();
            gameManager.m_dialogueManager.StartInteractiveConversation();

            // When the conversation ends then show the fail window
            restartConversation.OnConversationEnded(() => Window.Setup(failWindow));
        }
    }
    private void LevelPassedConversation()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.LevelData.Ending.SayEndingConversation();
        gameManager.m_dialogueManager.StartInteractiveConversation();
    }
    private void OnAnyConversationStarted()
    {
        QuizConversation quiz = GameManager.Instance.LevelData.Ending.ActiveQuizConversation;

        if (quiz)
        {
            NPCConversation quizResponse = quiz.CurrentResponse;

            // If a quiz response is active, make the success window setup
            // once the response is over
            if (quizResponse)
            {
                Debug.Log("Got a quiz response");
                quizResponse.OnConversationEnded(() =>
                {
                    Window.Setup(successWindow);
                    Window.gameObject.SetActive(true);
                });
            }
        }
    }
    #endregion
}
