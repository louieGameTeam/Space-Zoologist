using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the starting data of a particular level.
/// </summary>
[CreateAssetMenu(fileName="LevelData", menuName="Scene Data/LevelData")]
public class LevelData : ScriptableObject
{
    public IntVariable StartingBalance => startingBalance;
    public List<FoodSourceSpecies> FoodSourceSpecies => foodSources;
    public List<AnimalSpecies> AnimalSpecies => animalSpecies;
    public List<Item> Items => items;
    public AtmosphericComposition GlobalAtmosphere => globalAtmosphere;

    [Expandable] public IntVariable startingBalance = default;
    [SerializeField] public int MapWidth = default;
    [SerializeField] public int MapHeight = default;
    [Expandable] public List<FoodSourceSpecies> foodSources = default;
    [Expandable] public List<AnimalSpecies> animalSpecies = default;
    [Expandable] public List<Item> items = default;
    [SerializeField] private AtmosphericComposition globalAtmosphere = new AtmosphericComposition(1,2,3,90);
}
