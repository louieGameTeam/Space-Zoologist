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
    public Sprite FoodsourceSprite => foodsourceSprite;

    [SerializeField] private string speciesName = default;
    [SerializeField] private List<Need> needs = default;
    [SerializeField] private int rootRadius = default;
    [SerializeField] private int baseOutput = default;

    // This should later be made into a dynamic representation that changes based on the condition of the foodsource's needs.
    [SerializeField] private Sprite foodsourceSprite = default;
}
