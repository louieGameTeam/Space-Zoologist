using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelDataScriptableObject")]
public class LevelData : ScriptableObject
{
    public List<FoodSourceSpecies> FoodSourceSpecies => foodSources;
    public List<AnimalSpecies> AnimalSpecies => animalSpecies;
    public List<StoreItem> StoreItems => storeItems;

    [SerializeField] private List<FoodSourceSpecies> foodSources = default;
    [SerializeField] private List<AnimalSpecies> animalSpecies = default;
    [SerializeField] private List<StoreItem> storeItems = default;
}
