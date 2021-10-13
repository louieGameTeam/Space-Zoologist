using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntryList
{
    #region Public Properties
    public ResearchEntry[] Entries => entries;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of research entries")]
    private ResearchEntry[] entries;
    #endregion
}
