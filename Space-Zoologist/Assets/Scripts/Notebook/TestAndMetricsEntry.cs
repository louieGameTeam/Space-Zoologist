using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player observations on key metrics in play for one single aspect of the enclosure
/// </summary>
[System.Serializable]
public class TestAndMetricsEntry
{
    [SerializeField]
    [Tooltip("The research category on this entry")]
    private ResearchCategory category;
    [SerializeField]
    [Tooltip("The need analyzed in this entry")]
    private NeedType need;
    [SerializeField]
    [Tooltip("True if this marks an increase in need, and false if it marks a decrease")]
    private bool increase;
    [SerializeField]
    [Tooltip("Main text in the entry")]
    private string notes;
}
