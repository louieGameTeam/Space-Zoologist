using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A data class containing information about a given set from a level.
[System.Serializable]
public class SetTrace
{
    public enum Result { PASS, FAIL };
    public enum Failure { TIME, MONEY };
    // A string representing the player ID associated with this set trace.
    [SerializeField] private string playerID;
    // An integer representing the level this set belongs to.
    [SerializeField] private int levelID;
    // An integer representing the set this trace belongs to.
    [SerializeField] private int setID;
    // An enumeration representing status of set -- PASS/FAIL.
    [SerializeField] private Result result;
    // An integer representing number of days taken at time of pass/fail.
    [SerializeField] private int numDays;
    // An integer representing player currency at time of pass/fail.
    [SerializeField] private float currency;
    // An enumeration representing reason for set failure, if applicable.
    [SerializeField] private Failure failure;
    // An integer representing the score received on the report back for food related questions.
    [SerializeField] private int foodScore;
    // An integer representing the score received on the report back for terrain related questions.
    [SerializeField] private int terrainScore;

    // PUBLIC GETTERS / SETTERS
    public string PlayerID
    {
        get { return playerID; }
        set { playerID = value; }
    }

    public int LevelID
    {
        get { return levelID; }
        set { levelID = value; }
    }

    public int SetID
    {
        get { return setID; }
        set { setID = value; }
    }

    public Result ResultEnum
    {
        get { return result; }
        set { result = value; }
    }

    public int NumDays
    {
        get { return numDays; }
        set { numDays = value; }
    }

    public float Currency
    {
        get { return currency; }
        set { currency = value; }
    }

    public Failure FailureEnum
    {
        get { return failure; }
        set { failure = value; }
    }

    public int FoodScore
    {
        get { return foodScore; }
        set { foodScore = value; }
    }

    public int TerrainScore
    {
        get { return terrainScore; }
        set { terrainScore = value; }
    }
}
