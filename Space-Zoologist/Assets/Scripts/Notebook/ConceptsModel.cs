using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConceptsModel
{
    #region Private Fields
    // Map the resource requests to the enclosure it applies to
    private Dictionary<LevelID, ResourceRequestList> resourceRequests = new Dictionary<LevelID, ResourceRequestList>();
    #endregion

    #region Public Methods
    public void TryAddEnclosureId(LevelID id)
    {
        if (!resourceRequests.ContainsKey(id)) resourceRequests.Add(id, new ResourceRequestList());
    }
    public ResourceRequestList GetResourceRequestList(LevelID id) => resourceRequests[id];
    public void ReviewResourceRequests()
    {
        if(GameManager.Instance)
        {
            // Get the list of requests for the current enclosure id
            LevelID current = LevelID.FromCurrentSceneName();
            List<ResourceRequest> toReview = resourceRequests[current].RequestsWithStatus(ResourceRequest.Status.NotReviewed);
            // Sort the requests from highest to lowest priority
            toReview.Sort((x, y) => y.Priority.CompareTo(x.Priority));

            // Loop through all resources and review them
            foreach (ResourceRequest request in toReview)
            {
                // Get the item object with the given id
                Item itemObject = GameManager.Instance.LevelData.GetItemWithID(request.ItemRequested).itemObject;
                // Compute the total price
                float totalPrice = itemObject.Price * request.QuantityRequested;

                // If the balance exceeds the total price, grant the item
                if (totalPrice <= GameManager.Instance.Balance)
                {
                    request.Grant();
                }
                // If the balance is less than the total price but more than the price for one object,
                // grant only the amount you can actually buy
                else if (itemObject.Price <= GameManager.Instance.Balance)
                {
                    int quantityGranted = (int)(GameManager.Instance.Balance / itemObject.Price);
                    request.GrantPartially("Insufficent funds to buy all of the items requested", quantityGranted);
                }
                // If there is not enough money for any item, deny the request
                else request.Deny("Insufficient funds to buy any of the items requested");
            }
        }
        
    }
    #endregion
}
