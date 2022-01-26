using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestAndMetricsData : NotebookDataModule
{
    #region Public Typedefs
    [System.Serializable]
    public class Entry
    {
        public LevelID level;
        public TestAndMetricsEntryListData entries;
    }
    #endregion

    #region Public Properties
    public List<LevelID> Levels => levels;
    public List<TestAndMetricsEntryListData> ListDatas => listDatas;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of levels that the player has test and metric data for")]
    private List<Entry> entries = new List<Entry>();
    #endregion

    #region Constructors
    public TestAndMetricsData(NotebookConfig config) : base(config) { }
    #endregion

    #region Public Methods
    public TestAndMetricsEntryListData GetEntryList(LevelID level)
    {
        int index = entries.FindIndex(entry => entry.level == level);

        if (index >= 0)
        {
            return entries[index].entries;
        }
        else throw new System.IndexOutOfRangeException($"{nameof(TestAndMetricsData)}: " +
            $"level id '{level}' not found in the list of ids" +
            $"\n\tIDs present: [ {string.Join(", ", entries.Select(entry => entry.level))} ]");
    }
    public void TryAddEnclosureID(LevelID level)
    {
        int index = entries.FindIndex(entry => entry.level == level);

        if (index < 0)
        {
            entries.Add(new Entry
            {
                level = level,
                entries = new TestAndMetricsEntryListData()
            });
        }
    }
    #endregion
}
