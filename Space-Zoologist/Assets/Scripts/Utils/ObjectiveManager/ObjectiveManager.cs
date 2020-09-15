using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public abstract class Objective
    {
        public abstract bool Status { get; }
        public abstract void UpdateStatus();
    }


    public class SurvivalObjective : Objective
    {
        public List<Population> populations = default;
        private byte targetPopulationCount;
        private byte targetPopulationSize;
        private float targetTime;

        private float timer;
        private bool status;

        public override bool Status => this.status;

        public SurvivalObjective(List<Population> populations, byte targetPopulationCount, byte targetPopulationSize, float targetTime)
        {
            this.populations = populations;
            this.status = false;
        }

        public override void UpdateStatus()
        {
            throw new NotImplementedException();
        }
    }

    public class ResourceObjective : Objective
    {
        private PlayerBalance playerBalance;
        private int amountToKeep;

        public override bool Status => this.status;

        private bool status;

        public ResourceObjective(PlayerBalance playerBalance, int amountToKeep)
        {
            this.playerBalance = playerBalance;
            this.amountToKeep = amountToKeep;
            this.status = false;
        }

        public override void UpdateStatus()
        {
            if (this.playerBalance.Balance >= this.amountToKeep)
            {
                this.status = true;
            }
            else
            {
                this.status = false;
            }
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

    private List<Objective> objectives = new List<Objective>();
   
    public void ToggleObjectivePanel()
    {
        this.isOpen = !this.isOpen;
        this.objectivePanel.SetActive(this.isOpen);
    }

    private void Awake()
    {
        // Read in level objectives and create objectives
        foreach(SurvivalObjectiveData objectiveData in this.LevelObjectiveData.survivalObjectiveDatas)
        {
            //this.objectives.Add
        }

        foreach(ResourceObjectiveData objectiveData in this.LevelObjectiveData.resourceObjectiveDatas)
        {
            this.objectives.Add(new ResourceObjective(this.playerBalance, objectiveData.amountToKeep));
        }



        // Subscribe to event
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountIncreased, () =>
        {
            Population population = (Population)EventManager.Instance.EventData;

        });
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountDecreased, () =>
        {
            Population population = (Population)EventManager.Instance.EventData;

        });
    }
}
