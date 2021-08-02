using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchNotes
{
    // Public accessors

    public ResearchNoteLabels Labels => labels;
    public string Notes { get; set; }

    // Private editor data

    [SerializeField]
    [Expandable]
    [Tooltip("Lables attached to the notes in the research notes")]
    private ResearchNoteLabels labels;

    public void Setup()
    {
        // Instead, we will have to load the player's data
        Notes = "";
        foreach(string label in Labels.Labels)
        {
            Notes += "<color=yellow>" + label + "</color>: \n\n";
        }
    }
}
