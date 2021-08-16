using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObservationEntryList
{
    private const string defaultNoteText = "Current Situation: \n\n" +
        "Good for which species?: \n\n" +
        "What more does each species still need to meet target specifications?: \n";
    private const string defaultOtherNoteText = "What can each species access?: \n\n" +
        "What are available resources?: \n";

    public List<ObservationEntry> Entries => entries;

    [SerializeField]
    [Tooltip("List of the observation entries")]
    private List<ObservationEntry> entries;

    public ObservationEntryList(params ObservationEntry[] entries)
    {
        this.entries = new List<ObservationEntry>(entries);
    }

    public static ObservationEntryList Default()
    {
        return new ObservationEntryList
        (
            new ObservationEntry { Title = "Food", Text = defaultNoteText },
            new ObservationEntry { Title = "Water", Text = defaultNoteText },
            new ObservationEntry { Title = "Terrain", Text = defaultNoteText },
            new ObservationEntry { Title = "Other", Text = defaultOtherNoteText }
        );
    }
}
