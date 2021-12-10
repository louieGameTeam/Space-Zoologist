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
                Debug.Log("Here you go i guess");
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
    // True if the fail window should be displayed the next time a conversation ends
    private bool failWindowReady = false;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Subscribe to events on the event manager
        // NOTE: we're depending on "MainObjectivesCompleted" firing before the "GameOver" event,
        // if the order switches we'll think we lost!
        EventManager.Instance.SubscribeToEvent(EventType.MainObjectivesCompleted, OnMainObjectivesCompleted);
        EventManager.Instance.SubscribeToEvent(EventType.GameOver, OnGameOver);

        // Say the level passed conversation when level is passed
        objectiveFinishedWindow.PrimaryButtonData.ButtonAction.AddListener(LevelPassedConversation);

        // NOTE: can't really set up the success window right now 
        // because the passed conversation will automatically load the next level
        // without letting us pull up the success window first
        LevelDataLoader levelLoader = FindObjectOfType<LevelDataLoader>();
        // successWindow.PrimaryButtonData.ButtonAction.AddListener();

        // Reload level or load the level select in the fail window
        failWindow.PrimaryButtonData.ButtonAction.AddListener(() => levelLoader.ReloadLevel());
        failWindow.SecondaryButtonData.ButtonAction.AddListener(() => SceneManager.LoadScene("LevelMenu"));

        // Subscribe to conversation ended event
        ConversationManager.OnConversationEnded += OnConversationEnded;
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
            Debug.Log("Game over!");
            GameManager gameManager = GameManager.Instance;
            gameManager.m_dialogueManager.SetNewDialogue(gameManager.LevelData.RestartConversation);
            gameManager.m_dialogueManager.StartInteractiveConversation();

            // Ready the fail window
            failWindowReady = true;
        }
    }
    private void LevelPassedConversation()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.LevelData.PassedConversation.Speak(gameManager.m_dialogueManager);
        gameManager.m_dialogueManager.StartInteractiveConversation();
    }
    private void OnConversationEnded()
    {
        // If fail window was readied then setup the window with the fail window data
        if (failWindowReady)
        {
            Debug.Log("Setup fail window");
            Window.Setup(failWindow);
        }
        failWindowReady = false;
    }
    #endregion
}
