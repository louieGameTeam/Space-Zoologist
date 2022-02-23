using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeedRegistry
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of all needs for this species")]
    [ParallelItemRegistry("needArrays", "needs")]
    private NeedDataJaggedArray needData;
    #endregion

    #region Public Methods
    public NeedData[] GetNeedsWithCategory(ItemRegistry.Category category)
    {
        return needData.NeedArrays[(int)category].Needs;
    }
    #endregion
}
