using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObservationsModel
{
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
    private Dictionary<EnclosureID, ObservationsEntryList> data = new Dictionary<EnclosureID, ObservationsEntryList>();
    #endregion

    #region Public Methods
    public ObservationsEntryList GetEntryList(EnclosureID id) => data[id];
    public void TryAddEnclosureID(EnclosureID id)
    {
        if (!data.ContainsKey(id))
        {
            data.Add(id, new ObservationsEntryList(initialEntries[enclosureScaffold.ScaffoldLevel(id)]));
        }
    }
    #endregion
}
