using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalDominanceItem
{
    #region Public Properties
    public float Dominance => dominance;
    public ItemID ID => id;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Amount that this animal dominates over other animals for this resource")]
    private float dominance = 0f;
    #endregion

    #region Private Fields
    [SerializeField]
    [HideInInspector]
    private ItemID id = new ItemID(ItemRegistry.Category.Species, 0);
    #endregion
}
