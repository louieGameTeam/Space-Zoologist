using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// This system hanldes creating, saving and displaying the logs. 
/// </summary>
public class LogSystem : MonoBehaviour
{
    /// <summary>
    /// Data structure to store info of a log entry
    /// </summary>
    private class LogEntry
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

    // Log window
    [SerializeField] private GameObject logWindow = default;
    // Log text
    [SerializeField] private Text logWindowText = default;
    [SerializeField] private EnclosureSystem enclosureSystem = default;

    private EventManager eventManager;

    public void ToggleLog()
    {
        //Debug.Log("open log");

        this.logWindow.SetActive(!this.isInLogSystem);
        this.isInLogSystem = !this.isInLogSystem;
    }

    private void Update()
    {
        if (this.isInLogSystem)
        {
            this.displayWorldLog();
        }
    }

    private void Awake()
    {
        this.worldLog = new List<LogEntry>();
        this.populationLogs = new Dictionary<Population, List<LogEntry>>();
        this.foodSourceLogs = new Dictionary<FoodSource, List<LogEntry>>();
        this.enclosedAreaLogs = new Dictionary<EnclosedArea, List<LogEntry>>();
    }

    private void Start()
    {
        this.eventManager = EventManager.Instance;

        // Subscribe to the events
        this.eventManager.SubscribeToEvent(EventType.PopulationCountChange, () =>
        {
            this.handleLog(EventType.PopulationCountChange);
        });
        this.eventManager.SubscribeToEvent(EventType.PopulationExtinct, () =>
        {
            this.handleLog(EventType.PopulationExtinct);
        });
        this.eventManager.SubscribeToEvent(EventType.NewPopulation, () =>
        {
            this.handleLog(EventType.NewPopulation);
        });
        this.eventManager.SubscribeToEvent(EventType.NewFoodSource, () =>
        {
            this.handleLog(EventType.NewFoodSource);
        });
        this.eventManager.SubscribeToEvent(EventType.NewEnclosedArea, () =>
        {
            this.handleLog(EventType.NewEnclosedArea);
        });
        this.eventManager.SubscribeToEvent(EventType.AtmosphereChange, () =>
        {
            this.handleLog(EventType.AtmosphereChange);
        });
        this.eventManager.SubscribeToEvent(EventType.LiquidChange, () =>
        {
            this.handleLog(EventType.LiquidChange);
        });
        this.eventManager.SubscribeToEvent(EventType.FoodSourceChange, () =>
        {
            this.handleLog(EventType.FoodSourceChange);
        });
        this.eventManager.SubscribeToEvent(EventType.TerrainChange, () =>
        {
            this.handleLog(EventType.TerrainChange);
        });
    }

    private void handleLog(EventType eventType)
    {
        switch(eventType)
        {
            case EventType.PopulationCountChange:
                this.logPopulationCountChange(((Population population, bool increased)) EventManager.Instance.EventData);
                break;
            case EventType.PopulationExtinct:
                this.logPopulationExtinct((Population)EventManager.Instance.EventData);
                break;
            case EventType.NewPopulation:
                this.logNewCreation((Population)EventManager.Instance.EventData);
                break;
            case EventType.NewFoodSource:
                this.logNewCreation((FoodSource)EventManager.Instance.EventData);
                break;
            case EventType.NewEnclosedArea:
                this.logNewCreation((EnclosedArea)EventManager.Instance.EventData);
                break;
            case EventType.AtmosphereChange:
                this.logAtmoesphereChange((EnclosedArea)EventManager.Instance.EventData);
                break;
            case EventType.LiquidChange:
                this.logLiquidChange((Vector3Int)EventManager.Instance.EventData);
                break;
            case EventType.FoodSourceChange:
                this.logFoodSourceChanged((FoodSource)EventManager.Instance.EventData);
                break;
            case EventType.TerrainChange:
                this.logTerrainChange((List<Vector3Int>)EventManager.Instance.EventData);
                break;
            default:
                Debug.Assert(true, $"LogSystem does not knows how to handle {eventType} yet");
                break;
        }
    }

    private void displayWorldLog()
    {
        string logText = "\n";

        // if (this.worldLog.Count == 0)
        // {
        //     this.logWindowText.text = "Log\n" + "None\n";
        // }

        foreach(LogEntry logEntry in this.worldLog)
        {
            logText += $"{logEntry.GetDisplay()}\n";
        }

        this.logWindowText.text = logText;
    }

    private void logTerrainChange(List<Vector3Int> changedTiles)
    {
        List<byte> seenEnclosedAreaIds = new List<byte>();

        foreach (Vector3Int pos in changedTiles)
        {
            EnclosedArea enclosedArea = this.enclosureSystem.GetEnclosedAreaByCellPosition(pos);

            if (!seenEnclosedAreaIds.Contains(enclosedArea.id))
            {
                if (!this.enclosedAreaLogs.ContainsKey(enclosedArea))
                {
                    this.enclosedAreaLogs.Add(enclosedArea, new List<LogEntry>());
                }

                LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"Terrain changed in enclosed area {enclosedArea.id}");
                this.enclosedAreaLogs[enclosedArea].Add(newLog);
                this.worldLog.Add(newLog);

                seenEnclosedAreaIds.Add(enclosedArea.id);
            }
        }
    }

    private void logFoodSourceChanged (FoodSource foodSource)
    {
        if (!this.foodSourceLogs.ContainsKey(foodSource))
        {
            this.foodSourceLogs.Add(foodSource, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"{foodSource.Species.name} output changed!");
        this.foodSourceLogs[foodSource].Add(newLog);
        this.worldLog.Add(newLog);
    }

    private void logLiquidChange(Vector3Int cellPos)
    {
        enclosureSystem.UpdateEnclosedAreas();
        EnclosedArea enclosedArea = this.enclosureSystem.GetEnclosedAreaByCellPosition(cellPos);

        if (!this.enclosedAreaLogs.ContainsKey(enclosedArea))
        {
            this.enclosedAreaLogs.Add(enclosedArea, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"Liquid composition changed at enclose area {enclosedArea.id}!");
        this.enclosedAreaLogs[enclosedArea].Add(newLog);
        this.worldLog.Add(newLog);
    }

    private void logAtmoesphereChange(EnclosedArea enclosedArea)
    {
        LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"Atmospheric composition changed at enclose area {enclosedArea.id}!");

        if (!this.enclosedAreaLogs.ContainsKey(enclosedArea))
        {
            this.enclosedAreaLogs.Add(enclosedArea, new List<LogEntry>());
        }

        this.enclosedAreaLogs[enclosedArea].Add(newLog);
        this.worldLog.Add(newLog);
    }

    private void logNewCreation(object creation)
    {
        if (creation.GetType() == typeof(Population))
        {
            Population population = (Population)creation;

            if (!this.populationLogs.ContainsKey(population))
            {
                this.populationLogs.Add(population, new List<LogEntry>());
            }

            LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"New {population.species.SpeciesName} created!");

            this.populationLogs[population].Add(newLog);
            this.worldLog.Add(newLog);
        }
        else if (creation.GetType() == typeof(FoodSource))
        {
            FoodSource foodSource = (FoodSource)creation;

            if (!this.foodSourceLogs.ContainsKey(foodSource))
            {
                this.foodSourceLogs.Add(foodSource, new List<LogEntry>());
            }

            LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"New {foodSource.Species.SpeciesName} created!");

            this.foodSourceLogs[foodSource].Add(newLog);
            this.worldLog.Add(newLog);
        }
        else if (creation.GetType() == typeof(EnclosedArea))
        {
            EnclosedArea enclosedArea = (EnclosedArea)creation;

            // Don't log empty enclosed area
            if (enclosedArea.coordinates.Count == 0)
            {
                return;
            }

            if (!this.enclosedAreaLogs.ContainsKey(enclosedArea))
            {
                this.enclosedAreaLogs.Add(enclosedArea, new List<LogEntry>());
            }

            LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"Enclosed area {enclosedArea.id} created");

            this.enclosedAreaLogs[enclosedArea].Add(newLog);
            this.worldLog.Add(newLog);
        }
    }

    private void logPopulationCountChange((Population population, bool increased) populationData)
    {
        if (!this.populationLogs.ContainsKey(populationData.population))
        {
            this.populationLogs.Add(populationData.population, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"{populationData.population.species.SpeciesName} population size {(populationData.increased ? "increased" : "decreased")}!");

        // Store to population log
        this.populationLogs[populationData.population].Add(newLog);
        // Store to world log
        this.worldLog.Add(newLog);
    }

    private void logPopulationExtinct(Population population)
    {
        if (!this.populationLogs.ContainsKey(population))
        {
            this.populationLogs.Add(population, new List<LogEntry>());
        }

        LogEntry newLog = new LogEntry(Math.Round(Time.time, 0, MidpointRounding.AwayFromZero).ToString(), $"{population.species.SpeciesName} has gone extinct!");

        this.populationLogs[population].Add(newLog);
        this.worldLog.Add(newLog);
    }
}
