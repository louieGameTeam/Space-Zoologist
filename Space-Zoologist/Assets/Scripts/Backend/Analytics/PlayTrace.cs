using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A data class containing attributes that make up the telemetry of a play session.
[System.Serializable]
public class PlayTrace
{
    // BASIC INFORMATION
    // The player's ID
    private string playerID;
    // The play session ID
    private string sessionID;
    
    // TIME INFORMATION
    // The total time elapsed throughout the play session
    private float totalElapsedTime;
    // The time spent in level 1
    private float level1ElapsedTime;

    // PROGRESSION INFORMATION
    // A boolean indicating completion of the tutorial
    private bool tutorialComplete;
    // A boolean indicating completion of level 1
    private bool level1Complete;

    // Click information
}
