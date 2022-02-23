using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeedData
{
    #region Public Properties
    public bool Needed => needed;
    public int PreferenceRating => preferenceRating;
    public SpeciesNeedType SpeciesNeedType => speciesNeedType;
    public float Minimum => minimum;
    public float Maximum => maximum;
    public ItemID ID => id;
    public bool IsWater => id.Data.Name.AnyNameContains("Water");
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Determine if this item is needed by the species")]
    private bool needed;

    [SerializeField]
    [Tooltip("How much the need is preferred. Larger numbers indicate " +
        "a higher preference. Values below zero are used to indicate " +
        "items that do not contribute to the need of the species, as is " +
        "the case with terrain that species can traverse but cannot " +
        "contribute to the overall terrain need")]
    private int preferenceRating = -1;

    [SerializeField]
    [Tooltip("The way in which this species needs this animal")]
    private SpeciesNeedType speciesNeedType;

    [SerializeField]
    [Tooltip("A minimum of this much water needs to be present " +
        "to be drinkable by this species")]
    [Range(0f, 1f)]
    private float minimum;
    [SerializeField]
    [Tooltip("Water with more than this maximum of water " +
        "cannot be drinkable by this species")]
    [Range(0f, 1f)]
    private float maximum;
    #endregion

    #region Private Fields
    [SerializeField]
    [HideInInspector]
    private ItemID id;
    #endregion
}
