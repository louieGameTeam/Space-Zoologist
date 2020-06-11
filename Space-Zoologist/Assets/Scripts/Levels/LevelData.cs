using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelDataScriptableObject")]
public class LevelData : ScriptableObject
{
    public List<FoodSourceSpecies> FoodSourceSpecies => foodSources;
    public List<AnimalSpecies> AnimalSpecies => animalSpecies;
    public List<StoreItem> StoreItems => storeItems;
    public AtmosphericComposition GlobalAtmosphere => globalAtmosphere;

    [SerializeField] private List<FoodSourceSpecies> foodSources = default;
    [SerializeField] private List<AnimalSpecies> animalSpecies = default;
    [SerializeField] private List<StoreItem> storeItems = default;
    [SerializeField] private AtmosphericComposition globalAtmosphere = new AtmosphericComposition(1,2,3,90);
}
