using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntryConfig
{
    #region Public Properties
    public ResearchNoteLabels NoteLabels => noteLabels;
    public ResearchEncyclopediaConfig Encyclopedia => encyclopedia;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    private ResearchNoteLabels noteLabels;
    [SerializeField]
    private ResearchEncyclopediaConfig encyclopedia;
    #endregion
}
