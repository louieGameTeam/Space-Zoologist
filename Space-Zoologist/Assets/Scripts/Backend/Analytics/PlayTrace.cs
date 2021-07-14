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
    // Time spent in journal/notebook

    // PROGRESSION INFORMATION
    // A boolean indicating completion of the tutorial
    private bool tutorialComplete;
    // A boolean indicating completion of level 1
    private bool level1Complete;

    // DAY INFORMATION
    // serialized day object saved at end of day
    // how many animals were placed, tiles, food, etc
    // removal of tiles
    // how long particular UIs are on the screen for
    // population data for each species
        // detrimental tiles placed
        // beneficial tiles placed
    // currency information
        // currency at start of day
        // currency spent throughout the day
    // notes that students make inside the journal
    // click information
        // timestamp, flag, target
    
    // IN-GAME ASSESSMENT INFORMATION
    // recorded dialogue answers
    // 

    // Click information
}
