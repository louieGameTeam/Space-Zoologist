using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntryListConfig
{
    #region Public Properties
    public ResearchEntryConfig[] Entries => entries;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of research entries")]
    private ResearchEntryConfig[] entries;
    #endregion
}
