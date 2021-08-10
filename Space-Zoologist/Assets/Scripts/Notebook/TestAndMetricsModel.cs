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
    private Dictionary<EnclosureID, TestAndMetricsDataSet> entries = new Dictionary<EnclosureID, TestAndMetricsDataSet>();     
}
