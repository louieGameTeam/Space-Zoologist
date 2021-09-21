using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntry
{
    #region Public Properties
    public ResearchNotes Notes => notes;
    public ResearchEncyclopedia Encyclopedia => encyclopedia;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    private ResearchNotes notes;
    [SerializeField]
    private ResearchEncyclopedia encyclopedia;
    #endregion

    #region Public Methods
    public void Setup()
    {
        notes.Setup();
        encyclopedia.Setup();
    }
    #endregion
}
