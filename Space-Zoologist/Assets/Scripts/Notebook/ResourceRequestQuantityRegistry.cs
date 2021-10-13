using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceRequestQuantityRegistry
{
    #region Public Typedefs
    [System.Serializable]
    public class ArrayOfIntArrays
    {
        public IntArray[] intArrays;
    }
    [System.Serializable]
    public class IntArray
    {
        public int[] array;
    }
    #endregion

    #region Public Properties
    public int TotalRequestableResources => totalRequestableResources;
    public int InitialRequests => initialRequests;
    public int SubsequentRequests => subsequentRequests;
    public int MaxRequests
    {
        get
        {
            GameManager instance = GameManager.Instance;

            // If instance is found, use it to detect the current day in the simulation
            // and return the correct number of requests
            if (instance)
            {
                if (instance.CurrentDay == 1) return initialRequests;
                else return subsequentRequests;
            }
            else return -1;
        }
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of quantities requestable for each item")]
    [ParallelItemRegistry("intArrays", "array")]
    private ArrayOfIntArrays quantities;
    [SerializeField]
    [Tooltip("Total amount of resources that can be requested for this level")]
    private int totalRequestableResources;
    [SerializeField]
    [Tooltip("The number of requests that can be made on the first day of the enclosure")]
    private int initialRequests;
    [SerializeField]
    [Tooltip("The number of requests that can be made for the rest of the level")]
    private int subsequentRequests;
    #endregion

    #region Public Methods
    public int RequestableResourcesForItem(ItemID itemID) => quantities.intArrays[(int)itemID.Category].array[itemID.Index];
    #endregion
}
