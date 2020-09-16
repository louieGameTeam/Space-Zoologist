using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public enum ObjectiveStatus { Completed, InProgress, Failed}

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
                    this.timer += Time.deltaTime;

                    if (this.timer >= this.TargetTime)
                    {
                        return ObjectiveStatus.Completed;
                    }

                    break;
                }
            }
            return ObjectiveStatus.InProgress;
        }
    }

    public class ResourceObjective : Objective
    {
        private PlayerBalance playerBalance;
        public  int amountToKeep { get; private set; }

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

    [Expandable] public LevelObjectiveData LevelObjectiveData = default;

    private bool isOpen = false;

    // To access the populations 
    [SerializeField] private PopulationManager populationManager = default;
    // To access the player balance
    [SerializeField] private PlayerBalance playerBalance = default;
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

                displayText += $"Maintain at least {survivalObjective.TargetPopulationCount} of the ";
                displayText += $"{survivalObjective.AnimalSpecies.SpeciesName} population at size {survivalObjective.TargetPopulationSize}";
                displayText += $" for {survivalObjective.TargetTime} mins ";
                displayText += $"[{survivalObjective.Status.ToString()}] [{survivalObjective.timer}/{survivalObjective.TargetTime}]\n";
            }
            else if(objective.GetType() == typeof(ResourceObjective))
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
    private void Start()
    {
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
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountIncreased, () =>
        {
            Population population = (Population)EventManager.Instance.EventData;

            foreach (Objective objective in this.objectives)
            {
                if (objective.GetType() == typeof(SurvivalObjective))
                {
                    SurvivalObjective survivalObjective = (SurvivalObjective)objective;

                    if (survivalObjective.AnimalSpecies == population.species && !survivalObjective.Populations.Contains(population))
                    {
                       survivalObjective.Populations.Add(population);
                    }
                }
            }
        });
    }

    /// <summary>
    /// Check the status of the objectives
    /// </summary>
    private void Update()
    {
        bool isAllCompleted = true;

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
