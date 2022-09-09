using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ResearchConfig
{
    #region Public Typedefs
    [System.Serializable]
    public class ResearchEntryRegistry
    {
        public ResearchEntryListConfig[] entryLists;
    }
    #endregion

    #region Public Properties
    public ResearchEntryListConfig[] ResearchEntryLists => researchEntryRegistry.entryLists;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of research entry lists - used to make the entries parallel to the item registry")]
    [FormerlySerializedAs("researchEntryData")]
    [ParallelItemRegistry("entryLists", "entries")]
    private ResearchEntryRegistry researchEntryRegistry = null;
    #endregion

    #region Public Methods
    public ResearchEntryConfig GetEntry(ItemID id)
    {
        ResearchEntryConfig[] entries = researchEntryRegistry.entryLists[(int)id.Category].Entries;
        if (id.Index >= 0 && id.Index < entries.Length) return entries[id.Index];
        else return null;
    }
    #endregion
}
