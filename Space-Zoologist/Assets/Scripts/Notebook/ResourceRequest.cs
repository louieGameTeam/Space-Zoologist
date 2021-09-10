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
    public ResearchCategory Target
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
    public Item ItemRequested
    {
        get => itemRequested;
        set => itemRequested = value;
    }
    public Status CurrentStatus => currentStatus;
    public string StatusReason => statusReason;
    public int QuantityGranted => quantityGranted;
    public Item ItemGranted => itemGranted;
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
    private ResearchCategory target;
    [SerializeField]
    [Tooltip("Need of the target that this resource request is supposed to improve")]
    private NeedType improvedNeed;
    [SerializeField]
    [Tooltip("Quantity of the resource requested")]
    private int quantityRequested;
    [SerializeField]
    [Tooltip("The item that is requested")]
    private Item itemRequested;
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
    private Item itemGranted;
    #endregion

    #region Public Methods
    public void Grant()
    {
        Review(Status.Granted, "", quantityRequested, itemRequested);
    }
    public void GrantPartially(string statusReason, int quantityGranted, Item itemGranted)
    {
        Review(Status.Granted, statusReason, quantityGranted, itemGranted);
    }
    public void Deny(string statusReason)
    {
        Review(Status.Denied, statusReason, 0, null);
    }
    #endregion

    #region Private Methods
    private void Review(Status currentStatus, string statusReason, int quantityGranted, Item itemGranted)
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
                instance.m_resourceManager.AddItem(itemGranted.ItemName, quantityGranted);
            }
            else
            {
                Debug.Log("ResourceRequest: granted request will not go through to the resource manager " +
                    "because no instance of the GameManager could be found");
            }
        }
    }
    #endregion
}
