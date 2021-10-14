using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConceptsModel
{
    #region Private Fields
    // Map the resource requests to the enclosure it applies to
    private Dictionary<LevelID, ReviewedResourceRequestList> resourceRequests = new Dictionary<LevelID, ReviewedResourceRequestList>();
    #endregion

    #region Public Methods
    public void TryAddEnclosureId(LevelID id)
    {
        if (!resourceRequests.ContainsKey(id)) resourceRequests.Add(id, new ReviewedResourceRequestList());
    }
    public ReviewedResourceRequestList GetReviewedResourceRequestList(LevelID id) => resourceRequests[id];

    /// <summary>
    /// Subtract the cost of a review from the current balance 
    /// and add the review to the list of reviews for this level
    /// </summary>
    /// <param name="levelID"></param>
    /// <param name="review"></param>
    public void ConfirmReviwedResourceRequest(LevelID levelID, ReviewedResourceRequest review)
    {
        ReviewedResourceRequestList list = resourceRequests[levelID];

        // Check if the game manager exists and the review was not denied
        if(GameManager.Instance && review.CurrentStatus != ReviewedResourceRequest.Status.Denied)
        {
            // Add the item to the resource manager and subtract the total cost
            Item itemObjectGranted = GameManager.Instance.LevelData.GetItemWithID(review.Request.ItemRequested).itemObject;
            GameManager.Instance.m_resourceManager.AddItem(itemObjectGranted.ItemName, review.QuantityGranted);
            GameManager.Instance.SubtractFromBalance(review.TotalCost);
        }

        list.Reviews.Add(review);
    }
    #endregion
}
