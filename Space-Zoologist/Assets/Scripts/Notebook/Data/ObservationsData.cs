using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObservationsData : NotebookDataModule
{
    #region Public Typedefs
    [System.Serializable]
    public class Entry
    {
        public LevelID level;
        public ObservationsEntryListData observations;
    }
    #endregion

    #region Public Properties
    public IEnumerable<LevelID> LevelsIDs => entries.Select(entry => entry.level);
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of levels that the player has observed so far")]
    private List<Entry> entries = new List<Entry>();
    #endregion

    #region Constructors
    public ObservationsData(NotebookConfig config) : base(config) { }
    #endregion

    #region Public Methods
    public void TryAddEnclosureID(LevelID level)
    {
        int index = entries.FindIndex(entry => entry.level == level);

        if (index < 0)
        {
            entries.Add(new Entry
            {
                level = level,
                observations = new ObservationsEntryListData(Config, level)
            });
        }
    }
    public ObservationsEntryListData GetEntryList(LevelID level)
    {
        int index = entries.FindIndex(entry => entry.level == level);

        if (index >= 0)
        {
            return entries[index].observations;
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ObservationsData)}: " +
            $"no level id '{level}' exists in the observations data");
    }
    #endregion
}