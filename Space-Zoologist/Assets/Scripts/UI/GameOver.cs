using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject GameOverHUD = default;
    [SerializeField] GameObject IngameUI = default;
    [SerializeField] Button RestartButton = default;
    [SerializeField] Button NextLevelButton = default;
    [SerializeField] TextMeshProUGUI title = default;
    [SerializeField] TextMeshProUGUI text = default;
    [SerializeField] PauseManager PauseManager = default;
    [SerializeField] SceneNavigator SceneNavigator = default;
    [SerializeField] ObjectiveManager objectiveManager = default;
    [SerializeField] TimeSystem TimeSystem = default;

    [SerializeField] DialogueManager dialogueManager = default;
    [SerializeField] DialogueEditor.NPCConversation passedConversation = default;
    [SerializeField] DialogueEditor.NPCConversation restertEnclosureConversation = default;

    private void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.GameOver, HandleNPCEndConversation);
        this.RestartButton.onClick.AddListener(() => {this.SceneNavigator.LoadLevel(this.SceneNavigator.RecentlyLoadedLevel);});
        this.NextLevelButton?.onClick.AddListener(() => { this.SceneNavigator.LoadLevelMenu(); } );
    }

    public void HandleNPCEndConversation()
    {
        if (!this.TimeSystem.LessThanMaxDay)
        {
            dialogueManager.SetNewDialogue(restertEnclosureConversation);
        }
        else
        {
            dialogueManager.SetNewDialogue(passedConversation);
        }
        dialogueManager.StartInteractiveConversation();
        this.IngameUI.SetActive(false);
    }

    public void HandleGameOver()
    {
        this.PauseManager.Pause();
        this.GameOverHUD.SetActive(true);
        this.IngameUI.SetActive(false);


        // Game completed
        if (objectiveManager.IsMainObjectivesCompleted)
        {
            if(NextLevelButton != null)
                NextLevelButton.interactable = true;
            // title.text = "Objectives Complete";
            //text.text = "Completed Secondary Objectives: " + objectiveManager.NumSecondaryObjectivesCompleted;
        }
        else {
            // Game lost
            if(NextLevelButton != null)
                NextLevelButton.interactable = false;
            title.text = "Mission Failed";
            text.text = "";
        }
    }
}
