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

    // GENERAL TIME METRICS
    // The total time played for this player.
    [SerializeField] private float totalPlayTime;
    // The time spent playing the tutorial.
    [SerializeField] private float tutorialTime;
    // The time spent playing level 1.
    [SerializeField] private float level1Time;
    // The time spent playing level 2.
    [SerializeField] private float level2Time;
    // The time spent playing level 3.
    [SerializeField] private float level3Time;
    // The time spent playing level 4.
    [SerializeField] private float level4Time;
    // The time spent playing level 5.
    [SerializeField] private float level5Time;

    // PROGRESSION METRICS
    // A boolean indicating completion of the entire game.
    [SerializeField] private bool totalCompletion;
    // A boolean indicating completion of the tutorial.
    [SerializeField] private bool tutorialComplete;
    // A boolean indicating completion of level 1.
    [SerializeField] private bool level1Complete;
    // A boolean indicating completion of level 2.
    [SerializeField] private bool level2Complete;
    // A boolean indicating completion of level 3.
    [SerializeField] private bool level3Complete;
    // A boolean indicating completion of level 4.
    [SerializeField] private bool level4Complete;
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
    // A list of SetTraces for level 1.
    [SerializeField] private List<SetTrace> setTracesLevel1;
    // A list of SetTraces for level 2.
    [SerializeField] private List<SetTrace> setTracesLevel2;
    // A list of SetTraces for level 3.
    [SerializeField] private List<SetTrace> setTracesLevel3;
    // A list of SetTraces for level 4.
    [SerializeField] private List<SetTrace> setTracesLevel4;
    // A list of SetTraces for level 5.
    [SerializeField] private List<SetTrace> setTracesLevel5;
}
