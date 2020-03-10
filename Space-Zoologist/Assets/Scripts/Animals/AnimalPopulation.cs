using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Each AnimalPopulation calculates its own growth
 */
public class UpdateNeedEvent : UnityEvent<string, float> { }
public class AnimalPopulation : MonoBehaviour
{
    private Species species = default;
    public Species Species { get => species; private set => species = value; }
    public string SpeciesName { get => species._speciesType; }
    private Dictionary<string, float> Needs = new Dictionary<string, float>();
    public int PopSize { get; set; }
    private float GrowthTime = 60;
    private AnimalPopulationGrowth.PopGrowthStatus GrowthStatus;
    private float Timer = 0;
    public string populationType { get; set; }
    public float Dominace { get; set; }

    public void Start()
    {
        EventManager.StartListening("FoodDistributed", UpdatePopulationGrowthConditions);
    }

    public void Update()
    {
        this.Timer += Time.deltaTime;
        if (this.Timer > this.GrowthTime)
        {
            // Do we want growth condtions updated every cycle or invoked in another way?
            this.PopSize += (int)this.GrowthStatus;
            this.Timer = 0;
        }
    }

    public void InitializeFromSpecies(Species _species)
    {
        this.species = _species;
        foreach (Need need in _species.Needs)
        {
            Needs.Add(need.NeedName, 0);
            NeedSystemManager.RegisterPopulation(this, need.NeedName);
        }
    }

    // Called whenever an event triggers a system to update its value
    // or when a system calls this delegated method
    public void UpdateNeed(string NeedName, float value)
    {
        if (Needs.ContainsKey(NeedName))
        {
            Needs[NeedName] = value;
            UpdatePopulationGrowthConditions();
        }
        else
        {
            Debug.Log("Need not found");
        }
    }

    /*
     * Steps:
     * 1. Go through species needs, updating value with dictionary value
     * 2. Calcualte growth with the now updated list of needs
     * Note: Calculate growth requires NeedSeverity
     * TODO: Update growth calculator to include population size
     */
    public void UpdatePopulationGrowthConditions()
    {
        int needIndex = 0;
        foreach (var need in Needs)
        {
            this.species.Needs[needIndex].UpdateValue(need.Value);
            needIndex++;
        }
        this.species.PopGrowth.CalculateGrowth(this.species.Needs, this.GrowthTime, this.PopSize);
        this.GrowthTime = this.species.PopGrowth.GrowthTime;
        this.GrowthStatus = this.species.PopGrowth.GrowthStatus;
        Debug.Log("GrowthConditions updated, GrowthTime: " + this.GrowthTime +
            ", GrowthStatus: " + this.GrowthStatus);    
    }
}
