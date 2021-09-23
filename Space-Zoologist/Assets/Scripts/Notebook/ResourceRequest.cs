using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player's request for additional resources
/// they can use to sustain the enclosure
/// </summary>
[System.Serializable]
public class ResourceRequest
{
    #region Typedefs
    public enum Status
    {
        NotReviewed, Granted, Denied
    }
    #endregion

    #region Public Properties
    public int Priority
    {
        get => priority;
        set => priority = value;
    }
    public ItemID Target
    {
        get => target;
        set => target = value;
    }
    public NeedType ImprovedNeed
    {
        get => improvedNeed;
        set => improvedNeed = value;
    }
    public int QuantityRequested
    {
        get => quantityRequested;
        set => quantityRequested = value;
    }
    public ItemID ItemRequested
    {
        get => itemRequested;
        set => itemRequested = value;
    }
    public Status CurrentStatus => currentStatus;
    public string StatusReason => statusReason;
    public int QuantityGranted => quantityGranted;
    public ItemID ItemGranted => itemGranted;
    public bool FullyGranted => currentStatus == Status.Granted &&
        quantityRequested == quantityGranted &&
        itemRequested == itemGranted;
    public bool PartiallyGranted => currentStatus == Status.Granted && !FullyGranted;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Priority of the request relative to other requests being made")]
    private int priority;
    [SerializeField]
    [Tooltip("Target of the resource request")]
    private ItemID target;
    [SerializeField]
    [Tooltip("Need of the target that this resource request is supposed to improve")]
    private NeedType improvedNeed;
    [SerializeField]
    [Tooltip("Quantity of the resource requested")]
    private int quantityRequested;
    [SerializeField]
    [Tooltip("The item that is requested")]
    private ItemID itemRequested;
    [SerializeField]
    [Tooltip("Current status of the resource request")]
    private Status currentStatus;
    [SerializeField]
    [Tooltip("Written reason why the request was given the status it currently has")]
    private string statusReason;
    [SerializeField]
    [Tooltip("Amount of items that was actually granted the request")]
    private int quantityGranted;
    [SerializeField]
    [Tooltip("Item actually granted based on the request")]
    private ItemID itemGranted;
    #endregion

    #region Public Methods
    public void Grant()
    {
        Review(Status.Granted, "", quantityRequested, itemRequested);
    }
    public void GrantPartially(string statusReason, int quantityGranted, ItemID itemGranted)
    {
        Review(Status.Granted, statusReason, quantityGranted, itemGranted);
    }
    public void Deny(string statusReason)
    {
        Review(Status.Denied, statusReason, 0, ItemID.Invalid);
    }
    #endregion

    #region Private Methods
    private void Review(Status currentStatus, string statusReason, int quantityGranted, ItemID itemGranted)
    {
        this.currentStatus = currentStatus;
        this.statusReason = statusReason;
        this.quantityGranted = quantityGranted;
        this.itemGranted = itemGranted;

        // If the request was granted then we need to tell the resource manager
        if(currentStatus == Status.Granted)
        {
            GameManager instance = GameManager.Instance;

            // If the game manager exists then use it to get the resource manager and add the item requested
            if(instance)
            {
                LevelData.ItemData itemObjectGranted = instance.LevelData.ItemQuantities.Find(i => i.itemObject.ItemID == itemGranted);

                if (itemObjectGranted != null)
                {
                    instance.m_resourceManager.AddItem(itemObjectGranted.itemObject.ItemName, quantityGranted);
                }
                else Debug.Log("ResourceRequest: the item '" + itemGranted.Data.Name.ToString() +
                    "' could not be granted because no item object with the given ID was found");
            }
            else Debug.Log("ResourceRequest: granted request will not go through to the resource manager " +
                "because no instance of the GameManager could be found");
        }
    }
    #endregion
}
