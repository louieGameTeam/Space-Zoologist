using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchNotes
{
    [SerializeField]
    [Expandable]
    private ResearchNoteLabels labels;

    // For faster lookup
    private Dictionary<string, string> notes = new Dictionary<string, string>();

    public void Setup()
    {
        foreach (string label in labels.Labels) notes.Add(label, "");
    }

    public string ReadNote(string category) => notes[category];
    public void WriteNote(string category, string note)
    {
        notes[category] = note;
    }
}
