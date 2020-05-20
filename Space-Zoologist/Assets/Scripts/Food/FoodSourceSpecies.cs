using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FoodSourceSpecies : ScriptableObject
{
    public string SpeciesName => speciesName;
    public Dictionary<string, Need> Needs { get; private set; } = new Dictionary<string, Need>();
    public int RootRadius => rootRadius;
    public int BaseOutput => baseOutput;

    [SerializeField] private string speciesName = default;
    [SerializeField] private List<Need> needs = default;
    [SerializeField] private int rootRadius = default;
    [SerializeField] private int baseOutput = default;

    private void OnEnable()
    {
        foreach (Need need in needs)
        {
            //Needs.Add(need.NeedName, need); // TODO: Setup food source need sytem
        }
    }
}
