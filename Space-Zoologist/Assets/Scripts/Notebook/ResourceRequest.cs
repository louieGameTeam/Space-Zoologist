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
    public ItemID ItemAddressed
    {
        get => itemAddressed;
        set => itemAddressed = value;
    }
    public NeedType NeedAddressed
    {
        get
        {
            // If the item requested is food, the need type addressed is food
            if (ItemRequested.Category == ItemRegistry.Category.Food) return NeedType.FoodSource;
            // If it's not food we assume it is a tile. Check if it is a water tile
            else if (ItemRequested.Data.Name.Get(ItemName.Type.English).Contains("Water")) return NeedType.Liquid;
            // If it is not water it must be a tile to address terrain needs
            else return NeedType.Terrain;
        }
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
    public bool FullyGranted => currentStatus == Status.Granted &&
        quantityRequested == quantityGranted;
    public bool PartiallyGranted => currentStatus == Status.Granted && !FullyGranted;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Priority of the request relative to other requests being made")]
    private int priority;
    [SerializeField]
    [Tooltip("Item that the player is trying to address by making the resource request")]
    private ItemID itemAddressed;
    [SerializeField]
    [Tooltip("Quantity of the resource requested")]
    private int quantityRequested;
    [SerializeField]
    [Tooltip("The item that is requested")]
    private ItemID itemRequested;
    #endregion

    #region Private Fields
    private Status currentStatus = Status.NotReviewed;
    private string statusReason;
    private int quantityGranted;
    #endregion

    #region Public Methods
    public void Grant()
    {
        Review(Status.Granted, "", quantityRequested);
    }
    public void GrantPartially(string statusReason, int quantityGranted)
    {
        Review(Status.Granted, statusReason, quantityGranted);
    }
    public void Deny(string statusReason)
    {
        Review(Status.Denied, statusReason, 0);
    }
    #endregion

    #region Private Methods
    private void Review(Status currentStatus, string statusReason, int quantityGranted)
    {
        // Multiple reviews not allowed
        if(this.currentStatus == Status.NotReviewed)
        {
            this.currentStatus = currentStatus;
            this.statusReason = statusReason;
            this.quantityGranted = quantityGranted;

            if (GameManager.Instance)
            {
                if (currentStatus == Status.Granted)
                {
                    // Try to find the object granted so that we can update the resource manager
                    LevelData.ItemData itemGranted = GameManager.Instance.LevelData.GetItemWithID(itemRequested);

                    // Check if item could be found in the list of items on the level data
                    if (itemGranted != null)
                    {
                        GameManager.Instance.m_resourceManager.AddItem(itemGranted.itemObject.ItemName, quantityGranted);
                        GameManager.Instance.SubtractFromBalance(itemGranted.itemObject.Price * quantityGranted);
                    }
                    else Debug.LogWarning("ResourceRequest: the item '" + itemRequested.Data.Name.ToString() +
                        "' could not be granted because no item object with the given ID was found");
                }
            }
        }
    }
    #endregion
}
