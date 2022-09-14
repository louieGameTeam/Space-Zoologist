using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainDominance
{
    #region Public Typedefs
    [System.Serializable]
    public class TerrainDominanceItemArray
    {
        public TerrainDominanceItem[] array;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of terrain dominance for each tile for each species")]
    [EditArrayWrapperOnEnum(typeof(TileType))]
    private TerrainDominanceItemArray itemsArray;
    #endregion

    #region Public Methods
    public AnimalDominanceItem[] GetAllAnimalDominances(TileType tile)
    {
        return itemsArray.array[(int)tile].Dominances;
    }
    public AnimalDominanceItem GetAnimalDominance(TileType tile, ItemID animalID)
    {
        AnimalDominanceItem[] dominanceItems = GetAllAnimalDominances(tile);

        if (animalID.Index >= 0 && animalID.Index < dominanceItems.Length)
        {
            return dominanceItems[animalID.Index];
        }
        else throw new System.ArgumentException(
            $"Item ID '{animalID}' is not associated with any " +
            $"animal dominance for terrain type '{tile}'");
    }
    #endregion
}
