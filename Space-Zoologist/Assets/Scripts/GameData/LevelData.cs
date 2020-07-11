using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the starting data of a particular level.
/// Note: Instead of having the data of each level stored in a scriptable object like this, the data could be stored in the its related components in each level's scene.
/// </summary>
[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelDataScriptableObject")]
public class LevelData : ScriptableObject
{
    public int StartingBalance => startingBalance;
    public List<FoodSourceSpecies> FoodSourceSpecies => foodSources;
    public List<AnimalSpecies> AnimalSpecies => animalSpecies;
    public List<Item> Items => items;
    public AtmosphericComposition GlobalAtmosphere => globalAtmosphere;

    [SerializeField] private int startingBalance = default;
    [Expandable] public List<FoodSourceSpecies> foodSources = default;
    [Expandable] public List<AnimalSpecies> animalSpecies = default;
    [Expandable] public List<Item> items = default;
    [SerializeField] private AtmosphericComposition globalAtmosphere = new AtmosphericComposition(1,2,3,90);
}
