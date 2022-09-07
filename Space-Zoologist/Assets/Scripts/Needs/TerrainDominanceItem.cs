using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainDominanceItem
{
    #region Public Properties
    public AnimalDominanceItem[] Dominances => dominances;
    public TileType EnumValue => enumValue;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Determines animal dominance on this terrain type")]
    private AnimalDominanceItem[] dominances = null;
    #endregion

    #region Private Fields
    [SerializeField]
    [HideInInspector]
    private TileType enumValue = TileType.Dirt;
    #endregion
}
