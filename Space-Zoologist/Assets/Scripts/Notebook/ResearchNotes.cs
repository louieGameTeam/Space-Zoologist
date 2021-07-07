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
    private Dictionary<string, string> notes;

    public void Awake()
    {
        foreach (string label in labels.Labels) notes.Add(label, null);
    }

    public string ReadNote(string category) => notes[category];
    public void WriteNote(string category, string note)
    {
        notes[category] = note;
    }
}
