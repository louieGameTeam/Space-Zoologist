using System.Linq;
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
    private GenericWindow objectiveFinishedWindow = null;
    [SerializeField]
    [Tooltip("Data for the window displayed when the enclosure is finished")]
    private GenericSpriteAnimWindow successWindow = null;
    [SerializeField]
    [Tooltip("Data for the window displayed when the level fails")]
    private GenericSpriteAnimWindow failWindow = null;
    
    #endregion

    #region Private Fields
    // True when the main objectives are completed
    private bool mainObjectivesCompleted = false;
    
    // Param used to select the correct star count animation when opening the level success window
    private readonly string quizGradeAnimParam = "QuizGrade";
    
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
                OpenSpriteAnimWindow(failWindow, () => LevelDataLoader.ReloadLevel(), () => SceneNavigator.LoadScene("LevelMenu"));
            });
        }
    }
    private void OnPopulationExtinct(object eventData)
    {
        Population population = eventData as Population;
        // Count the number of animals that can still be placed in the enclosure
        int animalsRemainingToPlace = GameManager.Instance.m_resourceManager
            .CheckRemainingResource(population.species);
        // Count the number of animals remaining in the enclosure 
        int animalsRemainingInEnclosure = GameManager.Instance.m_populationManager.Populations
            .Where(pop => pop.species == population.species)
            .Sum(pop => pop.Count);

        // If you cannot add any more animals to the population that just went extinct,
        // then you know that you just failed the level
        if (animalsRemainingToPlace <= 0 && animalsRemainingInEnclosure <= 0) OnGameOver();
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
        // This results in an excellent score by default (3 stars)
        else ending.ActiveConversation.OnConversationEnded(() => OnSuccessConversationEnded(QuizGrade.Excellent));
    }

    private void OnSuccessConversationEnded(QuizGrade grade)
    {
        // Update the save data with the id of the level we are qualified to go to
        LevelEndingData ending = GameManager.Instance.LevelData.Ending;
        SaveData.QualifyForLevel(ending.GetNextLevelID());

        // Compute the rating for this level
        int rating = LevelRatingSystem.RateLevel(grade);
        SaveData.SetLevelRating(LevelID.Current(), rating);

        // Save changes to the save data
        SaveData.Save();

        // Let the game manager handle level exiting
        GameManager.Instance.HandleExitLevel();

        // Open the success window
        OpenSpriteAnimWindow(
            successWindow, 
            () => SceneNavigator.LoadScene("LevelMenu"), 
            () => LevelDataLoader.ReloadLevel(),
            grade);
    }
    private void OpenWindow(GenericWindow window, UnityAction primaryAction, UnityAction secondaryAction = null)
    {
        // Instantiate the window under the main canvas
        Transform canvas = GameObject.FindWithTag("MainCanvas").transform;
        window = Instantiate(window, canvas);

        // Setup the primary and secondary action of the window
        window.AddPrimaryAction(primaryAction);
        if (secondaryAction != null)
        {
            window.AddSecondaryAction(secondaryAction);
        }
    
        // Open the window
        window.Open();
    }
    
    private void OpenSpriteAnimWindow(GenericSpriteAnimWindow window, UnityAction primaryAction, UnityAction secondaryAction, QuizGrade grade = 0)
    {
        // Instantiate the window under the main canvas
        Transform canvas = GameObject.FindWithTag("MainCanvas").transform;
        window = Instantiate(window, canvas);

        // Setup the primary and secondary action of the window
        window.AddPrimaryAction(primaryAction);
        if (secondaryAction != null)
        {
            window.AddSecondaryAction(secondaryAction);
        }
        
        void OpenBehavior()
        {
            window.Animator.SetInteger(quizGradeAnimParam, (int)grade);
        }

        // Open the window
        window.Open(OpenBehavior);

        // Firework VFX if 3 stars
        if (grade == QuizGrade.Excellent)
        {
            window.GetComponent<FireworkGen>()?.SetIsSpawning(true);
        }
    }
    #endregion
}
