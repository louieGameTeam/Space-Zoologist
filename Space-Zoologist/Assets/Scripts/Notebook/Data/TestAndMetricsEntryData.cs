using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player observations on key metrics in play for one single aspect of the enclosure
/// </summary>
[System.Serializable]
public class TestAndMetricsEntryData
{
    #region Public Properties
    public ItemID Item
    {
        get => item;
        set => item = value;
    }
    public NeedType Need
    {
        get => need;
        set => need = value;
    }
    public bool Improved
    {
        get => imporoved;
        set => imporoved = value;
    }
    public string Notes
    {
        get => notes;
        set => notes = value;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("The item addressed in this entry")]
    private ItemID item;
    [SerializeField]
    [Tooltip("The need analyzed in this entry")]
    private NeedType need;
    [SerializeField]
    [Tooltip("True if this marks an increase in need, and false if it marks a decrease")]
    private bool imporoved;
    [SerializeField]
    [Tooltip("Main text in the entry")]
    private string notes;
    #endregion
}
