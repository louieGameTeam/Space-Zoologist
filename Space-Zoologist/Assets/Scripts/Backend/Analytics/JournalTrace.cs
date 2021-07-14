using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A data class containing information about an instance of the player opening the journal.
[System.Serializable]
public class JournalTrace
{
    // The timestamp (in elapsed seconds) of this journal opening.
    private float journalStartTime;
    // The timestamp (in elapsed seconds) of this journal closing.
    private float journalEndTime;
    // The amount of time this journal usage lasted in seconds.
    private float journalDeltaTime;
    // A string representing the notes the player took down in the journal (if any).
    private string journalNotes;

    // PUBLIC GETTERS/SETTERS
    public float JournalStartTime
    {
        get { return journalStartTime; }
        set { journalStartTime = value; }
    }

    public float JournalEndTime
    {
        get { return journalEndTime; }
        set { journalEndTime = value; }
    }

    public float JournalDeltaTime
    {
        get { return journalDeltaTime; }
        set { journalDeltaTime = value; }
    }

    public string JournalNotes
    {
        get { return journalNotes; }
        set { journalNotes = value; }
    }
}   
