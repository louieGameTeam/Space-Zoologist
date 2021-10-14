using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReviewedResourceRequest
{
    #region Public Typedefs
    public enum Status
    {
        Granted, PartiallyGranted, Denied
    }
    #endregion

    #region Public Properties
    public ResourceRequest Request { get; private set; }
    public int QuantityGranted { get; private set; }
    // The total cost of buying this review
    public float TotalCost
    {
        get
        {
            if (GameManager.Instance)
            {
                Item itemObject = GameManager.Instance.LevelData.GetItemWithID(Request.ItemRequested).itemObject;
                return itemObject.Price * QuantityGranted;
            }
            else return 0f;
        }
    }
    // Reason for the current status of the review
    public string StatusReason
    {
        get
        {
            switch(CurrentStatus)
            {
                case Status.Granted: return "Funds sufficient to grant request";
                case Status.PartiallyGranted: return "Insufficient funds to grant all items requested";
                case Status.Denied: return "Insufficient funds to grant request";
                default: return $"Unexpected status: {CurrentStatus}";
            }
        }
    }
    // Current status of the review
    public Status CurrentStatus
    {
        get
        {
            if (QuantityGranted >= Request.QuantityRequested) return Status.Granted;
            else if (QuantityGranted > 0) return Status.PartiallyGranted;
            else return Status.Denied;
        }
    }
    #endregion

    #region Factory Methods
    public static ReviewedResourceRequest Review(ResourceRequest request)
    {
        // Create a new review for this request
        ReviewedResourceRequest review = new ReviewedResourceRequest()
        {
            Request = request
        };

        if(GameManager.Instance)
        {
            // Get the item object with the given id
            Item itemObject = GameManager.Instance.LevelData.GetItemWithID(request.ItemRequested).itemObject;
            // Compute the total price
            float totalPrice = itemObject.Price * request.QuantityRequested;

            // If the balance exceeds the total price, grant the item
            if (totalPrice <= GameManager.Instance.Balance)
            {
                review.QuantityGranted = request.QuantityRequested;
            }
            // If the balance is less than the total price but more than the price for one object,
            // grant only the amount you can actually buy
            else if (itemObject.Price <= GameManager.Instance.Balance)
            {
                review.QuantityGranted = (int)(GameManager.Instance.Balance / itemObject.Price);
            }
            // If there is not enough money for any item, don't grant any
            else review.QuantityGranted = 0;
        }

        return review;
    }
    #endregion
}
