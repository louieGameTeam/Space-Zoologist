using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO have this create the food source item and hold it, then have the store display that info
[CreateAssetMenu]
public class FoodSourceSpecies : ScriptableObject
{
    public string SpeciesName => speciesName;
    public Dictionary<string, Need> Needs => needs;
    public int RootRadius => rootRadius;
    public int BaseOutput => baseOutput;
    public Item FoodSourceItem => FoodSource;

    [SerializeField] private string speciesName = default;
    [SerializeField] private int rootRadius = default;
    [SerializeField] private int baseOutput = default;
    [SerializeField] private Dictionary<string, Need> needs = new Dictionary<string, Need>();
    [SerializeField] private List<NeedTypeConstructData> needsList = default;
    [SerializeField] private Item FoodSource = default;


    private void OnEnable()
    {
        foreach (NeedTypeConstructData needData in needsList)
        {
            foreach (NeedConstructData need in needData.Needs)
            {
                // Use the NeedData to create Need
                Needs.Add(need.NeedName, new Need(needData.NeedType, need));
            }
        }
    }

    public void SetupData(string name, int rootRadius, int output, List<NeedTypeConstructData> needs)
    {
        this.needsList = new List<NeedTypeConstructData>();
        this.speciesName = name;
        this.rootRadius = rootRadius;
        this.baseOutput = output;
        this.needsList = needs;
    }
}
