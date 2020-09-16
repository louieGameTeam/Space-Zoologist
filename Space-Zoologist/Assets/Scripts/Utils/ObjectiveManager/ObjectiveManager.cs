using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public enum ObjectiveStatus { Completed, InProgress, Failed }

    public abstract class Objective
    {
        public abstract ObjectiveStatus Status { get; }
        public abstract ObjectiveStatus UpdateStatus();
    }

    public class SurvivalObjective : Objective
    {
        public List<Population> Populations = default;
        public AnimalSpecies AnimalSpecies { get; private set; }
        public byte TargetPopulationCount { get; private set; }
        public byte TargetPopulationSize { get; private set; }
        public float TargetTime { get; private set; }

        public float timer { get; private set; }
        private ObjectiveStatus status;

        public override ObjectiveStatus Status => this.status;

        public SurvivalObjective(AnimalSpecies animalSpecies, byte targetPopulationCount, byte targetPopulationSize, float targetTime)
        {
            this.Populations = new List<Population>();
            this.AnimalSpecies = animalSpecies;
            this.TargetPopulationCount = targetPopulationCount;
            this.TargetPopulationSize = targetPopulationSize;
            this.TargetTime = targetTime;
            this.status = ObjectiveStatus.InProgress;
        }

        public override ObjectiveStatus UpdateStatus()
        {
            byte satisfiedPopulationCount = 0;

            foreach (Population population in this.Populations)
            {
                // Found a population that has enough pop count
                if (population.Count >= this.TargetPopulationSize)
                {
                    satisfiedPopulationCount++;
                }

                // Have met the population number requirement
                if (satisfiedPopulationCount >= this.TargetPopulationCount)
                {

                    if (this.timer >= this.TargetTime)
                    {
                        return ObjectiveStatus.Completed;
                    }
                    else
                    {
                        this.timer += Time.deltaTime;
                    }

                    break;
                }
                // reset timer if requirement not met
                else
                {
                    this.timer = 0f;
                }
            }
            return ObjectiveStatus.InProgress;
        }
    }

    public class ResourceObjective : Objective
    {
        private PlayerBalance playerBalance;
        public int amountToKeep { get; private set; }

        public override ObjectiveStatus Status => this.status;

        private ObjectiveStatus status;

        public ResourceObjective(PlayerBalance playerBalance, int amountToKeep)
        {
            this.playerBalance = playerBalance;
            this.amountToKeep = amountToKeep;
            this.status = ObjectiveStatus.InProgress;
        }

        public override ObjectiveStatus UpdateStatus()
        {
            if (this.playerBalance.Balance >= this.amountToKeep)
            {
                this.status = ObjectiveStatus.InProgress;
            }
            else
            {
                this.status = ObjectiveStatus.Failed;
            }

            return this.status;
        }
    }

    [Expandable] public LevelDataReference LevelDataReference = default;
    private LevelObjectiveData LevelObjectiveData = default;

    private bool isOpen = false;

    // To access the populations 
    [SerializeField] private PopulationManager populationManager = default;
    // To access the player balance
    [SerializeField] private PlayerBalance playerBalance = default;
    [SerializeField] private PauseManager PauseManager = default;
    // Objective panel
    [SerializeField] private GameObject objectivePanel = default;
    [SerializeField] private Text objectivePanelText = default;

    private List<Objective> objectives = new List<Objective>();

    public void ToggleObjectivePanel()
    {
        this.isOpen = !this.isOpen;
        this.objectivePanel.SetActive(this.isOpen);
        this.UpdateObjectivePanel();
    }

    public void UpdateObjectivePanel()
    {
        string displayText = "";

        foreach (Objective objective in this.objectives)
        {
            if (objective.GetType() == typeof(SurvivalObjective))
            {
                SurvivalObjective survivalObjective = (SurvivalObjective)objective;
                string population = "population";
                string min = "minute";
                if (survivalObjective.TargetPopulationCount > 1)
                {
                    population += "s";
                }
                if (!(survivalObjective.TargetTime <= 120f))
                {
                    min += "s";
                }
                displayText += $"Maintain at least {survivalObjective.TargetPopulationCount} ";
                displayText += $"{survivalObjective.AnimalSpecies.SpeciesName} {population} with a count of {survivalObjective.TargetPopulationSize}";
                displayText += $" for {survivalObjective.TargetTime / 60f} {min} ";
                displayText += $"[{survivalObjective.Status.ToString()}] [{Math.Round(survivalObjective.timer, 0)}/{survivalObjective.TargetTime}]\n";
            }
            else if (objective.GetType() == typeof(ResourceObjective))
            {
                ResourceObjective resourceObjective = (ResourceObjective)objective;

                displayText += $"Have at least ${resourceObjective.amountToKeep} left [{resourceObjective.Status.ToString()}]\n";
            }
            else
            {
                Debug.Assert(true, $"{objective.GetType()} is not accounted for");
            }
        }

        this.objectivePanelText.text = displayText;
    }

    /// <summary>
    /// Create objective objects and subscribe to events
    /// </summary>
    public void Start()
    {
        this.LevelObjectiveData = this.LevelDataReference.LevelData.LevelObjectiveData;
        // Create the survival objectives
        foreach (SurvivalObjectiveData objectiveData in this.LevelObjectiveData.survivalObjectiveDatas)
        {
            this.objectives.Add(new SurvivalObjective(
                objectiveData.targetSpecies,
                objectiveData.targetPopulationCount,
                objectiveData.targetPopulationSize,
                objectiveData.timeRequirement
            ));
        }
        // Create the resource objective
        foreach (ResourceObjectiveData objectiveData in this.LevelObjectiveData.resourceObjectiveDatas)
        {
            this.objectives.Add(new ResourceObjective(this.playerBalance, objectiveData.amountToKeep));
        }

        // Add the population to related objective if not seen before
        EventManager.Instance.SubscribeToEvent(EventType.NewPopulation, () =>
        {
            Population population = (Population)EventManager.Instance.EventData;
            this.RegisterWithSurvivalObjectives(population);
        });
    }

    private void RegisterWithSurvivalObjectives(Population population)
    {
        //Debug.Log(population.gameObject.name + " attempting to update survivial objective");
        foreach (Objective objective in this.objectives)
        {
            if (objective.GetType() == typeof(SurvivalObjective))
            {
                SurvivalObjective survivalObjective = (SurvivalObjective)objective;
                if (survivalObjective.AnimalSpecies == population.species && !survivalObjective.Populations.Contains(population))
                {
                    Debug.Log(population.name + " was added to survival objective");
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
        bool isAllCompleted = true;
        if (this.PauseManager.IsPaused)
        {
            return;
        }
        if (this.isOpen)
        {
            this.UpdateObjectivePanel();
        }
        foreach (Objective objective in this.objectives)
        {
            if (objective.UpdateStatus() == ObjectiveStatus.InProgress)
            {
                isAllCompleted = false;
            }
        }

        // All objectives had reach end state
        if (isAllCompleted)
        {
            EventManager.Instance.InvokeEvent(EventType.ObjectivesCompleted, null);
            EventManager.Instance.InvokeEvent(EventType.GameOver, null);

            Debug.Log($"Level Completed!");
        }
    }
}
