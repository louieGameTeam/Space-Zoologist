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
    [SerializeField] private List<Need> needsList = default;


    private void OnEnable()
    {
        foreach (Need need in needsList)
        {
            Needs.Add(need.NeedName, need); // TODO: Setup food source need sytem
        }
    }
}
