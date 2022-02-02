using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelRating
{
    #region Public Properties
    public LevelID ID => id;
    public int Rating => rating;
    #endregion

    #region Private Editor Fields
    [SerializeField]
    [Tooltip("ID of the level with this rating")]
    private LevelID id;
    [SerializeField]
    [Tooltip("Rating of the level with this id")]
    private int rating;
    #endregion

    #region Constructors
    public LevelRating(LevelID id, int rating)
    {
        this.id = id;
        this.rating = rating;
    }
    #endregion
}
