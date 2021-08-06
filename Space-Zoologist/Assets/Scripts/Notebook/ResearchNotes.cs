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
        // Tags used to decorate the labels in the notes
        List<RichTextTag> labelTags = new List<RichTextTag>()
        {
            new RichTextTag("color", "white"),
            new RichTextTag("b")
        };

        // Instead, we will have to load the player's data
        Notes = "";
        
        foreach(string label in labels.Labels)
        {
            string richLabel = RichTextTag.ApplyMultiple(labelTags, label + ":");
            Notes += richLabel + " \n\n";
        }
    }
}
