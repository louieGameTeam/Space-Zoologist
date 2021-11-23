using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObservationsData : NotebookDataModule
{
    #region Public Properties
    public List<LevelID> LevelsIDs => levelIDs;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of levels that the player has observed so far")]
    private List<LevelID> levelIDs = new List<LevelID>();
    [SerializeField]
    [Tooltip("List of observation entry lists by level id")]
    private List<ObservationsEntryListData> entryLists = new List<ObservationsEntryListData>();
    #endregion

    #region Constructors
    public ObservationsData(NotebookConfig config) : base(config) { }
    #endregion

    #region Public Methods
    public void TryAddEnclosureID(LevelID id)
    {
        if (!levelIDs.Contains(id))
        {
            levelIDs.Add(id);
            entryLists.Add(new ObservationsEntryListData(Config, id));
        }
    }
    public ObservationsEntryListData GetEntryList(LevelID id)
    {
        int index = levelIDs.IndexOf(id);

        if (index >= 0)
        {
            if (index < entryLists.Count)
            {
                return entryLists[index];
            }
            else throw new System.IndexOutOfRangeException($"{nameof(ObservationsData)}: " +
                $"no list of observations corresponds to the level id '{id}'" +
                $"\n\tLevel ID index: {index}" +
                $"\n\tCount observation lists: {entryLists.Count}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ObservationsData)}: " +
            $"no level id '{id}' exists in the observations data");
    }
    #endregion
}