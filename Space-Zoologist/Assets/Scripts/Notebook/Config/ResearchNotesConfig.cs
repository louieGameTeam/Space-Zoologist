using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchNotesConfig
{
    #region Public Properties
    public ResearchNoteLabels Labels => labels;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Lables attached to the notes in the research notes")]
    private ResearchNoteLabels labels = null;
    #endregion
}
