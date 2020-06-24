using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AnimalSpecies : ScriptableObject
{
    // Getters
    public string SpeciesName => speciesName;
    public int Dominance => dominance;
    public float GrowthFactor => growthFactor;
    public Dictionary<string, Need> Needs => needs;
    public float Size => size;
    public List<TileType> AccessibleTerrain => accessibleTerrain;
    public Sprite Icon => icon;
    public Sprite Representation => representation;

    // Values
    [SerializeField] private string speciesName = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private int dominance = default;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float growthFactor = default;
    [SerializeField] private Dictionary<string, Need> needs = new Dictionary<string, Need>();
    [SerializeField] private List<Need> needsList = default;
    [Range(0.0f, 10.0f)]
    [SerializeField] private float size = default;
    [SerializeField] private List<TileType> accessibleTerrain = default;
    [SerializeField] private Sprite icon = default;

    // Replace later with actual representation/animations/behaviors
    [SerializeField] private Sprite representation = default;

    private void OnEnable()
    {
        foreach (Need need in needsList)
        {
            needs.Add(need.NeedName, need);
        }
    }
}
