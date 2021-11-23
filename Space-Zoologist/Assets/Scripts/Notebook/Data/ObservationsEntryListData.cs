using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObservationsEntryListData : NotebookDataModule
{
    #region Public Properties
    public List<ObservationsEntryData> Entries => entries;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of the observation entries")]
    private List<ObservationsEntryData> entries;
    #endregion

    #region Constructors
    public ObservationsEntryListData(NotebookConfig config, LevelID id) : base(config)
    {
        // Copy the list from the initial entries to this one
        LevelScaffold scaffold = config.Observations.Scaffold;
        List<ObservationsEntryListData> initialEntries = config.Observations.InitialEntries;
        ObservationsEntryListData copyList = initialEntries[scaffold.ScaffoldLevel(id)];
        entries = new List<ObservationsEntryData>(copyList.entries);        
    }
    #endregion
}
