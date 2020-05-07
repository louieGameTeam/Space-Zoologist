using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FoodSourceSpecies : ScriptableObject
{ 
    public Dictionary<string, Need> Needs { get; private set; }
    public int RootRadius => rootRadius;
    public int BaseOutput => baseOutput;

    [SerializeField] private List<Need> needs = default;
    [SerializeField] private int rootRadius = default;
    [SerializeField] private int baseOutput = default;

    private void OnEnable()
    {
        Needs = new Dictionary<string, Need>();
        foreach (Need need in needs)
        {
            Needs.Add(need.NeedName, need);
        }
    }
}
