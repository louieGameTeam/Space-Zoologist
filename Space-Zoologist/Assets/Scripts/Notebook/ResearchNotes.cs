using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchNotes
{
    #region Public Properties
    public ResearchNoteLabels Labels => labels;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Expandable]
    [Tooltip("Lables attached to the notes in the research notes")]
    private ResearchNoteLabels labels;
    #endregion

    #region Private Fields
    // Maps the label to the note's text
    private Dictionary<string, string> notes = new Dictionary<string, string>();
    #endregion

    #region Public Methods
    public void Setup()
    {
        // Later we will have to actually load the save data
        notes.Clear();

        // Add an empty note for each label
        foreach (string label in labels.Labels)
        {
            notes.Add(label, "");
        }
    }
    public string ReadNote(string label) => notes[label];
    public string WriteNote(string label, string note) => notes[label] = note;
    #endregion
}
