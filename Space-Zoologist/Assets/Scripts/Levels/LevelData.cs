using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelDataScriptableObject")]
public class LevelData : ScriptableObject
{
    public List<FoodSourceSpecies> FoodSources => foodSources;
    [SerializeField] private List<FoodSourceSpecies> foodSources = default;
}
