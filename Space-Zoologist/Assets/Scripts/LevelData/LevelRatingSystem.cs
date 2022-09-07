using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelRatingSystem
{

    #region Public Fields
    public static readonly string noRatingText = "No rating - enclosure not yet designed";
    // TODO: Rewrite dialogue
    public static readonly string[] ratingText = new string[]
    {
        "Needs redesign. Species’ populations are unstable",
        "Almost there! Some species are self sustaining",
        "Congratulations, all species are self sustaining in this enclosure!"
    };
    #endregion

    #region Public Methods
    public static string GetRatingText(int rating)
    {
        if (rating >= 0 && rating < ratingText.Length)
        {
            return ratingText[rating];
        }
        else return noRatingText;
    }
    public static int RateCurrentLevel()
    {
        GameManager gameManager = GameManager.Instance;

        // If a game manager was found then rate the level it is currently managing
        QuizConversation currentQuiz = GameObject.FindObjectOfType<QuizConversation>();
        if (currentQuiz)
        {
            return RateLevel(currentQuiz.CurrentQuiz.Grade);
        }
        else throw new MissingReferenceException($"{nameof(LevelRatingSystem)}: " +
            $"cannot rate the current level because no quiz could be found " +
            $"in the current scene");
    }
    public static int RateLevel(QuizGrade grade)
    {
        return (int) grade;
    }
    #endregion
}
