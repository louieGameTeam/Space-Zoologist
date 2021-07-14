using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A data class containing information about the levels played during this session.
public class LevelTrace
{
    // The ID associated with this level.
    private int levelID;
    // The timestamp (in elapsed seconds) of the beginning of this level.
    private float levelStartTime;
    // The timestamp (in elapsed seconds) of the end of this level.
    private float levelEndTime;
    // The amount of time this level lasted in seconds.
    private float levelDeltaTime;
    // The total number of days passed in this level.
    private int levelElapsedDays;
    // A list of DayTrace objects detailing player actions throughout the day.
    private List<DayTrace> dayTraces;
    // A boolean indicating completion of the level.
    private bool levelComplete;

    // PUBLIC GETTERS/SETTERS
    public int LevelID
    {
        get { return levelID; }
        set { levelID = value; }
    }

    public float LevelStartTime
    {
        get { return levelStartTime; }
        set { levelStartTime = value; }
    }

    public float LevelEndTime
    {
        get { return levelEndTime; }
        set { levelEndTime = value; }
    }

    public float LevelDeltaTime
    {
        get { return levelDeltaTime; }
        set { levelDeltaTime = value; }
    }

    public int LevelElapsedDays
    {
        get { return levelElapsedDays; }
        set { levelElapsedDays = value; }
    }

    public List<DayTrace> DayTraces
    {
        get { return dayTraces; }
        set { dayTraces = value; }
    }

    public bool LevelComplete
    {
        get { return levelComplete; }
        set { levelComplete = value; }
    }
}
