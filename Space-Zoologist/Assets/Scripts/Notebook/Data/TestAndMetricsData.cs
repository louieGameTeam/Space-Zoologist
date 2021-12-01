using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestAndMetricsData : NotebookDataModule
{
    #region Public Properties
    public List<LevelID> Levels => levels;
    public List<TestAndMetricsEntryListData> ListDatas => listDatas;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of levels that the player has test and metric data for")]
    private List<LevelID> levels = new List<LevelID>();
    [SerializeField]
    [Tooltip("List of entry lists, parallel to the levels list")]
    private List<TestAndMetricsEntryListData> listDatas = new List<TestAndMetricsEntryListData>();
    #endregion

    #region Constructors
    public TestAndMetricsData(NotebookConfig config) : base(config) { }
    #endregion

    #region Public Methods
    public TestAndMetricsEntryListData GetEntryList(LevelID id)
    {
        int index = levels.IndexOf(id);

        if (index >= 0)
        {
            if (index < listDatas.Count)
            {
                return listDatas[index];
            }
            else throw new System.IndexOutOfRangeException($"{nameof(TestAndMetricsData)}: " +
                $"no data list corresponding to the level id '{id}'" +
                $"\n\tID index: {index}" +
                $"\n\tData list count: {listDatas.Count}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(TestAndMetricsData)}: " +
            $"level id '{id}' not found in the list of ids" +
            $"\n\tIDs present: [ {string.Join(", ", levels)} ]");
    }
    public void TryAddEnclosureID(LevelID id)
    {
        if (!levels.Contains(id))
        {
            levels.Add(id);
            listDatas.Add(new TestAndMetricsEntryListData());
        }
    }
    #endregion
}
