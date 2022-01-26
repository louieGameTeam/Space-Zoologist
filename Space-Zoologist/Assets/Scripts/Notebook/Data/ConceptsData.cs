using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConceptsData : NotebookDataModule
{
    #region Public Typedefs
    [System.Serializable]
    public class Entry
    {
        public LevelID level;
        public ReviewedResourceRequestList requests;
    }
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("List of the level ids that are unlocked")]
    private List<Entry> entries = new List<Entry>();
    #endregion

    #region Constructors
    public ConceptsData(NotebookConfig config) : base(config) { }
    #endregion

    #region Public Methods
    public void TryAddEnclosureId(LevelID level)
    {
        int index = entries.FindIndex(entry => entry.level == level);

        if(index < 0)
        {
            entries.Add(new Entry 
            { 
                level = level, 
                requests = new ReviewedResourceRequestList() 
            });
        }
    }
    public ReviewedResourceRequestList GetReviewedResourceRequestList(LevelID level)
    {
        int index = entries.FindIndex(entry => entry.level == level);

        // Check to make sure the id exists in the list
        if (index >= 0)
        {
            return entries[index].requests;
        }
        else throw new System.IndexOutOfRangeException($"{nameof(ConceptsData)}: " +
            $"level with id {level} has not been encountered yet");
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
