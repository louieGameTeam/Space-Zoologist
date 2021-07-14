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
    // The amount of time this day lasted in seconds.
    private float dayDeltaTime;
    // An int representing the currency the player has at the start of the day.
    private int currencyStart;
    // An int representing the currency the player has at the end of the day.
    private int currencyEnd;
    // The total amount of time the player had the journal open throughout the day.
    private float journalTime;
    // A list of JournalTraces detailing information about the player opening/closing the journal.
    private List<JournalTrace> journalTraces;

    // PUBLIC GETTERS/SETTERS
    public int DayID
    {
        get { return dayID; }
        set { dayID = value; }
    }

    public float DayStartTime
    {
        get { return dayStartTime; }
        set { dayStartTime = value; }
    }

    public float DayEndTime
    {
        get { return dayEndTime; }
        set { dayEndTime = value; }
    }

    public float DayDeltaTime
    {
        get { return dayDeltaTime; }
        set { dayDeltaTime = value; }
    }

    public int CurrencyStart
    {
        get { return currencyStart; }
        set { currencyStart = value; }
    }

    public int CurrencyEnd
    {
        get { return currencyEnd; }
        set { currencyEnd = value; }
    }

    public float JournalTime
    {
        get { return journalTime; }
        set { journalTime = value; }
    }

    public List<JournalTrace> JournalTraces
    {
        get { return journalTraces; }
        set { journalTraces = value; }
    }
}
