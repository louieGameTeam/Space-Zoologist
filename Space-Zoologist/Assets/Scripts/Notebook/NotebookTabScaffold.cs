using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotebookTabScaffold
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Scaffold used to determine when different tabs are available")]
    private EnclosureScaffold enclosureScaffold;
    [SerializeField]
    [Tooltip("List of notebook tab masks to match the enclosure scaffold levels")]
    private List<NotebookTabMask> masks;
    #endregion

    #region Public Methods
    public NotebookTabMask GetMask(EnclosureID id) => masks[enclosureScaffold.ScaffoldLevel(id)];
    #endregion
}
