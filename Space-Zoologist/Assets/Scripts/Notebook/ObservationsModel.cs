using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObservationsModel
{
    #region Public Properties
    public List<EnclosureID> EnclosureIDs => new List<EnclosureID>(observationsEntries.Keys);
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Scaffolding of the observation page based on the current enclosure level")]
    private EnclosureScaffold enclosureScaffold;
    [SerializeField]
    [Tooltip("Set of initial entries corresponding to each scaffolding level")]
    private List<ObservationsEntryList> initialEntries;
    #endregion

    #region Private Fields
    // Map the list to the enclosure it applies to
    private Dictionary<EnclosureID, ObservationsEntryList> observationsEntries = new Dictionary<EnclosureID, ObservationsEntryList>();
    #endregion

    #region Public Methods
    public ObservationsEntryList GetEntryList(EnclosureID id) => observationsEntries[id];
    public void TryAddEnclosureID(EnclosureID id)
    {
        if (!observationsEntries.ContainsKey(id))
        {
            observationsEntries.Add(id, new ObservationsEntryList(initialEntries[enclosureScaffold.ScaffoldLevel(id)]));
        }
    }
    #endregion
}
