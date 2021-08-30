using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConceptsModel
{
    #region Private Fields
    // Map the resource requests to the enclosure it applies to
    private Dictionary<EnclosureID, ResourceRequestList> resourceRequests = new Dictionary<EnclosureID, ResourceRequestList>();
    #endregion

    #region Public Methods
    public void TryAddEnclosureId(EnclosureID id)
    {
        if (!resourceRequests.ContainsKey(id)) resourceRequests.Add(id, new ResourceRequestList());
    }
    public ResourceRequestList GetResourceRequestList(EnclosureID id) => resourceRequests[id];
    // Maximum number of requests that can be made by the player when playing this enclosure
    public int MaxRequests(EnclosureID _) => 10;
    // Maximum number of resources that can be requested by the player when playing this enclosure
    public int MaxResources(EnclosureID _) => 100;
    // Count the requests that the player has remaining
    public int RemainingRequests(EnclosureID id) => MaxRequests(id) - resourceRequests[id].TotalRequestsGranted;
    public int RemainingResources(EnclosureID id) => MaxResources(id) - resourceRequests[id].TotalResourcesGranted;
    public void ReviewResourceRequests()
    {
        // Get the list of requests for the current enclosure id
        EnclosureID current = EnclosureID.FromCurrentSceneName();
        List<ResourceRequest> toReview = resourceRequests[current].RequestsWithStatus(ResourceRequest.Status.NotReviewed);
        // Sort the requests from highest to lowest priority
        toReview.Sort((x, y) => y.Priority.CompareTo(x.Priority));

        // Loop through all resources and review them
        foreach(ResourceRequest request in toReview)
        {
            int remainingResources = RemainingResources(current);

            // If there are remaining resources, compute quantity to grant
            if (remainingResources > 0)
            {
                int quantityGranted = Mathf.Min(request.QuantityRequested, remainingResources);

                // If quantity granted is the same as quantity requested then we can fully grant the request
                if (quantityGranted == request.QuantityRequested) request.Grant();
                // If quantity granted is less than quantity requested then partially grant the request 
                else request.GrantPartially("Insufficient resources to fully grant this request", quantityGranted, request.ItemRequested);
            }
            else request.Deny("Insufficient resources to grant this request");
        }
    }
    #endregion
}
