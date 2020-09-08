using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This system hanldes creating, saving and displaying the logs. 
/// </summary>
public class LogSystem : MonoBehaviour
{
    /// <summary>
    /// Data structure to store info of a log entry
    /// </summary>
    public class LogEntry
    {
        private string logTime;
        private string logText;

        public LogEntry(string logTime, string logText)
        {
            this.logTime = logTime;
            this.logText = logText;
        }

        public string GetDisplay()
        {
            return $"[{this.logTime}] {this.logText}";
        }
    }

    // Stores all logs
    private List<LogEntry> worldLog = default;
    // Stores logs about populations
    private Dictionary<Population, List<LogEntry>> populationLogs = default;
    // Stores logs about food source
    private Dictionary<FoodSource, List<LogEntry>> foodSourceLogs = default;
    // Stores logs about enclosed area
    private Dictionary<EnclosedArea, List<LogEntry>> enclosedAreaLogs = default;

    private bool isInLogSystem = false;
    private EventType lastEventType = default; // Default is the enum at 0

    // Log window
    [SerializeField] private GameObject logWindow = default;
    // Log text
    [SerializeField] private Text logWindowText = default;

    private void Awake()
    {
        this.worldLog = new List<LogEntry>();
        this.populationLogs = new Dictionary<Population, List<LogEntry>>();
        this.foodSourceLogs = new Dictionary<FoodSource, List<LogEntry>>();
        this.enclosedAreaLogs = new Dictionary<EnclosedArea, List<LogEntry>>();
    }

    private void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountIncreased, () =>
        {
            this.lastEventType = EventType.PopulationCountIncreased;
            this.handleLog();
        });
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountDecreased, () =>
        {
            this.lastEventType = EventType.PopulationCountDecreased;
            this.handleLog();
        });
    }

    private void handleLog()
    {
        if (this.lastEventType == EventType.PopulationCountIncreased)
        {
            this.logPopulationIncrease((Population)EventManager.Instance.LastEventInvoker);
        }
        else if (this.lastEventType == EventType.PopulationCountDecreased)
        {
            this.logPopulationDecrease((Population)EventManager.Instance.LastEventInvoker);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("l"))
        {
            Debug.Log("open log");

            this.logWindow.SetActive(!this.isInLogSystem);
            this.isInLogSystem = !this.isInLogSystem;

            if (this.isInLogSystem)
            {
                this.displayWorldLog();
            }
        }
    }

    private void displayWorldLog()
    {
        string logText = "Log\n";

        if (this.worldLog.Count == 0)
        {
            this.logWindowText.text = "Log\n" + "None\n";
        }

        foreach(LogEntry logEntry in this.worldLog)
        {
            logText += $"{logEntry.GetDisplay()}\n";
        }

        this.logWindowText.text = logText;
    }


    private void logPopulationIncrease(Population population)
    {
        if (!this.populationLogs.ContainsKey(population))
        {
            this.populationLogs.Add(population, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Time.time.ToString(), $"{population.species.SpeciesName} population size increased!");

        // Store to population log
        this.populationLogs[population].Add(newLog);
        // Store to world log
        this.worldLog.Add(newLog);
    }

    private void logPopulationDecrease(Population population)
    {
        if (!this.populationLogs.ContainsKey(population))
        {
            this.populationLogs.Add(population, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Time.time.ToString(), $"{population.species.SpeciesName} population size decreased!");

        // Store to population log
        this.populationLogs[population].Add(newLog);
        // Store to world log
        this.worldLog.Add(newLog);
    }
}
