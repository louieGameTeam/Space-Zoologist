using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using DialogueEditor;

public class GameOverController : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Data for the window displayed when all objectives finish, before final NPC dialogue")]
    private GenericWindow objectiveFinishedWindow;
    [SerializeField]
    [Tooltip("Data for the window displayed when the enclosure is finished")]
    private GenericWindow successWindow;
    [SerializeField]
    [Tooltip("Data for the window displayed when the level fails")]
    private GenericWindow failWindow;
    #endregion

    #region Private Fields
    // True when the main objectives are completed
    private bool mainObjectivesCompleted = false;
    #endregion

    #region Monobehaviour Messages
    private void Start()
    {
        // Subscribe to events on the event manager
        // NOTE: we're depending on "MainObjectivesCompleted" firing before the "GameOver" event,
        // if the order switches we'll think we lost!
        EventManager.Instance.SubscribeToEvent(EventType.MainObjectivesCompleted, OnMainObjectivesCompleted);
        EventManager.Instance.SubscribeToEvent(EventType.GameOver, OnGameOver);
        EventManager.Instance.SubscribeToEvent(EventType.PopulationExtinct, OnPopulationExtinct);
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
            OpenWindow(objectiveFinishedWindow, LevelPassedConversation);
        }
        else
        {
            // Instantiate a copy of the restart conversation and say it
            GameManager gameManager = GameManager.Instance;
            NPCConversation restartConversation = gameManager.LevelData.RestartConversation.InstantiateAndSay();
            gameManager.m_dialogueManager.StartInteractiveConversation();

            // When the conversation ends then show the fail window
            restartConversation.OnConversationEnded(() =>
            {
                OpenWindow(failWindow, () => LevelDataLoader.ReloadLevel(), () => SceneManager.LoadScene("LevelMenu"));
            });
        }
    }
    private void OnPopulationExtinct()
    {
        Population population = EventManager.Instance.EventData as Population;
        int animalsRemaining = GameManager.Instance.m_resourceManager.CheckRemainingResource(population.species);

        // If you cannot add any more animals to the population that just went extinct,
        // then you know that you just failed the level
        if (animalsRemaining <= 0) OnGameOver();
    }
    private void LevelPassedConversation()
    {
        GameManager gameManager = GameManager.Instance;
        LevelEndingData ending = gameManager.LevelData.Ending;

        ending.SayEndingConversation();
        gameManager.m_dialogueManager.StartInteractiveConversation();

        // If the ending is a quiz then subscribe to its conversation ended event
        if (ending.IsQuiz)
        {
            ending.ActiveQuizConversation.OnConversationEnded.AddListener(OnSuccessConversationEnded);
        }
        // If the ending is not a quiz then subscribe to the ending event for the non-quiz conversation
        else ending.ActiveConversation.OnConversationEnded(OnSuccessConversationEnded);
    }
    private void OnSuccessConversationEnded()
    {
        // Open the success window
        OpenWindow(successWindow, () => LevelDataLoader.LoadNextLevel(), () => SceneManager.LoadScene("LevelMenu"));
    }
    private void OpenWindow(GenericWindow window, UnityAction primaryAction, UnityAction secondaryAction = null)
    {
        // Get the first canvas you can find
        Canvas canvas = FindObjectOfType<Canvas>();
        canvas = canvas.rootCanvas;

        // Instantiate the window under the root canvas
        window = Instantiate(window, canvas.transform);
        // Open the window
        window.Open(primaryAction, secondaryAction);
    }
    #endregion
}
