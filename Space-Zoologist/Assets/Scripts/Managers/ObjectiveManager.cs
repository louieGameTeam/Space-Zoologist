using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages all the objectives and invoke event when game is over
/// </summary>
public class ObjectiveManager : MonoBehaviour
{
    private LevelDataReference levelDataReference = default;
    private LevelObjectiveData LevelObjectiveData = default;

    public bool IsGameOver => this.isGameOver;

    private bool isOpen = true;
    private bool isGameOver = false;

    // To access the player balance
    [SerializeField] private PlayerBalance playerBalance = default;

    // Objective panel
    [SerializeField] private GameObject objectivePanel = default;
    [SerializeField] private Text objectivePanelText = default;

    // Currently all main objectives are survival objectives
    private List<Objective> mainObjectives = new List<Objective>();
    private List<Objective> secondaryObjectives = new List<Objective>();

    public bool IsMainObjectivesCompleted { get; private set; }
    public int NumSecondaryObjectivesCompleted { get; private set; }

    public void ToggleObjectivePanel()
    {
        this.isOpen = !this.isOpen;
        this.objectivePanel.SetActive(this.isOpen);
        UpdateObjectives();
        this.UpdateObjectivePanel();
    }

    public void TurnObjectivePanelOff()
    {
        this.isOpen = false;
        this.objectivePanel.SetActive(this.isOpen);
        UpdateObjectives();
        this.UpdateObjectivePanel();
    }

    private void UpdateObjectives()
    {
        // Level is completed when all mian objectives are done, failed when one has failed
        foreach (Objective objective in this.mainObjectives)
        {
            if (objective.UpdateStatus() == ObjectiveStatus.InProgress)
            {
                IsMainObjectivesCompleted = false;
            }

            if (objective.UpdateStatus() == ObjectiveStatus.Failed)
            {
                // GameOver.cs listens for the event and handles gameover
                EventManager.Instance.InvokeEvent(EventType.GameOver, null);
            }
        }

        // Secondary objective status can be viewed on screen
        foreach (Objective objective in this.secondaryObjectives)
        {
            if (objective.UpdateStatus() == ObjectiveStatus.Completed)
            {
                NumSecondaryObjectivesCompleted++;
            }
        }
    }

    public void UpdateObjectivePanel()
    {
        string displayText = "\n";

        foreach (Objective objective in this.mainObjectives)
        {
            displayText += objective.GetObjectiveText();
        }
        if (this.secondaryObjectives.Count == 0)
        {
            this.objectivePanelText.text = displayText;
            return;
        }
        displayText += "Secondary Objectives:\n";
        foreach (Objective objective in this.secondaryObjectives)
        {
            displayText += objective.GetObjectiveText();
        }

        this.objectivePanelText.text = displayText;
    }

    /// <summary>
    /// Create objective objects and subscribe to events
    /// </summary>
    private void Start()
    {
        levelDataReference = FindObjectOfType<LevelDataReference>();
        this.LevelObjectiveData = this.levelDataReference.LevelData.LevelObjectiveData;

        // Create the survival objectives
        foreach (SurvivalObjectiveData objectiveData in this.LevelObjectiveData.survivalObjectiveDatas)
        {
            this.mainObjectives.Add(new SurvivalObjective(
                objectiveData.targetSpecies,
                objectiveData.targetPopulationCount,
                objectiveData.targetPopulationSize,
                objectiveData.timeRequirement
            ));
        }

        // Create the resource objective
        foreach (ResourceObjectiveData objectiveData in this.LevelObjectiveData.resourceObjectiveDatas)
        {
            this.secondaryObjectives.Add(new ResourceObjective(this.playerBalance, objectiveData.amountToKeep));
        }

        // Add the population to related objective if not seen before
        EventManager.Instance.SubscribeToEvent(EventType.NewPopulation, () =>
        {
            Population population = (Population)EventManager.Instance.EventData;
            this.RegisterWithSurvivalObjectives(population);
        });
        this.UpdateObjectivePanel();
    }

    private void RegisterWithSurvivalObjectives(Population population)
    {
        // Debug.Log(population.gameObject.name + " attempting to update survival objective");
        foreach (Objective objective in this.mainObjectives)
        {
            if (objective.GetType() == typeof(SurvivalObjective))
            {
                SurvivalObjective survivalObjective = (SurvivalObjective)objective;
                if (survivalObjective.AnimalSpecies == population.species && !survivalObjective.Populations.Contains(population))
                {
                    // Debug.Log(population.name + " was added to survival objective");
                    survivalObjective.Populations.Add(population);
                }
            }
        }
    }

    /// <summary>
    /// Check the status of the objectives
    /// </summary>
    private void Update()
    {
        IsMainObjectivesCompleted = true;
        NumSecondaryObjectivesCompleted = 0;
        UpdateObjectives();

        if (this.isOpen)
        {
            this.UpdateObjectivePanel();
        }

        // All objectives had reach end state
        if (IsMainObjectivesCompleted && !this.isGameOver)
        {
            this.isGameOver = true;

            // TODO figure out what should happen when the main objectives are complete
            EventManager.Instance.InvokeEvent(EventType.MainObjectivesCompleted, null);

            // GameOver.cs listens for the event and handles gameover
            EventManager.Instance.InvokeEvent(EventType.GameOver, null);

            Debug.Log($"Level Completed!");
        }
    }
}
