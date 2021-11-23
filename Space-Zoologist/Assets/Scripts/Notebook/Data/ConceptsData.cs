using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConceptsData : NotebookDataModule
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of the level ids that are unlocked")]
    private List<LevelID> levels = new List<LevelID>();
    [SerializeField]
    [Tooltip("List of reviewed resource requests, parallel to the list of levels")]
    private List<ReviewedResourceRequestList> resourceRequests = new List<ReviewedResourceRequestList>();
    #endregion

    #region Constructors
    public ConceptsData(NotebookConfig config) : base(config) { }
    #endregion

    #region Public Methods
    public void TryAddEnclosureId(LevelID id)
    {
        if(!levels.Contains(id))
        {
            levels.Add(id);
            resourceRequests.Add(new ReviewedResourceRequestList());
        }
    }
    public ReviewedResourceRequestList GetReviewedResourceRequestList(LevelID id)
    {
        int index = levels.IndexOf(id);

        // Check to make sure the id exists in the list
        if (index >= 0)
        {
            // Check to make sure the index is within range of the requests
            if (index < resourceRequests.Count)
            {
                return resourceRequests[index];
            }
            else throw new System.IndexOutOfRangeException($"{nameof(ConceptsData)}: " +
                $"no resource request list corresponds to level {id}");
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ConceptsData)}: " +
            $"level with id {id} has not been encountered yet");
    }

    /// <summary>
    /// Subtract the cost of a review from the current balance 
    /// and add the review to the list of reviews for this level
    /// </summary>
    /// <param name="levelID"></param>
    /// <param name="review"></param>
    public void ConfirmReviwedResourceRequest(LevelID levelID, ReviewedResourceRequest review)
    {
        ReviewedResourceRequestList list = GetReviewedResourceRequestList(levelID);

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
