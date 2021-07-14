using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DayTrace
{
    // DAY INFORMATION
    // how many animals were placed, tiles, food, etc
    // removal of tiles
    // how long particular UIs are on the screen for
        // rough journal UI is currently setup; further work on pages needs doing
    // population data for each species
        // detrimental tiles placed
        // beneficial tiles placed
    // click information
        // timestamp, flag, target
    
    // An int representing the day being recorded. E.g., day 1 ID = "1"
    private int dayID;
    // The timestamp (in elapsed seconds) that this day started.
    private float dayStartTime;
    // The timestamp (in elapsed seconds) that this day ended.
    private float dayEndTime;
    // An int representing the currency the player has at the start of the day.
    private int currencyStart;
    // An int representing the currency the player has at the end of the day.
    private int currencyEnd;
    // The total amount of time the player had the journal open throughout the day.
    private float journalTime;
    // A list of JournalTraces detailing information about the player opening/closing the journal.
    private List<JournalTrace> journalTraces;
}
