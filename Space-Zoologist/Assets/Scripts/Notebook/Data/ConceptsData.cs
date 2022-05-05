using System.Linq;
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
        public int attempt;
        public ReviewedResourceRequestList reviews;
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
    public void OnLevelEncountered(LevelID level)
    {
        // Get all entries with the given level
        Entry latestEntry = GetEntryWithLatestAttempt(level);

        if (latestEntry != null)
        {
            // Add an entry for the next attempt to the list of entries
            entries.Add(new Entry
            {
                level = level,
                attempt = latestEntry.attempt + 1,
                reviews = new ReviewedResourceRequestList()
            });
        }
        // If no entries were found with this level, then we add the first one
        else
        {
            entries.Add(new Entry
            {
                level = level,
                attempt = 1,
                reviews = new ReviewedResourceRequestList()
            });
        }
    }

    /// <summary>
    /// Subtract the cost of a review from the current balance 
    /// and add the review to the list of reviews for this level
    /// </summary>
    /// <param name="levelID"></param>
    /// <param name="review"></param>
    public void ConfirmReviwedResourceRequest(LevelID levelID, ReviewedResourceRequest review)
    {
        ReviewedResourceRequestList list = GetEntryWithLatestAttempt(levelID).reviews;

        // Check if the game manager exists and the review was not denied
        if(GameManager.Instance && review.CurrentStatus != ReviewedResourceRequest.Status.Denied)
        {
            // Add the item to the resource manager and subtract the total cost
            Item itemObjectGranted = GameManager.Instance.LevelData.GetItemWithID(review.Request.ItemRequested).itemObject;
            GameManager.Instance.m_resourceManager.AddItem(itemObjectGranted.ID, review.QuantityGranted);
            GameManager.Instance.SubtractFromBalance(review.TotalCost);
        }

        list.Reviews.Add(review);
    }
    public IEnumerable<Entry> GetEntriesWithLevel(LevelID level) => entries
        .Where(entry => entry.level == level);
    public Entry GetEntryWithLatestAttempt(LevelID level)
    {
        IEnumerable<Entry> entries = GetEntriesWithLevel(level);

        // If some entries exist then search for the latest one
        if (entries.Count() > 0)
        {
            Entry maxEntry = null;

            foreach (Entry entry in entries)
            {
                // If max entry is not yet set then make it this entry
                if (maxEntry == null)
                {
                    maxEntry = entry;
                }
                // If this entry was attempted later than the max entry,
                // make the max entry this entry
                else if (entry.attempt > maxEntry.attempt)
                {
                    maxEntry = entry;
                }
            }

            return maxEntry;
        }
        // If no entries exist then return null
        else return null;
    }
    #endregion
}
