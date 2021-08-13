using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all of the test and metrics entries, 
/// mapping the entries to the level/enclosure the notes were taken about
/// </summary>
[System.Serializable]
public class TestAndMetricsModel
{
    // Public accessor of private data
    public Dictionary<EnclosureID, TestAndMetricsDataSet> Entries => entries;

    private Dictionary<EnclosureID, TestAndMetricsDataSet> entries = new Dictionary<EnclosureID, TestAndMetricsDataSet>();  
    
    /// <summary>
    /// Add the id if it is not already in the dictionary
    /// </summary>
    /// <param name="id"></param>
    public void TryAddID(EnclosureID id)
    {
        if(!entries.ContainsKey(id)) entries.Add(id, new TestAndMetricsDataSet());
    }
}
