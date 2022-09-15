using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlocker : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Operations that this blocker blocks")]
    private UIBlockerMask operationsBlocked = null;
    #endregion

    #region Public Methods
    public bool OperationIsBlocked(string operation) => operationsBlocked.OperationIsBlocked(operation);
    #endregion
}
