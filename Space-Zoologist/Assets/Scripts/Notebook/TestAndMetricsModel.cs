using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestAndMetricsModel
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Enclosure scaffolding information")]
    private EnclosureScaffold enclosureScaffold;
    [SerializeField]
    [TextArea(3, 10)]
    [Tooltip("Initial text for each scaffold level")]
    private List<string> initialTexts;
    #endregion

    #region Private Fields
    // Map the list ot the enclosure it applies to
    private Dictionary<EnclosureID, TestAndMetricsEntryList> observationsEntries = new Dictionary<EnclosureID, TestAndMetricsEntryList>();
    #endregion

    #region Public Methods
    public TestAndMetricsEntryList GetEntryList(EnclosureID id) => observationsEntries[id];
    public void TryAddEnclosureID(EnclosureID id)
    {
        if(!observationsEntries.ContainsKey(id))
        {
            observationsEntries.Add(id, new TestAndMetricsEntryList());
        }
    }
    public string GetInitialEntryText(EnclosureID id) => initialTexts[enclosureScaffold.ScaffoldLevel(id)];
    #endregion
}
