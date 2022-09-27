using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A data class containing summative information that can be easily aggregated
// across all players.
[System.Serializable]
public class SummaryTrace
{
    // The player ID associated with this summary.
    [SerializeField] private string playerID;
    [SerializeField] private string playerName;

    // GENERAL TIME METRICS
    // The total time played for this player.
    [SerializeField] private float totalPlayTime;
    // The time spent playing the tutorial.
    [SerializeField] private float tutorialTime;
    // The time spent playing level 1.
    [SerializeField] private float level1Time;
    // The time spent playing level 1 enclosure 1.
    [SerializeField] private float l1Enclosure1Time;
    [SerializeField] private float l1Enclosure2Time;
    [SerializeField] private float l1Enclosure3Time;
    [SerializeField] private float l1Enclosure4Time;
    // The time spent playing level 2.
    [SerializeField] private float level2Time;
    [SerializeField] private float l2Enclosure1Time;
    [SerializeField] private float l2Enclosure2Time;
    [SerializeField] private float l2Enclosure3Time;
    [SerializeField] private float l2Enclosure4Time;
    // The time spent playing level 3.
    [SerializeField] private float level3Time;
    [SerializeField] private float l3Enclosure1Time;
    [SerializeField] private float l3Enclosure2Time;
    [SerializeField] private float l3Enclosure3Time;
    [SerializeField] private float l3Enclosure4Time;
    // The time spent playing level 4.
    [SerializeField] private float level4Time;
    [SerializeField] private float l4Enclosure1Time;
    [SerializeField] private float l4Enclosure2Time;
    [SerializeField] private float l4Enclosure3Time;
    [SerializeField] private float l4Enclosure4Time;
    // The time spent playing level 5.
    [SerializeField] private float level5Time;

    // PROGRESSION METRICS
    // A boolean indicating completion of the entire game.
    [SerializeField] private bool totalCompletion;
    // A boolean indicating completion of the tutorial.
    [SerializeField] private bool tutorialComplete;
    // A boolean indicating completion of level 1.
    [SerializeField] private bool level1Complete;
    // A boolean indicating completion of level 1 enclosure 1.
    [SerializeField] private bool l1Enclosure1Complete;
    [SerializeField] private bool l1Enclosure2Complete;
    [SerializeField] private bool l1Enclosure3Complete;
    [SerializeField] private bool l1Enclosure4Complete;
    // A boolean indicating completion of level 2.
    [SerializeField] private bool level2Complete;
    [SerializeField] private bool l2Enclosure1Complete;
    [SerializeField] private bool l2Enclosure2Complete;
    [SerializeField] private bool l2Enclosure3Complete;
    [SerializeField] private bool l2Enclosure4Complete;
    // A boolean indicating completion of level 3.
    [SerializeField] private bool level3Complete;
    [SerializeField] private bool l3Enclosure1Complete;
    [SerializeField] private bool l3Enclosure2Complete;
    [SerializeField] private bool l3Enclosure3Complete;
    [SerializeField] private bool l3Enclosure4Complete;
    // A boolean indicating completion of level 4.
    [SerializeField] private bool level4Complete;
    [SerializeField] private bool l4Enclosure1Complete;
    [SerializeField] private bool l4Enclosure2Complete;
    [SerializeField] private bool l4Enclosure3Complete;
    [SerializeField] private bool l4Enclosure4Complete;
    // A boolean indicating completion of level 5.
    [SerializeField] private bool level5Complete;

    // RESEARCH METRICS
    // An integer representing number of times the research tab was opened.
    [SerializeField] private int numResearchTabOpen;
    // A float representing elapsed time while having research tab open.
    [SerializeField] private float timeResearchTabOpen;
    // An integer representing number of articles read in research tab.
    [SerializeField] private int numArticlesRead;
    // An integer representing number of bookmarks created.
    [SerializeField] private int numBookmarksCreated;
    // A string representing a snapshot of all notes taken in research tab.
    [SerializeField] private string notesResearchTab;

    // OBSERVATION METRICS
    // An integer representing number of times the observation tool was used.
    [SerializeField] private int numObservationToolOpen;
    // A float representing elapsed time while having observation tool open.
    [SerializeField] private float timeObservationToolOpen;
    // A string representing a snapshot of all notes taken in observation mode.
    [SerializeField] private string notesObservationTool;

    // CONCEPT METRICS
    // An integer representing number of times player requested resources.
    [SerializeField] private int numResourceRequests;
    // An integer representing number of times resource requests were approved.
    [SerializeField] private int numResourceRequestsApproved;
    // An integer representing number of times resource requests were denied.
    [SerializeField] private int numResourceRequestsDenied;
    // An integer representing number of times player used the draw tool.
    [SerializeField] private int numDrawToolUsed;

    // TESTING METRICS -- see SetTrace.cs
    // A list of SetTraces.
    [SerializeField] private List<SetTrace> setTraces;

    // Default constructor.
    public SummaryTrace()
    {
        playerID = "";
        playerName = "";
        totalPlayTime = 0f;
        tutorialTime = 0f;
        level1Time = 0f;
        l1Enclosure1Time = 0f;
        l1Enclosure2Time = 0f;
        l1Enclosure3Time = 0f;
        l1Enclosure4Time = 0f;
        level2Time = 0f;
        l2Enclosure1Time = 0f;
        l2Enclosure2Time = 0f;
        l2Enclosure3Time = 0f;
        l2Enclosure4Time = 0f;
        level3Time = 0f;
        l3Enclosure1Time = 0f;
        l3Enclosure2Time = 0f;
        l3Enclosure3Time = 0f;
        l3Enclosure4Time = 0f;
        level4Time = 0f;
        l4Enclosure1Time = 0f;
        l4Enclosure2Time = 0f;
        l4Enclosure3Time = 0f;
        l4Enclosure4Time = 0f;
        level5Time = 0f;
        totalCompletion = false;
        tutorialComplete = false;
        level1Complete = false;
        l1Enclosure1Complete = false;
        l1Enclosure2Complete = false;
        l1Enclosure3Complete = false;
        l1Enclosure4Complete = false;
        level2Complete = false;
        l2Enclosure1Complete = false;
        l2Enclosure2Complete = false;
        l2Enclosure3Complete = false;
        l2Enclosure4Complete = false;
        level3Complete = false;
        l3Enclosure1Complete = false;
        l3Enclosure2Complete = false;
        l3Enclosure3Complete = false;
        l3Enclosure4Complete = false;
        level4Complete = false;
        l4Enclosure1Complete = false;
        l4Enclosure2Complete = false;
        l4Enclosure3Complete = false;
        l4Enclosure4Complete = false;
        level5Complete = false;
        numResearchTabOpen = 0;
        timeResearchTabOpen = 0f;
        numArticlesRead = 0;
        numBookmarksCreated = 0;
        notesResearchTab = "";
        numObservationToolOpen = 0;
        timeObservationToolOpen = 0f;
        notesObservationTool = "";
        numResourceRequests = 0;
        numResourceRequestsApproved = 0;
        numResourceRequestsDenied = 0;
        numDrawToolUsed = 0;
        setTraces = new List<SetTrace>();
    }

    // PUBLIC GETTERS / SETTERS
    public string PlayerID
    {
        get { return playerID; }
        set { playerID = value; }
    }

    public string PlayerName
    {
        get { return playerName; }
        set { playerName = value; }
    }

    public float TotalPlayTime
    {
        get { return totalPlayTime; }
        set { totalPlayTime = value; }
    }

    public float TutorialTime
    {
        get { return tutorialTime; }
        set { tutorialTime = value; }
    }

    public float Level1Time
    {
        get { return level1Time; }
        set { level1Time = value; }
    }

    public float L1Enclosure1Time
    {
        get { return l1Enclosure1Time; }
        set { l1Enclosure1Time = value; }
    }
    public float L1Enclosure2Time
    {
        get { return l1Enclosure2Time; }
        set { l1Enclosure2Time = value; }
    }
    public float L1Enclosure3Time
    {
        get { return l1Enclosure3Time; }
        set { l1Enclosure3Time = value; }
    }
    public float L1Enclosure4Time
    {
        get { return l1Enclosure4Time; }
        set { l1Enclosure4Time = value; }
    }

    public float Level2Time
    {
        get { return level2Time; }
        set { level2Time = value; }
    }
    public float L2Enclosure1Time
    {
        get { return l2Enclosure1Time; }
        set { l2Enclosure1Time = value; }
    }
    public float L2Enclosure2Time
    {
        get { return l2Enclosure2Time; }
        set { l2Enclosure2Time = value; }
    }
    public float L2Enclosure3Time
    {
        get { return l2Enclosure3Time; }
        set { l2Enclosure3Time = value; }
    }
    public float L2Enclosure4Time
    {
        get { return l2Enclosure4Time; }
        set { l2Enclosure4Time = value; }
    }

    public float Level3Time
    {
        get { return level3Time; }
        set { level3Time = value; }
    }
    public float L3Enclosure1Time
    {
        get { return l3Enclosure1Time; }
        set { l3Enclosure1Time = value; }
    }
    public float L3Enclosure2Time
    {
        get { return l3Enclosure2Time; }
        set { l3Enclosure2Time = value; }
    }
    public float L3Enclosure3Time
    {
        get { return l3Enclosure3Time; }
        set { l3Enclosure3Time = value; }
    }
    public float L3Enclosure4Time
    {
        get { return l3Enclosure4Time; }
        set { l3Enclosure4Time = value; }
    }

    public float Level4Time
    {
        get { return level4Time; }
        set { level4Time = value; }
    }
    public float L4Enclosure1Time
    {
        get { return l4Enclosure1Time; }
        set { l4Enclosure1Time = value; }
    }
    public float L4Enclosure2Time
    {
        get { return l4Enclosure2Time; }
        set { l4Enclosure2Time = value; }
    }
    public float L4Enclosure3Time
    {
        get { return l4Enclosure3Time; }
        set { l4Enclosure3Time = value; }
    }
    public float L4Enclosure4Time
    {
        get { return l4Enclosure4Time; }
        set { l4Enclosure4Time = value; }
    }

    public float Level5Time
    {
        get { return level5Time; }
        set { level5Time = value; }
    }

    public bool TotalCompletion
    {
        get { return totalCompletion; }
        set { totalCompletion = value; }
    }

    public bool TutorialComplete
    {
        get { return tutorialComplete; }
        set { tutorialComplete = value; }
    }

    public bool Level1Complete
    {
        get { return level1Complete; }
        set { level1Complete = value; }
    }

    public bool L1Enclosure1Complete
    {
        get { return l1Enclosure1Complete; }
        set { l1Enclosure1Complete = value; }
    }
    public bool L1Enclosure2Complete
    {
        get { return l1Enclosure2Complete; }
        set { l1Enclosure2Complete = value; }
    }
    public bool L1Enclosure3Complete
    {
        get { return l1Enclosure3Complete; }
        set { l1Enclosure3Complete = value; }
    }
    public bool L1Enclosure4Complete
    {
        get { return l1Enclosure4Complete; }
        set { l1Enclosure4Complete = value; }
    }

    public bool Level2Complete
    {
        get { return level2Complete; }
        set { level2Complete = value; }
    }
    public bool L2Enclosure1Complete
    {
        get { return l2Enclosure1Complete; }
        set { l2Enclosure1Complete = value; }
    }
    public bool L2Enclosure2Complete
    {
        get { return l2Enclosure2Complete; }
        set { l2Enclosure2Complete = value; }
    }
    public bool L2Enclosure3Complete
    {
        get { return l2Enclosure3Complete; }
        set { l2Enclosure3Complete = value; }
    }
    public bool L2Enclosure4Complete
    {
        get { return l2Enclosure4Complete; }
        set { l2Enclosure4Complete = value; }
    }

    public bool Level3Complete
    {
        get { return level3Complete; }
        set { level3Complete = value; }
    }
    public bool L3Enclosure1Complete
    {
        get { return l3Enclosure1Complete; }
        set { l3Enclosure1Complete = value; }
    }
    public bool L3Enclosure2Complete
    {
        get { return l3Enclosure2Complete; }
        set { l3Enclosure2Complete = value; }
    }
    public bool L3Enclosure3Complete
    {
        get { return l3Enclosure3Complete; }
        set { l3Enclosure3Complete = value; }
    }
    public bool L3Enclosure4Complete
    {
        get { return l3Enclosure4Complete; }
        set { l3Enclosure4Complete = value; }
    }

    public bool Level4Complete
    {
        get { return level4Complete; }
        set { level4Complete = value; }
    }
    public bool L4Enclosure1Complete
    {
        get { return l4Enclosure1Complete; }
        set { l4Enclosure1Complete = value; }
    }
    public bool L4Enclosure2Complete
    {
        get { return l4Enclosure2Complete; }
        set { l4Enclosure2Complete = value; }
    }
    public bool L4Enclosure3Complete
    {
        get { return l4Enclosure3Complete; }
        set { l4Enclosure3Complete = value; }
    }
    public bool L4Enclosure4Complete
    {
        get { return l4Enclosure4Complete; }
        set { l4Enclosure4Complete = value; }
    }

    public bool Level5Complete
    {
        get { return level5Complete; }
        set { level5Complete = value; }
    }

    public int NumResearchTabOpen
    {
        get { return numResearchTabOpen; }
        set { numResearchTabOpen = value; }
    }

    public float TimeResearchTabOpen
    {
        get { return timeResearchTabOpen; }
        set { timeResearchTabOpen = value; }
    }

    public int NumArticlesRead
    {
        get { return numArticlesRead; }
        set { numArticlesRead = value; }
    }

    public int NumBookmarksCreated
    {
        get { return numBookmarksCreated; }
        set { numBookmarksCreated = value; }
    }

    public string NotesResearchTab
    {
        get { return notesResearchTab; }
        set { notesResearchTab = value; }
    }

    public int NumObservationToolOpen
    {
        get { return numObservationToolOpen; }
        set { numObservationToolOpen = value; }
    }

    public float TimeObservationToolOpen
    {
        get { return timeObservationToolOpen; }
        set { timeObservationToolOpen = value; }
    }

    public string NotesObservationTool
    {
        get { return notesObservationTool; }
        set { notesObservationTool = value; }
    }

    public int NumResourceRequests
    {
        get { return numResourceRequests; }
        set { numResourceRequests = value; }
    }

    public int NumResourceRequestsApproved
    {
        get { return numResourceRequestsApproved; }
        set { numResourceRequestsApproved = value; }
    }

    public int NumResourceRequestsDenied
    {
        get { return numResourceRequestsDenied; }
        set { numResourceRequestsDenied = value; }
    }

    public int NumDrawToolUsed
    {
        get { return numDrawToolUsed; }
        set { numDrawToolUsed = value; }
    }

    public List<SetTrace> SetTraces
    {
        get { return setTraces; }
        set { setTraces = value; }
    }
}
