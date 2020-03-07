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
    private AnimalPopulationGrowth PopGrowth;
    private List<Need> Needs;
    private Dictionary<string, float> DictionaryOfNeeds;
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

    public void InitializeAnimalPopulation(AnimalPopulationGrowth popGrowth, List<Need> needs, int popSize, string name)
    {
        this.PopGrowth = popGrowth;
        this.Needs = needs;
        this.PopSize = popSize;
        this.populationType = name;
        // Initialize the Dictionary of needs by their need type
        // There should probably be a neutral baseline start for each need?
        DictionaryOfNeeds = new Dictionary<string, float>();
        foreach (Need need in needs)
        {
            DictionaryOfNeeds.Add(need.NeedType, 0);
        }
    }

    // Called whenever an event triggers a system to update its value
    // or when a system calls this delegated method
    public void UpdateNeed(string needType, float value)
    {
        if (DictionaryOfNeeds.ContainsKey(needType))
        {
            DictionaryOfNeeds[needType] = value;
            UpdatePopulationGrowthConditions();
        }
        else
        {
            Debug.Log("Need not found");
        }
    }

    /*
     * Steps:
     * 1. Go through DictionaryOfNeeds grabbing needValues and updatingNeedValue
     * 2. Calcualte growth with the now updated list of needs
     * Note: Calculate growth requires NeedSeverity from each need, so changing List<Needs>
     * to List<Need.NeedConditions> means you will have to pass in List<NeedSeverity> in correct order as well.
     */
    public void UpdatePopulationGrowthConditions()
    {
        foreach (Need need in Needs)
        {
            float needValue = 0;
            DictionaryOfNeeds.TryGetValue(need.NeedType, out needValue);
            need.UpdateValue(needValue);
        }
        this.PopGrowth.CalculateGrowth(Needs, this.GrowthTime, this.PopSize);
        this.GrowthTime = this.PopGrowth.GrowthTime;
        this.GrowthStatus = this.PopGrowth.GrowthStatus;
        Debug.Log("GrowthConditions updated, GrowthTime: " + this.GrowthTime +
            ", GrowthStatus: " + this.GrowthStatus);
        
    }
}
