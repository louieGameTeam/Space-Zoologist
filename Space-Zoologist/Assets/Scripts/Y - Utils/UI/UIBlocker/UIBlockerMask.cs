using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIBlockerMask
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Each value is 'true' if the given operation will be blocked")]
    private bool[] operationsBlocked;
    #endregion

    #region Public Methods
    public bool OperationIsBlocked(string operation)
    {
        int index = UIBlockerSettings.IndexOf(operation);

        if (index >= 0 && index < operationsBlocked.Length)
        {
            return operationsBlocked[index];
        }
        else throw new System.IndexOutOfRangeException(
            $"No mask value could be found for the operation '{operation}'");
    }
    #endregion
}
