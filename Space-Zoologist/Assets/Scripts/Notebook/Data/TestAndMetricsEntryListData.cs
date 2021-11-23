using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A set of test and metric enries
/// </summary>
[System.Serializable]
public class TestAndMetricsEntryListData
{
    #region Public Properties
    // Access the list of entries
    public List<TestAndMetricsEntryData> Entries => entries;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of entries in this data set")]
    private List<TestAndMetricsEntryData> entries = new List<TestAndMetricsEntryData>();
    #endregion
}
