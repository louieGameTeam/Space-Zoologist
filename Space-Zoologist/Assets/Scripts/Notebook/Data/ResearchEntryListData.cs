using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchEntryListData : NotebookDataModule
{
    #region Public Properties
    public ResearchEntryData[] Entries => entries;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of research entries")]
    private ResearchEntryData[] entries = new ResearchEntryData[0];
    #endregion

    #region Constructors
    public ResearchEntryListData(NotebookConfig config, ResearchEntryListConfig listConfig) : base(config)
    {
        entries = new ResearchEntryData[listConfig.Entries.Length];
    
        for(int i = 0; i < entries.Length; i++)
        {
            entries[i] = new ResearchEntryData(config, listConfig.Entries[i]);
        }
    }
    #endregion
}
