using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data for a single need of a 
/// food or animal species
/// </summary>
/// <remarks>
/// The need data has a lot of information that goes unused
/// for many need types. Custom editor code is used to display
/// only the properties that apply for the item identified
/// by the need
/// </remarks>
[System.Serializable]
public class NeedData
{
    #region Public Typedefs
    public class PreferenceComparer : IComparer<NeedData> 
    {
        public int Compare(NeedData a, NeedData b)
        {
            return a.preferred.CompareTo(b.preferred);
        }
    }
    #endregion

    #region Public Properties
    public bool Needed => needed;
    public SpeciesNeedType SpeciesNeedType => speciesNeedType;
    public bool UseAsTerrainNeed => useAsTerrainNeed;
    public bool TraversibleOnly => traversibleOnly;
    public FoodNeedType FoodNeedType => foodNeedType;
    public bool Preferred => preferred;
    public bool UseAsWaterNeed => useAsWaterNeed;
    public float Minimum => minimum;
    public float Maximum => maximum;
    public ItemID ID => id;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Determine if this item is needed by the species")]
    private bool needed;

    [SerializeField]
    [Tooltip("The way in which this species needs this animal")]
    private SpeciesNeedType speciesNeedType;

    [SerializeField]
    [Tooltip("If true, then this water need should be treated as terrain " +
        "that the species can traverse")]
    private bool useAsTerrainNeed = false;
    [SerializeField]
    [Tooltip("A traversible only terrain can be traversed by the species " +
        "but does not contribute to the species terrain need")]
    private bool traversibleOnly = false;

    [SerializeField]
    [Tooltip("How the food source is used. The species may use it as food to eat " +
        "or as a tree that it can nest in")]
    private FoodNeedType foodNeedType;
    [SerializeField]
    [Tooltip("How much the need is preferred compared to other needs" +
        " with the same type")]
    private bool preferred = false;
    
    [SerializeField]
    [Tooltip("If true, then this water need should be treated as water " +
        "that the species can drink from")]
    private bool useAsWaterNeed = true;
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
