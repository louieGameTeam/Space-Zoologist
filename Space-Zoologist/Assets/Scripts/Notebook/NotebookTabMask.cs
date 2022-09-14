using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotebookTabMask
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("True false value for each notebook tab")]
    private bool[] mask;
    #endregion

    #region Public Methods
    public bool Get(NotebookTab tab) => mask[(int)tab];
    #endregion
}
