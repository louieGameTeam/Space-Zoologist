using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    // Initialize the instance of this GameManager to null.
    private static GameManager instance = null;
    private PlayTrace currentPlayTrace = new PlayTrace();
    // Initialize a level trace to use as the current level.
    private LevelTrace currentLevelTrace = null;
    // Initialize a day trace to use for the current day in the level being played.
    private DayTrace currentDayTrace = null;

    // On Awake, check the status of the instance. If the instance is null, replace it with the current GameManager.
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
    }

    // Enable and disable functions to allocate delegates.
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get basic information on start.
        currentPlayTrace.PlayerID = GetPlayerID();
        currentPlayTrace.SessionID = GetSessionID();

        // Initialize the list of level traces.
        currentPlayTrace.LevelTraces = new List<LevelTrace>();

        // Listen to next day events being fired.
        Action onNextDay += NextDayTrace(currentPlayTrace.SessionElapsedTime, currentDayTrace);   
        EventManager.Instance.SubscribeToEvent(EventType.OnNextDay, onNextDay);
    }

    // Update is called once per frame
    void Update()
    {
        // Begin tracking elapsed time.
        currentPlayTrace.SessionElapsedTime += Time.deltaTime;
    }

    // Use scene change functions to determine level being loaded/unloaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        // If the buildIndex of the scene is greater than 2 (i.e. not the introduction, main menu, or level menu), get level information for trace.
        if (scene.buildIndex > 2)
        {
            // Get references to the TimeSystem object and the PlayerBalance object.
            int currentDay = GameObject.Find("TimeSystem").GetComponent<TimeSystem>().CurrentDay;
            float playerBalance = GameObject.Find("PlayerBalance").GetComponent<PlayerBalance>().Balance;
            // Set the current level trace.
            currentLevelTrace = GetLevelStartInformation(scene.buildIndex, currentPlayTrace.SessionElapsedTime);
            // Create the starting day trace.
            currentDayTrace = CreateDayTrace(currentPlayTrace.SessionElapsedTime, currentDay, playerBalance);
            DebugDayTrace(currentDayTrace);
            StartCoroutine(ChangeScene());
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("OnSceneUnloaded: " + scene.name);
        // If the buildIndex of the scene is greater than 2 (i.e. not the introduction, main menu, or level menu), get trace for current level and modify it.
        if (scene.buildIndex > 2)
        {
            currentLevelTrace = GetLevelEndInformation(currentLevelTrace, currentPlayTrace.SessionElapsedTime);
            currentPlayTrace.LevelTraces.Add(currentLevelTrace);
            // Reset the current level and day traces.
            currentLevelTrace = null;
            currentDayTrace = null;
            DebugLevelTrace(currentPlayTrace.LevelTraces);
            string json = ConvertPlayTraceToJSON(currentPlayTrace);
            Debug.Log(json);
            StartCoroutine(SubmitPlayTrace.TrySubmitPlayTrace(json));
        }
    }

    // Public method for accessing PlayTrace object.
    public PlayTrace CurrentPlayTrace
    {
        get { return currentPlayTrace; }
        set { currentPlayTrace = value; }
    }

    // Placeholder functions to gather basic information related to playtrace: playerID and sessionID.
    private string GetPlayerID()
    {
        return "test";
    }

    private string GetSessionID()
    {
        return "test";
    }

    // A function that returns a level trace, initiated by a scene load.
    // Input: buildIndex - the level ID in the build settings; timestamp - the time (in elapsed seconds) this level started
    private LevelTrace GetLevelStartInformation(int buildIndex, float timestamp)
    {
        LevelTrace levelTrace = new LevelTrace();
        levelTrace.LevelID = buildIndex;
        levelTrace.LevelStartTime = timestamp;
        levelTrace.LevelComplete = false;
        levelTrace.DayTraces = new List<DayTrace>();

        return levelTrace;
    }

    // A function that modifies and returns a level trace, initiated by a scene unload.
    // Input: levelTrace - the current level being traced; timestamp - the time (in elapsed seconds) this level ended
    private LevelTrace GetLevelEndInformation(LevelTrace levelTrace, float timestamp)
    {
        // Get reference to the PlayerBalance object.
        // float playerBalance = GameObject.Find("PlayerBalance").GetComponent<PlayerBalance>().Balance;
        levelTrace.LevelEndTime = timestamp;
        levelTrace.LevelDeltaTime = levelTrace.LevelEndTime - levelTrace.LevelStartTime;
        CloseDayTrace(timestamp, currentDayTrace, 0);
        levelTrace.LevelElapsedDays = levelTrace.DayTraces.Count;
        levelTrace.LevelComplete = true;

        return levelTrace;
    }

    // A function that creates a new day trace, initiated either by a level load or advancing the time.
    // Input: timestamp - the time (in elapsed seconds) this day started
    private DayTrace CreateDayTrace(float timestamp, int currentDay, float playerBalance)
    {
        //Debug.Log("creating new day trace...");
        // Initialize the day trace.
        DayTrace dayTrace = new DayTrace();
        //Debug.Log("Current day is: " + timeSystem.GetComponent<TimeSystem>().CurrentDay);
        dayTrace.DayID = currentDay;
        dayTrace.DayStartTime = timestamp;
        dayTrace.BalanceStart = playerBalance;
        dayTrace.JournalTraces = new List<JournalTrace>();
        dayTrace.JournalTime = 0f;

        return dayTrace;
    }

    // A function that closes out a day trace by providing end of day information.
    private void CloseDayTrace(float timestamp, DayTrace currentDayTrace, float playerBalance)
    {
        // Close out the current day trace and push it to the list held by the level trace.
        currentDayTrace.DayEndTime = timestamp;
        currentDayTrace.DayDeltaTime = currentDayTrace.DayEndTime - currentDayTrace.DayStartTime;
        currentDayTrace.BalanceEnd = playerBalance;
        currentLevelTrace.DayTraces.Add(currentDayTrace);
    }

    // A function that modifies the current day trace, pushes it to the list of day traces for the level, and sets a new current day trace.
    // Initiated by the onNextDay event.
    private void NextDayTrace(float timestamp, DayTrace currentDayTrace)
    {
        // Get references to the TimeSystem object and the PlayerBalance object.
        int currentDay = GameObject.Find("TimeSystem").GetComponent<TimeSystem>().CurrentDay;
        float playerBalance = GameObject.Find("PlayerBalance").GetComponent<PlayerBalance>().Balance;
        CloseDayTrace(timestamp, currentDayTrace, playerBalance);
        DayTrace newDayTrace = CreateDayTrace(timestamp, currentDay, playerBalance);
        this.currentDayTrace = newDayTrace;
    }

    // A function that serializes the current PlayTrace object to JSON.
    private string ConvertPlayTraceToJSON(PlayTrace playTrace)
    {
        string json = JsonUtility.ToJson(playTrace);
        return json;
    }

    // Debug functions to help test.
    private void DebugLevelTrace(List<LevelTrace> levelTraces)
    {
        Debug.Log("Level traces:");
        foreach (LevelTrace levelTrace in levelTraces)
        {
            Debug.Log(levelTrace.LevelID);
            Debug.Log(levelTrace.LevelStartTime);
            Debug.Log(levelTrace.LevelEndTime);
            Debug.Log(levelTrace.LevelDeltaTime);
            Debug.Log(levelTrace.LevelComplete);
            Debug.Log("Day traces:");
            foreach (DayTrace dayTrace in levelTrace.DayTraces)
            {
                DebugDayTrace(dayTrace);
            }
        }
    }

    private void DebugDayTrace(DayTrace dayTrace)
    {
        Debug.Log(dayTrace.DayID);
        Debug.Log(dayTrace.DayStartTime);
        Debug.Log(dayTrace.DayEndTime);
        Debug.Log(dayTrace.DayDeltaTime);
        Debug.Log(dayTrace.BalanceStart);
        Debug.Log(dayTrace.BalanceEnd);
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(2);
    }
}
