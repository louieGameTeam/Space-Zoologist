﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance { get { return _instance; } }

    #region Level Data Variables
    [Header("Used when playing level scene directly")]
    [SerializeField] string LevelOnPlay = "Level1E1";
    [SerializeField] private string directory = "Levels/";
    [Expandable, SerializeField] private LevelData m_levelData;
    public LevelData LevelData { get { return m_levelData; } }
    public SerializedLevel PresetMap { get; private set; }
    public Dictionary<string, FoodSourceSpecies> FoodSources = new Dictionary<string, FoodSourceSpecies>();
    public Dictionary<string, AnimalSpecies> AnimalSpecies = new Dictionary<string, AnimalSpecies>();
    public float Balance { get; private set; }
    #endregion

    #region Game State Variables
    [Header("Game State Variables")]
    [SerializeField] SceneNavigator SceneNavigator = default;
    [SerializeField] Button RestartButton = default;
    [SerializeField] Button NextLevelButton = default;
    [SerializeField] GameObject IngameUI = default;
    [SerializeField] GameObject GameOverHUD = default;
    [SerializeField] TextMeshProUGUI gameOverTitle = default;
    [SerializeField] TextMeshProUGUI gameOverText = default;

    [Header("Time Variables")]
    [SerializeField] int maxDay = 20;
    private int currentDay = 1;
    [SerializeField] Text CurrentDayText = default;
    public bool IsPaused { get; private set; }
    public bool WasPaused { get; private set; }
    [SerializeField] public GameObject PauseButton = default;
    private Image PauseButtonSprite = default;
    private Button PauseButtonButton = default;
    [SerializeField] private Sprite PauseSprite = default;
    [SerializeField] private Sprite ResumeSprite = default;

    public bool IsGameOver { get { return m_isGameOver; } }
    private bool m_isGameOver = false;
    private List<Objective> m_mainObjectives = new List<Objective>();
    private List<Objective> m_secondaryObjectives = new List<Objective>();
    public bool isMainObjectivesCompleted { get; private set; }
    public int numSecondaryObjectivesCompleted { get; private set; }

    private bool isObjectivePanelOpen;
    
    [Header("Objective Variables")]
    [SerializeField] private GameObject objectivePane = default;
    [SerializeField] private Text objectivePanelText = default;
    #endregion

    #region Need System Variables
    private Dictionary<NeedType, NeedSystem> m_needSystems;
    public Dictionary<NeedType, NeedSystem> NeedSystems { get { return m_needSystems; } }
    #endregion

    #region Managers
    public DialogueEditor.ConversationManager m_conversationManager { get; private set; }
    public DialogueManager m_dialogueManager { get; private set; }
    public ReservePartitionManager m_reservePartitionManager { get; private set; }
    public FoodSourceManager m_foodSourceManager { get; private set; }
    public PopulationManager m_populationManager { get; private set; }
    public ResourceManager m_resourceManager { get; private set; }
    public BehaviorPatternUpdater m_behaviorPatternUpdater { get; private set; }
    public TilePlacementController m_tilePlacementController { get; private set; }
    public PlotIO m_plotIO { get; private set; }
    public GridSystem m_gridSystem { get; private set; }
    public EnclosureSystem m_enclosureSystem { get; private set; }
    public Inspector m_inspector { get; private set; }
    public PlayerController m_playerController { get; private set; }
    #endregion

    #region Monobehaviour Callbacks
    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        m_levelData = LevelDataReference.instance.LevelData;
        SetManagers();
        LoadResources();
        SetNeedSystems();
        InitializeManagers();
        LoadLevelData();
        SetupObjectives();
        InitializeGameStateVariables();
        InitialNeedSystemUpdate();
    }

    void Update()
    {
        isMainObjectivesCompleted = true;
        numSecondaryObjectivesCompleted = 0;
        UpdateObjectives();

        if (isObjectivePanelOpen)
        {
            this.UpdateObjectivePanel();
        }

        // All objectives had reach end state
        if (isMainObjectivesCompleted && !this.m_isGameOver)
        {
            this.m_isGameOver = true;

            // TODO figure out what should happen when the main objectives are complete
            EventManager.Instance.InvokeEvent(EventType.MainObjectivesCompleted, null);

            // GameOver.cs listens for the event and handles gameover
            EventManager.Instance.InvokeEvent(EventType.GameOver, null);

            Debug.Log($"Level Completed!");
        }
    }
    #endregion

    #region Loading Functions
    public void SaveMap(string name = null, bool preset = true)
    {
        name = name ?? LevelOnPlay;
        name = name + ".json";
        string fullPath = preset ? "Assets/Resources/" + this.directory + name : Path.Combine(Application.persistentDataPath, name);

        Debug.Log("Saving Grid to " + fullPath);
        if (File.Exists(fullPath))
            Debug.Log("Overwriting file at " + fullPath);

        SerializedLevel level;
        try
        {
            level = new SerializedLevel();
            level.SetPopulations(m_populationManager);
            level.SetPlot(m_plotIO.SavePlot());
        }
        catch
        {
            Debug.LogError("Serialization error, NOT saved to protect existing saves");
            return;
        }

        using (StreamWriter streamWriter = new StreamWriter(fullPath))
            streamWriter.Write(JsonUtility.ToJson(level));
        Debug.Log("Grid Saved to: " + fullPath);
    }

    public void LoadMap(string name = null, bool preset = true)
    {
        name = name ?? m_levelData.Level.SceneName;
        string fullPath = preset ? this.directory + name : Path.Combine(Application.persistentDataPath, name);

        SerializedLevel serializedLevel;
        try
        {
            var jsonTextFile = Resources.Load<TextAsset>(fullPath).ToString();
            serializedLevel = JsonUtility.FromJson<SerializedLevel>(jsonTextFile);
        }
        catch
        {
            Debug.LogWarning("No map save found for this scene, create a map using map designer or check your spelling");
            Debug.Log("Creating Empty level");
            serializedLevel = new SerializedLevel();
            serializedLevel.SetPlot(new SerializedPlot(new SerializedMapObjects(),
                JsonUtility.FromJson<SerializedGrid>(File.ReadAllText(this.directory + "_defaultGrid.json"))));
        }
        Debug.Log("Loading " + name);
        m_plotIO.LoadPlot(serializedLevel.serializedPlot);
        //Animals loaded after map to avoid path finding issues
        this.PresetMap = serializedLevel;
        Reload();
    }

    public void Reload()
    {
        m_plotIO.Initialize();
        m_plotIO.ReloadGridObjectManagers();
        m_populationManager.Initialize();
    }

    private void LoadLevelData()
    {
        // set balance
        Balance = LevelData.StartingBalance;

        // set the food source dictionary
        foreach (FoodSourceSpecies foodSource in m_levelData.FoodSourceSpecies)
        {
            foreach (LevelData.ItemData data in m_levelData.ItemQuantities)
            {
                Item item = data.itemObject;
                if (item)
                {
                    if (item.Type.Equals(ItemType.Food) && item.ID.Equals(foodSource.SpeciesName) && !FoodSources.ContainsKey(item.ID))
                    {
                        this.FoodSources.Add(item.ID, foodSource);
                    }
                }
            }
        }

        // set the animal dictionary
        foreach (AnimalSpecies animalSpecies in m_levelData.AnimalSpecies)
        {
            if (AnimalSpecies.ContainsKey(animalSpecies.SpeciesName)) continue;
            this.AnimalSpecies.Add(animalSpecies.SpeciesName, animalSpecies);
        }

        LoadMap();
    }

    private void SetManagers()
    {
        // add the references to the managers here
        // temporary find function until scene reorganization
        m_conversationManager = FindObjectOfType<DialogueEditor.ConversationManager>();
        m_dialogueManager = FindObjectOfType<DialogueManager>();
        m_reservePartitionManager = FindObjectOfType<ReservePartitionManager>();
        m_foodSourceManager = FindObjectOfType<FoodSourceManager>();
        m_populationManager = FindObjectOfType<PopulationManager>();
        m_resourceManager = FindObjectOfType<ResourceManager>();
        m_behaviorPatternUpdater = FindObjectOfType<BehaviorPatternUpdater>();
        m_tilePlacementController = FindObjectOfType<TilePlacementController>();
        m_plotIO = FindObjectOfType<PlotIO>();
        m_gridSystem = FindObjectOfType<GridSystem>();
        m_enclosureSystem = FindObjectOfType<EnclosureSystem>();
        m_inspector = FindObjectOfType<Inspector>();
        m_playerController = FindObjectOfType<PlayerController>();
    }

    private void LoadResources()
    {
        // load everything from resources folder here and I mean EVERYTHING
        m_tilePlacementController.LoadResources();
    }

    private void InitializeManagers()
    {
        m_conversationManager.Initialize();
        m_dialogueManager.Initialize();
        m_reservePartitionManager.Initialize();
        m_foodSourceManager.Initialize();
        m_resourceManager.Initialize();
        m_tilePlacementController.Initialize();
    }

    private void SetupObjectives()
    {
        isObjectivePanelOpen = true;

        // Create the survival objectives
        foreach (SurvivalObjectiveData objectiveData in LevelData.LevelObjectiveData.survivalObjectiveDatas)
        {
            m_mainObjectives.Add(new SurvivalObjective(
                objectiveData.targetSpecies,
                objectiveData.targetPopulationCount,
                objectiveData.targetPopulationSize,
                objectiveData.timeRequirement
            ));
        }

        // Create the resource objective
        foreach (ResourceObjectiveData objectiveData in LevelData.LevelObjectiveData.resourceObjectiveDatas)
        {
            m_secondaryObjectives.Add(new ResourceObjective(objectiveData.amountToKeep));
        }

        // Add the population to related objective if not seen before
        EventManager.Instance.SubscribeToEvent(EventType.NewPopulation, () =>
        {
            Population population = (Population)EventManager.Instance.EventData;
            this.RegisterWithSurvivalObjectives(population);
        });
        this.UpdateObjectivePanel();
    }

    private void RegisterWithSurvivalObjectives(Population population)
    {
        // Debug.Log(population.gameObject.name + " attempting to update survival objective");
        foreach (Objective objective in m_mainObjectives)
        {
            if (objective.GetType() == typeof(SurvivalObjective))
            {
                SurvivalObjective survivalObjective = (SurvivalObjective)objective;
                if (survivalObjective.AnimalSpecies == population.species && !survivalObjective.Populations.Contains(population))
                {
                    // Debug.Log(population.name + " was added to survival objective");
                    survivalObjective.Populations.Add(population);
                }
            }
        }
    }

    private void InitializeGameStateVariables()
    {
        // set up the game state
        EventManager.Instance.SubscribeToEvent(EventType.GameOver, HandleNPCEndConversation);
        this.RestartButton.onClick.AddListener(() => { this.SceneNavigator.LoadLevel(this.SceneNavigator.RecentlyLoadedLevel); });
        this.NextLevelButton?.onClick.AddListener(() => { this.SceneNavigator.LoadLevelMenu(); });
        UpdateDayText(currentDay);
        this.IsPaused = false;
        this.WasPaused = false;
        this.PauseButtonSprite = this.PauseButton.GetComponent<Image>();
        this.PauseButtonButton = this.PauseButton.GetComponent<Button>();
    }
    #endregion

    #region Balance Functions
    public void SubtractFromBalance(float value)
    {
        if (this.Balance - value >= 0)
        {
            this.Balance -= value;
        }
    }

    public void SetBalance(float value)
    {
        this.Balance = value;
    }
    #endregion

    #region Need System Functions

    private void SetNeedSystems()
    {
        // create dictionary
        m_needSystems = new Dictionary<NeedType, NeedSystem>();

        // Add environmental NeedSystem
        AddNeedSystem(new TerrainNeedSystem());
        AddNeedSystem(new LiquidNeedSystem());
        // FoodSource and Species NS
        AddNeedSystem(new FoodSourceNeedSystem());
        AddNeedSystem(new PredatoryPreySystem());
    }

    private void InitialNeedSystemUpdate()
    {
        this.UpdateAllNeedSystems();
        m_populationManager.UpdateAllGrowthConditions();
        TogglePause();
        EventManager.Instance.SubscribeToEvent(EventType.PopulationExtinct, () =>
        {
            this.UnregisterWithNeedSystems((Life)EventManager.Instance.EventData);
        });
    }

    /// <summary>
    /// Register a Population or FoodSource with the systems using the strings need names.b
    /// </summary>
    /// <param name="life">This could be a Population or FoodSource since they both inherit from Life</param>
    public void RegisterWithNeedSystems(Life life)
    {
        // Register to NS by NeedType (string)
        foreach (Need need in life.GetNeedValues().Values)
        {
            Debug.Assert(m_needSystems.ContainsKey(need.NeedType), $"No { need.NeedType } system");
            m_needSystems[need.NeedType].AddConsumer(life);
        }
    }

    public void UnregisterWithNeedSystems(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            Debug.Assert(m_needSystems.ContainsKey(need.NeedType), $"No { need } system");
            m_needSystems[need.NeedType].RemoveConsumer(life);
        }
    }

    /// <summary>
    /// Add a system so that populations can register with it via it's need name.
    /// </summary>
    /// <param name="needSystem">The system to add</param>
    private void AddNeedSystem(NeedSystem needSystem)
    {
        if (!this.m_needSystems.ContainsKey(needSystem.NeedType))
        {
            m_needSystems.Add(needSystem.NeedType, needSystem);
        }
        else
        {
            Debug.Log($"{needSystem.NeedType} need system already existed");
        }
    }

    public void UpdateAllNeedSystems()
    {
        foreach (KeyValuePair<NeedType, NeedSystem> entry in m_needSystems)
        {
            entry.Value.UpdateSystem();
        }
    }


    public void UpdateNeedSystem(NeedType needType)
    {
        if (this.m_needSystems.ContainsKey(needType))
        {
            this.m_needSystems[needType].UpdateSystem();
        }
    }

    public void UpdateAccessMap()
    {
        m_reservePartitionManager.UpdateAccessMapChangedAt(m_gridSystem.ChangedTiles.ToList<Vector3Int>());
    }

    /// <summary>
    /// Update all the need system that is mark "dirty"
    /// </summary>
    /// <remarks>
    /// The order of the NeedSystems' update metter,
    /// this should be their relative order(temp) :
    /// Terrian/Atmosphere -> Species -> FoodSource -> Density
    /// This order can be gerenteed in how NeedSystems is add to the manager in Awake()
    /// </remarks>
    public void UpdateDirtyNeedSystems()
    {
        // Update populations' accessible map when terrain was modified
        if (m_gridSystem.HasTerrainChanged)
        {
            // TODO: Update population's accessible map only for changed terrain
            m_reservePartitionManager.UpdateAccessMapChangedAt(m_gridSystem.ChangedTiles.ToList<Vector3Int>());
        }

        foreach (KeyValuePair<NeedType, NeedSystem> entry in m_needSystems)
        {
            NeedSystem system = entry.Value;
            if (system.IsDirty)
            {
                //Debug.Log($"Updating {system.NeedType} NS by dirty flag");
                system.UpdateSystem();
            }
            else if (system.CheckState())
            {
                //Debug.Log($"Updating {system.NeedType} NS by dirty pre-check");
                system.UpdateSystem();
            }
        }

        // Reset pop accessibility status
        m_populationManager.UdateAllPopulationStateForChecking();

        // Reset food source accessibility status
        m_foodSourceManager.UpdateAccessibleTerrainInfoForAll();

        // Reset terrain modified flag
        m_gridSystem.HasTerrainChanged = false;
    }
    #endregion

    #region Game State Functions
    public void TryToPause()
    {
        // prevents accidentally unpausing when should not (two different accessors)
        if (this.IsPaused)
            this.WasPaused = true;
        else
            this.Pause();
    }

    public void TryToUnpause()
    {
        // prevents accidentally pausing when should not (two different accessors)
        if (this.WasPaused)
            this.WasPaused = false;
        else
            this.Unpause();
    }

    public void TogglePause()
    {
        if (this.IsPaused)
        {
            this.Unpause();
        }
        else
        {
            this.Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 1;
        this.IsPaused = true;
        this.PauseButtonSprite.sprite = this.ResumeSprite;
        this.PauseButtonButton.onClick.RemoveListener(this.Pause);
        this.PauseButtonButton.onClick.AddListener(this.Unpause);
        foreach (Population population in m_populationManager.Populations)
            population.PauseAnimalsMovementController();
        m_gridSystem.UpdateAnimalCellGrid();
        AudioManager.instance?.PlayOneShot(SFXType.Pause);
    }

    public void Unpause()
    {
        this.IsPaused = false;
        this.PauseButtonSprite.sprite = this.PauseSprite;
        this.PauseButtonButton.onClick.RemoveListener(this.Unpause);
        this.PauseButtonButton.onClick.AddListener(this.Pause);
        foreach (Population population in m_populationManager.Populations)
            population.UnpauseAnimalsMovementController();
        AudioManager.instance?.PlayOneShot(SFXType.Unpause);
    }

    public void TwoTimeSpeed()
    {
        Time.timeScale = 2;
    }

    public void FourTimeSpeed()
    {
        Time.timeScale = 4;
    }

    public void HandleNPCEndConversation()
    {
        if (!(currentDay < maxDay))
        {
            m_dialogueManager.SetNewDialogue(m_levelData.RestartConversation);
        }
        else
        {
            m_dialogueManager.SetNewDialogue(m_levelData.PassedConversation);
        }
        m_dialogueManager.StartInteractiveConversation();
        this.IngameUI.SetActive(false);
    }

    public void HandleGameOver()
    {
        Pause();
        this.GameOverHUD.SetActive(true);
        this.IngameUI.SetActive(false);


        // Game completed
        if (isMainObjectivesCompleted)
        {
            if (NextLevelButton != null)
                NextLevelButton.interactable = true;
            // title.text = "Objectives Complete";
            //text.text = "Completed Secondary Objectives: " + objectiveManager.NumSecondaryObjectivesCompleted;
        }
        else
        {
            // Game lost
            if (NextLevelButton != null)
                NextLevelButton.interactable = false;
            gameOverTitle.text = "Mission Failed";
            gameOverText.text = "";
        }
    }

    public void ToggleObjectivePanel()
    {
        this.isObjectivePanelOpen = !this.isObjectivePanelOpen;
        this.objectivePane.SetActive(this.isObjectivePanelOpen);
        UpdateObjectives();
        this.UpdateObjectivePanel();
    }

    public void TurnObjectivePanelOff()
    {
        this.isObjectivePanelOpen = false;
        this.objectivePane.SetActive(this.isObjectivePanelOpen);
        UpdateObjectives();
        this.UpdateObjectivePanel();
    }

    private void UpdateObjectives()
    {
        // Level is completed when all mian objectives are done, failed when one has failed
        foreach (Objective objective in m_mainObjectives)
        {
            if (objective.UpdateStatus() == ObjectiveStatus.InProgress)
            {
                isMainObjectivesCompleted = false;
            }

            if (objective.UpdateStatus() == ObjectiveStatus.Failed)
            {
                // GameOver.cs listens for the event and handles gameover
                EventManager.Instance.InvokeEvent(EventType.GameOver, null);
            }
        }

        // Secondary objective status can be viewed on screen
        foreach (Objective objective in m_secondaryObjectives)
        {
            if (objective.UpdateStatus() == ObjectiveStatus.Completed)
            {
                numSecondaryObjectivesCompleted++;
            }
        }
    }

    public void UpdateObjectivePanel()
    {
        string displayText = "\n";

        foreach (Objective objective in m_mainObjectives)
        {
            displayText += objective.GetObjectiveText();
        }
        if (m_secondaryObjectives.Count == 0)
        {
            this.objectivePanelText.text = displayText;
            return;
        }
        displayText += "Secondary Objectives:\n";
        foreach (Objective objective in m_secondaryObjectives)
        {
            displayText += objective.GetObjectiveText();
        }

        this.objectivePanelText.text = displayText;
    }

    private void UpdateDayText(int day)
    {
        CurrentDayText.text = "" + day;
        if (maxDay > 0)
        {
            CurrentDayText.text += " / " + maxDay;
        }
    }

    public bool LessThanMaxDay()
    {
        return currentDay <= maxDay;
    }

    public void nextDay()
    {
        m_gridSystem.CountDown();
        m_populationManager.UpdateAccessibleLocations();
        m_populationManager.UpdateAllPopulationRegistration();
        UpdateAllNeedSystems();
        for (int i = m_populationManager.Populations.Count - 1; i >= 0; i--)
        {
            m_populationManager.Populations[i].HandleGrowth();
        }
        m_populationManager.UpdateAllGrowthConditions();
        m_inspector.UpdateCurrentDisplay();
        UpdateDayText(++currentDay);
    }
    #endregion

    #region Other Functions
    // no idea what these two do
    public void ClearAnimals()
    {
        SerializedLevel level = new SerializedLevel();
        level.SetPopulations(m_populationManager);
        level.SetPlot(m_plotIO.SavePlot());
        level.serializedPopulations = new SerializedPopulation[0];
        this.PresetMap = level;

        foreach (Population population in m_populationManager.Populations)
        {
            population.RemoveAll();
        }

        m_populationManager.Initialize();
    }
    public void ClearMapObjects()
    {
        FoodSourceManager foodSourceManager = FindObjectOfType<FoodSourceManager>();
        foodSourceManager.DestroyAll();
    }
    #endregion
}
