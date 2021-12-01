using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlocker : MonoBehaviour
{
    #region Public Properties
    public string[] OperationsBlocked => operationsBlocked;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [UIBlockableOperation]
    [Tooltip("Operations that this blocker blocks")]
    private string[] operationsBlocked;
    #endregion

    #region Public Methods
    public bool OperationIsBlocked(string operation) => System.Array.IndexOf(operationsBlocked, operation) >= 0;
    #endregion
}
