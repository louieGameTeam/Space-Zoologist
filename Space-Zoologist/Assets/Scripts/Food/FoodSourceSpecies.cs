using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FoodSourceSpecies : ScriptableObject
{
    public string SpeciesName => speciesName;
    public Dictionary<string, Need> Needs => needs;
    public int RootRadius => rootRadius;
    public int BaseOutput => baseOutput;

    [SerializeField] private string speciesName = default;
    [SerializeField] private int rootRadius = default;
    [SerializeField] private int baseOutput = default;
    [SerializeField] private Dictionary<string, Need> needs = new Dictionary<string, Need>();
    [SerializeField] private List<NeedData> needsList = default;


    private void OnEnable()
    {
        foreach (NeedData needData in needsList)
        {
            // Use the NeedData to create Need
            Needs.Add(needData.NeedName, new Need(needData)); 
        }
    }
}
