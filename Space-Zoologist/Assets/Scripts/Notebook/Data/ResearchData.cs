using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResearchData : NotebookDataModule
{
    public ResearchEntryListData [] ResearchEntryData => researchEntryData;

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of research data that the player has added to")]
    private ResearchEntryListData[] researchEntryData = new ResearchEntryListData[0];
    #endregion

    #region Constructors
    public ResearchData(NotebookConfig config) : base(config) 
    {
        // Initialize the list to be parallel to the config list
        researchEntryData = new ResearchEntryListData[config.Research.ResearchEntryLists.Length];

        // Initialize each list in the list of lists
        for(int i = 0; i < config.Research.ResearchEntryLists.Length; i++)
        {
            ResearchEntryListConfig listConfig = config.Research.ResearchEntryLists[i];
            researchEntryData[i] = new ResearchEntryListData(config, listConfig);
        }
    }
    #endregion

    #region Public Methods
    public override void SetConfig(NotebookConfig config)
    {
        base.SetConfig(config);

        if(researchEntryData.Length != config.Research.ResearchEntryLists.Length)
        {
            // Initialize the list to be parallel to the config list
            researchEntryData = new ResearchEntryListData[config.Research.ResearchEntryLists.Length];

            // Initialize each list in the list of lists
            for (int i = 0; i < config.Research.ResearchEntryLists.Length; i++)
            {
                ResearchEntryListConfig listConfig = config.Research.ResearchEntryLists[i];
                researchEntryData[i] = new ResearchEntryListData(config, listConfig);
            }
        }
    }
    public ResearchEntryData GetEntry(ItemID id)
    {
        int categoryIndex = (int)id.Category;

        if (categoryIndex >= 0 && categoryIndex < researchEntryData.Length)
        {
            ResearchEntryData[] entries = researchEntryData[(int)id.Category].Entries;

            if (id.Index >= 0 && id.Index < entries.Length)
            {
                return entries[id.Index];
            }
            else throw new System.IndexOutOfRangeException($"{nameof(ResearchData)}: " +
                $"no research entry found at category '{id.Category}' " +
                $"and index '{id.Index}'" +
                $"\n\tCount entries at category: {entries.Length}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ResearchData)}: " +
            $"no research entry list found for category '{id.Category}'" +
            $"\n\tCategory index: {categoryIndex}" +
            $"\n\tEntry list count: {researchEntryData.Length}");
    }
    #endregion
}
