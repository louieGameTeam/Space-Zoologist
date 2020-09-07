using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Stores all logs
    private List<LogEntry> worldLog = default;
    // Stores logs about populations
    private Dictionary<Population, List<LogEntry>> populationLogs = default;
    // Stores logs about food source
    private Dictionary<FoodSource, List<LogEntry>> foodSourceLogs = default;
    // Stores logs about enclosed area
    private Dictionary<EnclosedArea, List<LogEntry>> enclosedAreaLogs = default;

    private void Awake()
    {
        this.worldLog = new List<LogEntry>();
        this.populationLogs = new Dictionary<Population, List<LogEntry>>();
        this.foodSourceLogs = new Dictionary<FoodSource, List<LogEntry>>();
        this.enclosedAreaLogs = new Dictionary<EnclosedArea, List<LogEntry>>();
    }

    private void Start()
    {
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountIncreased, this.logPopulationIncrease);
        EventManager.Instance.SubscribeToEvent(EventType.PopulationCountDecreased, this.logPopulationDecrease);
    }


    private void logPopulationIncrease()
    {
        Population population = (Population)EventManager.Instance.LastEventInvoker;

        if (!this.populationLogs.ContainsKey(population))
        {
            this.populationLogs.Add(population, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Time.time.ToString(), $"{population.species.SpeciesName} population size increased!");

        this.populationLogs[population].Add(newLog);
    }

    private void logPopulationDecrease()
    {
        Population population = (Population)EventManager.Instance.LastEventInvoker;

        if (!this.populationLogs.ContainsKey(population))
        {
            this.populationLogs.Add(population, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Time.time.ToString(), $"{population.species.SpeciesName} population size decreased!");

        this.populationLogs[population].Add(newLog);
    }
}
