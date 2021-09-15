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

    // NOTEBOOK/RESEARCH METRICS
    // The total time spent in the notebook for this player.
    [SerializeField] private float totalNotebookTime;
    // The total time spent in the notebook for this player in level 1.
    [SerializeField] private float level1NotebookTime;
    // The total time spent in the notebook for this player in level 2.
    [SerializeField] private float leve2NotebookTime;
    // The total time spent in the notebook for this player in level 3.
    [SerializeField] private float level3NotebookTime;
    // The total time spent in the notebook for this player in level 4.
    [SerializeField] private float level4NotebookTime;
    // The total time spent in the notebook for this player in level 5.
    [SerializeField] private float level5NotebookTime;
    // The total number of additions the player made to the notebook.
    [SerializeField] private int totalNotebookAdditions;
    // The number of additions the player made to the notebook in level 1.
    [SerializeField] private int level1NotebookAdditions;
    // The number of additions the player made to the notebook in level 2.
    [SerializeField] private int level2NotebookAdditions;
    // The number of additions the player made to the notebook in level 3.
    [SerializeField] private int level3NotebookAdditions;
    // The number of additions the player made to the notebook in level 4.
    [SerializeField] private int level4NotebookAdditions;
    // The number of additions the player made to the notebook in level 5.
    [SerializeField] private int level5NotebookAdditions;

    // PERFORMANCE METRICS -- this is where we need some thought.
    // The total number of times the player ran out of resources.
    [SerializeField] private int numResourcesLost;
    // The total number of times the player requested more resources.
    [SerializeField] private int numResourcesRequested;
    // The total number of times any species went into decline.
    [SerializeField] private int numSpeciesDeclined;
    // The total number of times any species thrived.
    [SerializeField] private int numSpeciesThrived;

    // REPORT-BACK METRICS
    // level 1 - answers given
    // level 1 - questions correct/incorrect
    // level 1 - time spent in assessment
    // ...repeated for all levels
}
