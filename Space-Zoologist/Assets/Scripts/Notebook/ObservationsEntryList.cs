using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObservationsEntryList
{
    public List<ObservationsEntry> Entries => entries;

    [SerializeField]
    [Tooltip("List of the observation entries")]
    private List<ObservationsEntry> entries;

    public ObservationsEntryList(ObservationsEntryList other)
    {
        entries = new List<ObservationsEntry>(other.entries);
    }
}
