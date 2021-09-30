using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SummaryManager : MonoBehaviour
{
    // Initialize the instance of this SummaryManager to null.
    private static SummaryManager instance = null;
    // Initialize the current SummaryTrace object.
    private SummaryTrace currentSummaryTrace = null;
    // Initialize current level and set.
    private int currentLevel;
    private int currentSet;
    // Boolean values indicating status of notebook tabs.
    private bool researchOpen = false;
    private bool observationOpen = false;
    // Keep track of certain objects in scene globally.
    private LevelDataReference levelData;
    private NotebookTabPicker picker;

    // On Awake, check the status of the instance. If the instance is null, replace it with the current SummaryManager.
    // Else, destroy the gameObject this script is attached to. There can only be one.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Check if current user has summary trace data in DB.
        StartCoroutine(GetSummaryTrace.TryGetSummaryTrace(GetPlayerID(), (value) => {
            SummaryTraceResponse response = value;
            // If the trace was found for the user, set its data field to be the current summary trace.
            if (response.code == 0)
            {
                currentSummaryTrace = response.data;
            // If no trace for the current user is found, create a new summary trace to work from.
            } else if (response.code == 2)
            {
                currentSummaryTrace = new SummaryTrace();
            }
        }));
    }

    // Enable and disable functions to allocate delegates.
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Test submission of summary trace.
        // string json = ConvertSummaryTraceToJSON(CreateSummaryTrace());
        // StartCoroutine(SubmitSummaryTrace.TrySubmitSummaryTrace(json));
    }

    // Update is called once per frame
    void Update()
    {
        // Begin tracking total play time.
        if (currentSummaryTrace != null)
        {
            currentSummaryTrace.TotalPlayTime += Time.deltaTime;
        }

        // Track time in levels. CurrentLevel is determined by OnSceneLoaded and CheckLevelAndSet functions.
        if (currentLevel == 1)
        {
            currentSummaryTrace.Level1Time += Time.deltaTime;
        } else if (currentLevel == 2)
        {
            currentSummaryTrace.Level2Time += Time.deltaTime;
        } else if (currentLevel == 3)
        {
            currentSummaryTrace.Level3Time += Time.deltaTime;
        } else if (currentLevel == 4)
        {
            currentSummaryTrace.Level4Time += Time.deltaTime;
        } else if (currentLevel == 5)
        {
            currentSummaryTrace.Level5Time += Time.deltaTime;
        }

        // Track time in notebook tabs/tools.
        if (researchOpen)
        {
            currentSummaryTrace.TimeResearchTabOpen += Time.deltaTime;
        }
        if (observationOpen)
        {
            currentSummaryTrace.TimeObservationToolOpen += Time.deltaTime;
        }
    }

    // Use scene change functions to determine level being loaded/unloaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the buildIndex of the scene is 3 (i.e., if we are in the MainLevel scene).
        if (scene.buildIndex == 3)
        {
            // Get references to necessary objects.
            levelData = GameObject.Find("LevelData").GetComponent<LevelDataReference>();
            // Subscribe to all relevant events.
            SubscribeToEvents();
            // Check current level and set (enclosure).
            CheckLevelAndSet(levelData);
            // Check progression statistics.
            CheckAndModifyProgression(currentLevel, currentSummaryTrace);
        }
    }

    // Placeholder functions to gather basic information related to player info: playerID and sessionID.
    private string GetPlayerID()
    {
        return "test";
    }

    private string GetSessionID()
    {
        return "test";
    }

    // Creates a new summary trace using default constructor.
    private SummaryTrace CreateSummaryTrace()
    {
        SummaryTrace summaryTrace = new SummaryTrace();
        return summaryTrace;
    }

    // A function that serializes the current SummaryTrace object to JSON.
    private string ConvertSummaryTraceToJSON(SummaryTrace summaryTrace)
    {
        string json = JsonUtility.ToJson(summaryTrace);
        return json;
    }

    // A function that uses the event management system to subscribe to events used in this manager.
    private void SubscribeToEvents()
    {
        // Listen to notebook events being fired.
        EventManager.Instance.SubscribeToEvent(EventType.OnJournalOpened, OnJournalOpened);
    }

    // A function that sets the current level and set (enclosure) based on the levelData object.
    private void CheckLevelAndSet(LevelDataReference levelData)
    {
        string levelName = levelData.LevelData.Level.SceneName;

        // Parse level name for level information.
        if (levelName.Contains("Level1"))
        {
            currentLevel = 1;
        } else if (levelName.Contains("Level2"))
        {
            currentLevel = 2;
        } else if (levelName.Contains("Level3"))
        {
            currentLevel = 3;
        } else if (levelName.Contains("Level4"))
        {
            currentLevel = 4;
        } else if (levelName.Contains("Level5"))
        {
            currentLevel = 5;
        }

        //Parse level name for set information.
        if (levelName.Contains("E1"))
        {
            currentSet = 1;
        } else if (levelName.Contains("E2"))
        {
            currentSet = 2;
        } else if (levelName.Contains("E3"))
        {
            currentSet = 3;
        } else if (levelName.Contains("E4")) 
        {
            currentSet = 4;
        }
    }

    // A hacky function that determines current progression statistics (this will probably need refining).
    // Currently, because not all players will pass through all enclosures to get to the next level,
    // we can't use status of enclosure completion to determine if an entire level was completed.
    // This instead sets completion of a level on the start of another level. This of course
    // leaves level 5 out of completion.
    private void CheckAndModifyProgression(int currentLevel, SummaryTrace trace)
    {
        if (currentLevel == 1)
        {
            trace.TutorialComplete = true;
        }
        if (currentLevel == 2)
        {
            trace.Level1Complete = true;
        }
        if (currentLevel == 3)
        {
            trace.Level2Complete = true;
        }
        if (currentLevel == 4)
        {
            trace.Level3Complete = true;
        }
        if (currentLevel == 5)
        {
            trace.Level4Complete = true;
        }

        // TODO: Figure out what happens with level 5/total completion of game.
    }

    // A function that executes upon opening the journal.
    private void OnJournalOpened()
    {
        picker = GameObject.Find("Tabs").GetComponent<NotebookTabPicker>();
        EventManager.Instance.SubscribeToEvent(EventType.OnTabChanged, OnTabChanged);
    }

    // A function that executes upon closing the journal.
    private void OnJournalClosed()
    {
        picker = null;
        EventManager.Instance.UnsubscribeToEvent(EventType.OnTabChanged, null);
    }

    // A function that processes notebook tab change events.
    private void OnTabChanged()
    {
        // Handle research tab.
        if (picker.CurrentTab == NotebookTab.Research)
        {
            Debug.Log("Research tab was opened.");
            researchOpen = true;
            currentSummaryTrace.NumResearchTabOpen += 1;
        } else if (picker.CurrentTab != NotebookTab.Research)
        {
            Debug.Log("Research tab was closed.");
            researchOpen = false;
        }

        // Handle observation tab.
        if (picker.CurrentTab == NotebookTab.Observe)
        {
            Debug.Log("Observation tab was opened.");
            observationOpen = true;
            currentSummaryTrace.NumObservationToolOpen += 1;
        } else if (picker.CurrentTab != NotebookTab.Research)
        {
            Debug.Log("Observation tab was closed.");
            observationOpen = false;
        }
    }

    // A function that ceases all tracking, updates the summary trace with current values, and submits
    // data to DB.
    private void SaveSummaryTrace()
    {
        Debug.Log("Sending summary trace to DB.");
    }
}
