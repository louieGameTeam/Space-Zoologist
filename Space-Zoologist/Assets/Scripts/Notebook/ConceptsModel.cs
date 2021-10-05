using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConceptsModel
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Quantity of an item that is requestable in each level")]
    private ResourceRequestQuantityRegistry[] quantityRegistries;
    #endregion

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
    // Maximum number of requests that can be made by the player when playing this enclosure
    public int MaxRequests(LevelID enclosureID)
    {
        if (enclosureID.LevelNumber >= 0 && enclosureID.LevelNumber < quantityRegistries.Length)
        {
            return quantityRegistries[enclosureID.LevelNumber].MaxRequests;
        }
        else return -1;
    }
    // Maximum number of resources that can be requested by the player when playing this enclosure
    public int MaxRequestableResources(LevelID enclosureID)
    {
        if (enclosureID.LevelNumber >= 0 && enclosureID.LevelNumber < quantityRegistries.Length)
        {
            return quantityRegistries[enclosureID.LevelNumber].TotalRequestableResources;
        }
        else return -1;
    }
    public int MaxRequestableResourcesForItem(LevelID enclosureID, ItemID itemID)
    {
        if (enclosureID.LevelNumber >= 0 && enclosureID.LevelNumber < quantityRegistries.Length)
        {
            return quantityRegistries[enclosureID.LevelNumber].RequestableResourcesForItem(itemID);
        }
        else return -1;
    }
    // Count the requests that the player has remaining
    public int RemainingRequests(LevelID enclosureID)
    {
        GameManager instance = GameManager.Instance;

        // If the game manager exists, check which day we are on in the simulation
        if (instance)
        {
            if (instance.CurrentDay == 1) return MaxRequests(enclosureID) - resourceRequests[enclosureID].TotalRequestsGrantedInDayRange(0, 1);
            else return MaxRequests(enclosureID) - resourceRequests[enclosureID].TotalRequestsGrantedInDayRange(2, int.MaxValue);
        }
        else return -1;
    }
    public int RemainingRequestableResources(LevelID enclosureID)
    {
        return MaxRequestableResources(enclosureID) - resourceRequests[enclosureID].TotalResourcesGranted;
    }
    public int RemainingRequestableResourcesForItem(LevelID enclosureID, ItemID itemID)
    {
        return MaxRequestableResourcesForItem(enclosureID, itemID) - resourceRequests[enclosureID].TotalItemsGranted(itemID);
    }
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
                Item itemObject = GameManager.Instance.LevelData.GetItemWithID(request.ItemRequested).itemObject;
                float totalPrice = itemObject.Price * request.QuantityRequested;

                if(GameManager.Instance.Balance <= totalPrice)
                {

                }

                int remainingRequests = RemainingRequests(current);
                int remainingResources = RemainingRequestableResources(current);
                int remainingResourcesForItem = RemainingRequestableResourcesForItem(current, request.ItemRequested);

                // If there are remaining resources, compute quantity to grant
                if (remainingRequests > 0 && remainingResources > 0 && remainingResourcesForItem > 0)
                {
                    int quantityGranted = Mathf.Min(request.QuantityRequested, remainingResources, remainingResourcesForItem);

                    // If quantity granted is the same as quantity requested then we can fully grant the request
                    if (quantityGranted == request.QuantityRequested) request.Grant();
                    else if (quantityGranted == remainingResources) request.GrantPartially("Insufficient resources to fully grant this request", quantityGranted);
                    else request.GrantPartially("Ran out of items in stock to fully grant this request", quantityGranted);
                }
                else if (remainingRequests <= 0) request.Deny("Ran out of requests to grant");
                else if (remainingResources <= 0) request.Deny("Ran out of resources to grant");
                else request.Deny("No more items of this type in stock");
            }
        }
        
    }
    #endregion
}
