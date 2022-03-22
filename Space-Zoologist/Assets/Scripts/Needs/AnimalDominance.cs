using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AnimalDominance))]
public class AnimalDominance : ScriptableObjectSingleton<AnimalDominance>
{
    #region Public Methods
    public static TerrainDominance TerrainDominance => Instance.terrainDominance;
    public static AnimalDominanceItem[] FoodDominance => Instance.foodDominance;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Measures how much each animal dominates other animals " +
        "when competing for the same terrain space")]
    private TerrainDominance terrainDominance;
    [SerializeField]
    [Tooltip("List of dominance amounts for each animal " +
        "competing for shared food sources")]
    private AnimalDominanceItem[] foodDominance;
    #endregion

    #region Public Methods
    public static AnimalDominanceItem GetFoodDominance(ItemID animalID)
    {
        if (animalID.Index >= 0 && animalID.Index < FoodDominance.Length)
        {
            return FoodDominance[animalID.Index];
        }
        else throw new System.ArgumentException(
            $"Animal ID '{animalID}' has no food dominance associated with it");
    }
    #endregion
}
