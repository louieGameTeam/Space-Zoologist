using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeedDataArray
{
    #region Public Properties
    public NeedData[] Needs => needs;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of needs")]
    private NeedData[] needs;
    #endregion
}
