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
}
