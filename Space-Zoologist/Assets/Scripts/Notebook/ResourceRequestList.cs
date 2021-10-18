using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceRequestList
{
    #region Public Properties
    public int TotalRequestsGranted => RequestsWithStatus(ResourceRequest.Status.Granted).Count();
    public int TotalResourcesGranted => RequestsWithStatus(ResourceRequest.Status.Granted).Sum(x => x.QuantityGranted);
    public List<ResourceRequest> Requests => requests;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of requests")]
    private List<ResourceRequest> requests = new List<ResourceRequest>();
    #endregion

    #region Public Methods
    public int TotalItemsGranted(ItemID itemID) => requests
        .Where(x => x.CurrentStatus == ResourceRequest.Status.Granted && x.ItemRequested == itemID)
        .Sum(x => x.QuantityGranted);
    public List<ResourceRequest> RequestsWithStatus(ResourceRequest.Status status) => requests
        .Where(x => x.CurrentStatus == status)
        .ToList();
    #endregion
}
