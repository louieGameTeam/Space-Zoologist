﻿using System.Collections;
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
    // Initialize current level, set, and set trace.
    private int currentLevel;
    private int currentSet;
    private SetTrace currentSetTrace = null;
    // Boolean values indicating status of notebook tabs.
    private bool researchOpen = false;
    private bool observationOpen = false;
    // Keep track of certain objects in scene globally.
    private LevelDataReference levelData;
    private NotebookTabPicker picker;
    private float timer;

    // On Awake, check the status of the instance. If the instance is null, replace it with the current SummaryManager.
    // Else, destroy the gameObject this script is attached to. There can only be one.
    void Awake()
    {
        timer = 0;
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
                currentSummaryTrace = JsonUtility.FromJson<SummaryTrace>(response.data);
            // If no trace for the current user is found, create a new summary trace to work from.
            } else if (response.code == 2)
            {
                currentSummaryTrace = new SummaryTrace();
                currentSummaryTrace.PlayerID = GetPlayerID();
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

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 60) {
            timer = 0;
            SaveSummaryTrace();
        }

        // Begin tracking total play time.
        if (currentSummaryTrace != null)
        {
            currentSummaryTrace.TotalPlayTime += Time.deltaTime;
        }

        // Track time in levels. CurrentLevel is determined by OnSceneLoaded and CheckLevelAndSet functions.
        if (currentLevel == 0) {
            currentSummaryTrace.TutorialTime += Time.deltaTime;
        } else if (currentLevel == 1)
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
        // If current set trace is not null, we know we are leaving a level.
        if (currentSetTrace != null)
        {
            // Check progression statistics.
            CheckAndModifyProgression(currentLevel, currentSummaryTrace);
            // Add the current set trace to the list in the summary trace.
            currentSummaryTrace.SetTraces.Add(currentSetTrace);
        }

        // If the buildIndex of the scene is 3 (i.e., if we are in the MainLevel scene).
        if (scene.buildIndex == 3)
        {
            // Subscribe to all relevant events.
            SubscribeToEvents();
            // Get references to necessary objects.
            levelData = GameObject.Find("LevelData").GetComponent<LevelDataReference>();
            // Check current level and set (enclosure).
            CheckLevelAndSet(levelData);
            // Create a new set trace, and set the current set trace to the one created.
            currentSetTrace = CreateSetTrace(currentLevel, currentSet, currentSummaryTrace.PlayerID);
        }
    }

    // Placeholder functions to gather basic information related to player info: playerID and sessionID.
    // Note: PlayerID is set later on if they choose to sign in with their email.
    private string GetPlayerID()
    {
        return "default_user";
    }

    // Probably no longer necessary
    // private string GetSessionID()
    // {
    //     return "test";
    // }

    // A function that uses the event management system to subscribe to events used in this manager.
    private void SubscribeToEvents()
    {
        // Listen to notebook events being fired.
        EventManager.Instance.SubscribeToEvent(EventType.OnJournalOpened, OnJournalOpened);
        EventManager.Instance.SubscribeToEvent(EventType.OnJournalClosed, OnJournalClosed);
        // Listen to progression events being fired.
        EventManager.Instance.SubscribeToEvent(EventType.OnSetEnd, OnSetEnd);
        EventManager.Instance.SubscribeToEvent(EventType.OnSetPass, OnSetPass);
        EventManager.Instance.SubscribeToEvent(EventType.OnSetFail, OnSetFail);
        // Listen to events that trigger saves.
        EventManager.Instance.SubscribeToEvent(EventType.TriggerSave, SaveSummaryTrace);
    }

    // A function that sets the current level and set (enclosure) based on the levelData object.
    private void CheckLevelAndSet(LevelDataReference levelData)
    {
        string levelName = levelData.LevelData.Level.SceneName;

        // Parse level name for level information.
        if (levelName.Contains("Level0"))
        {
            currentLevel = 0;
        } else if (levelName.Contains("Level1"))
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
        else
        {
            currentLevel = 0;
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
        } else
        {
            currentSet = 1;
        }
    }

    // A hacky function that determines current progression statistics (this will probably need refining).
    // Currently, because not all players will pass through all enclosures to get to the next level,
    // we can't use status of enclosure completion to determine if an entire level was completed.
    // This instead sets completion of a level on the start of another level. This of course
    // leaves level 5 out of completion.
    private void CheckAndModifyProgression(int currentLevel, SummaryTrace trace)
    {
        SaveSummaryTrace();
        if (currentLevel == 0)
        {
            trace.TutorialComplete = true;
        }
        if (currentLevel == 1)
        {
            trace.Level1Complete = true;
        }
        if (currentLevel == 2)
        {
            trace.Level2Complete = true;
        }
        if (currentLevel == 3)
        {
            trace.Level3Complete = true;
        }
        if (currentLevel == 4)
        {
            trace.Level4Complete = true;
        }
        if (currentLevel == 5)
        {
            trace.Level5Complete = true;
        }
    }

    // A function that executes upon opening the journal.
    private void OnJournalOpened()
    {
        picker = GameObject.Find("Tabs").GetComponent<NotebookTabPicker>();
        EventManager.Instance.SubscribeToEvent(EventType.OnTabChanged, OnTabChanged);
        EventManager.Instance.SubscribeToEvent(EventType.OnBookmarkAdded, OnBookmarkAdded);
    }

    // A function that executes upon closing the journal.
    private void OnJournalClosed()
    {
        picker = null;
        EventManager.Instance.UnsubscribeToEvent(EventType.OnTabChanged, (Action)null);
        EventManager.Instance.UnsubscribeToEvent(EventType.OnBookmarkAdded, (Action)null);
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
            EventManager.Instance.SubscribeToEvent(EventType.OnArticleChanged, OnArticleChanged);
        } else if (picker.CurrentTab != NotebookTab.Research)
        {
            Debug.Log("Research tab was closed.");
            researchOpen = false;
            EventManager.Instance.UnsubscribeToEvent(EventType.OnArticleChanged, (Action)null);
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

    // A function that processes article change events within research tab.
    private void OnArticleChanged()
    {
        Debug.Log("An article was clicked.");
        currentSummaryTrace.NumArticlesRead += 1;
    }

    // A function that processes additions of bookmarks to the notebook.
    private void OnBookmarkAdded()
    {
        Debug.Log("Bookmark was added.");
        currentSummaryTrace.NumBookmarksCreated += 1;
    }

    // A function that creates and returns a new set trace; initiated by OnSceneLoaded.
    private SetTrace CreateSetTrace(int level, int set, string playerID)
    {
        SetTrace trace = new SetTrace();
        trace.PlayerID = playerID;
        trace.LevelID = level;
        trace.SetID = set;
        return trace;
    }

    // A function that executes at the end of a set (enclosure).
    // NOTE: This event is currently not invoked by anything.
    private void OnSetEnd()
    {
        Debug.Log("Set was ended.");
        currentSetTrace.NumDays = GameManager.Instance.CurrentDay;
        currentSetTrace.Currency = GameManager.Instance.Balance;
    }

    // A function that executes on successful completion of a set (enclosure).
    // NOTE: This event is currently not invoked by anything.
    private void OnSetPass()
    {
        Debug.Log("Set was completed successfully.");
        currentSetTrace.ResultEnum = SetTrace.Result.PASS;
    }

    // A function that executes on failure of a set (enclosure).
    // NOTE: This event is currently not invoked by anything.
    private void OnSetFail()
    {
        Debug.Log("Set was failed.");
        currentSetTrace.ResultEnum = SetTrace.Result.FAIL;
        // TODO: Assign the reason for failure from the enum in SetTrace.cs, like below.
        // currentSetTrace.FailureEnum = SetTrace.Failure.ENUM
    }

    // A function that executes on completion of the quiz/report-back, and records scores to the current set trace.
    // NOTE: This event is currently not invoked by anything.
    private void OnQuizEnd()
    {
        Debug.Log("Quiz ended.");
        // TODO: Assign the scores, as below.
        // currentSetTrace.TerrainScore = terrainScore;
        // currentSetTrace.FoodScore = foodScore;
    }

    // A function that serializes the current SummaryTrace object to JSON.
    private string ConvertSummaryTraceToJSON(SummaryTrace summaryTrace)
    {
        string json = JsonUtility.ToJson(summaryTrace);
        return json;
    }

    // A function that submits data to DB.
    private void SaveSummaryTrace()
    {
        Debug.Log("Sending summary trace to DB.");
        Debug.Log("Player ID POST: " + currentSummaryTrace.PlayerID);
        string json = ConvertSummaryTraceToJSON(currentSummaryTrace);
        StartCoroutine(SubmitSummaryTrace.TrySubmitSummaryTrace(json));
    }
}