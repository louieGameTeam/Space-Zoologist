using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeedDataJaggedArray
{
    #region Public Properties
    public NeedDataArray[] NeedArrays => needArrays;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of need data arrays")]
    private NeedDataArray[] needArrays = new NeedDataArray[0];
    #endregion

    #region Public Methods

    #endregion
}
