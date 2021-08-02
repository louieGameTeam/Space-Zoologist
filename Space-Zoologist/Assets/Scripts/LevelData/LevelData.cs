using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the starting data of a particular level.
/// </summary>
[CreateAssetMenu(fileName="LevelData", menuName="Scene Data/LevelData")]
public class LevelData : ScriptableObject
{
    [System.Serializable]
    public class ItemData
    {
        [Expandable] public Item itemObject;
        public int initialAmount;

        public ItemData(Item item)
        {
            itemObject = item;
        }
    }

    public Level Level = default;
    public float StartingBalance => startingBalance;
    public List<FoodSourceSpecies> FoodSourceSpecies => foodSources;
    public List<AnimalSpecies> AnimalSpecies => animalSpecies;
    public List<ItemData> ItemQuantities => itemQuantities;
    public AtmosphericComposition GlobalAtmosphere => globalAtmosphere;

    [SerializeField] public float startingBalance = default;
    [SerializeField] public int MapWidth = default;
    [SerializeField] public int MapHeight = default;
    [Expandable] public LevelObjectiveData LevelObjectiveData = default;
    [Expandable] public List<FoodSourceSpecies> foodSources = default;
    [Expandable] public List<AnimalSpecies> animalSpecies = default;
    [SerializeField] public List<ItemData> itemQuantities = default;
    [SerializeField] private AtmosphericComposition globalAtmosphere = new AtmosphericComposition(1,2,3,90);

}
