using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A data class containing attributes that make up the telemetry of a play session.
[System.Serializable]
public class PlayTrace
{
    // BASIC INFORMATION
    // The player's ID.
    [SerializeField] private string playerID;
    // The play session ID.
    [SerializeField] private string sessionID;
    
    // TIME INFORMATION
    // The total time elapsed throughout the play session.
    [SerializeField] private float sessionElapsedTime;

    // LEVEL INFORMATION
    // A list of LevelTrace objects containing detailed information about the actions the player took in levels.
    [SerializeField] private List<LevelTrace> levelTraces;

    // PUBLIC GETTERS/SETTERS
    public string PlayerID
    {
        get { return playerID; }
        set { playerID = value; }
    }

    public string SessionID
    {
        get { return sessionID; }
        set { sessionID = value; }
    }

    public float SessionElapsedTime
    {
        get { return sessionElapsedTime; }
        set { sessionElapsedTime = value; }
    }

    public List<LevelTrace> LevelTraces
    {
        get { return levelTraces; }
        set { levelTraces = value; }
    }
}
