using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ObservationsConfig
{
    #region Public Properties
    public LevelScaffold Scaffold => scaffold;
    public List<ObservationsEntryListData> InitialEntries => initialEntries;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [FormerlySerializedAs("enclosureScaffold")]
    [Tooltip("Scaffolding of the observation page based on the current enclosure level")]
    private LevelScaffold scaffold = null;
    [SerializeField]
    [Tooltip("Set of initial entries corresponding to each scaffolding level")]
    private List<ObservationsEntryListData> initialEntries = null;
    #endregion
}
