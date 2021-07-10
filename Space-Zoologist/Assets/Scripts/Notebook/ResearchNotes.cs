using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchNotes
{
    // Public accessors

    public ResearchNoteLabels Labels => labels;

    // Private editor data

    [SerializeField]
    [Expandable]
    [Tooltip("Lables attached to the notes in the research notes")]
    private ResearchNoteLabels labels;

    // For faster lookup
    private Dictionary<string, string> notes = new Dictionary<string, string>();

    public void Setup()
    {
        foreach (string label in labels.Labels) notes.Add(label, "");
    }

    public string ReadNote(string label) => notes[label];
    public void WriteNote(string label, string note)
    {
        notes[label] = note;
    }
}
