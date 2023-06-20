using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelRatingUI : MonoBehaviour
{
    #region Private Editor Fields
    [SerializeField]
    [Tooltip("Flavor text describing the rating of this level")]
    private TextMeshProUGUI ratingText = null;
    [SerializeField]
    [Tooltip("Game object prefab to instantiate to represent an achieved score point")]
    private GameObject ratingObjectFilled = null;
    [SerializeField]
    [Tooltip("Game object prefab to instantiate to represent an unearned score point")]
    private GameObject ratingObjectEmpty = null;
    [SerializeField]
    [Tooltip("Parent to instantiate the rating objects into")]
    private Transform ratingObjectParent = null;
    #endregion

    #region Public Methods
    public void Setup(LevelData enclosure)
    {
        // Get the id for this level
        LevelID current = LevelID.FromSceneName(enclosure.Level.SceneName);

        if (current < SaveData.LatestLevelQualified)
        {
            // Get the rating for this level
            int rating = SaveData.GetLevelRating(current);

            // Only display rating if this level has a rating
            // or if it is one whole level before the latest level qualified
            if (rating >= 0 || current.LevelNumber < SaveData.LatestLevelQualified.LevelNumber)
            {
                // Setup the rating text and rating objects
                // ratingText.text = LevelRatingSystem.GetRatingText(rating);

                // Create a rating object for each rating level
                for (int i = 0; i <= rating; i++)
                {
                    Instantiate(ratingObjectFilled, ratingObjectParent);
                }

                for (int i = rating; i < LevelRatingSystem.MaxRating; i++)
                {
                    Instantiate(ratingObjectEmpty, ratingObjectParent);

                }

                // Make this object enabled
                gameObject.SetActive(true);
            }
            // If this has no rating and it is the same level as the latest level qualified,
            // we do not want to see the rating
            else gameObject.SetActive(false);
        }
        // Do not display ratings for levels we are not qualified to complete
        else gameObject.SetActive(false);
    }

    public void Disable () {
        gameObject.SetActive (false);
    }

    #endregion
}
