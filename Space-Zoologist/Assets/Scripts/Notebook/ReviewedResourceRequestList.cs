using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReviewedResourceRequestList
{
    #region Public Properties
    public List<ReviewedResourceRequest> Reviews => reviews;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of requests")]
    private List<ReviewedResourceRequest> reviews = new List<ReviewedResourceRequest>();
    #endregion

    #region Public Methods
    public List<ReviewedResourceRequest> RequestsWithStatus(ReviewedResourceRequest.Status status) => reviews
        .Where(x => x.CurrentStatus == status)
        .ToList();
    #endregion
}
