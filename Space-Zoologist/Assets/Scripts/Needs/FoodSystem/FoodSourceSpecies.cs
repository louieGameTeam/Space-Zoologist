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
    [SerializeField] private List<NeedConstructData> needsList = default;
    [SerializeField] private Item FoodSource = default;


    private void OnEnable()
    {
        if (needsList == null)
        {
            return;
        }
        foreach (NeedConstructData needConstructData in needsList)
        {
            Needs.Add(needConstructData.NeedName, new Need(needConstructData)); // TODO: Setup food source need sytem
        }
    }
}
