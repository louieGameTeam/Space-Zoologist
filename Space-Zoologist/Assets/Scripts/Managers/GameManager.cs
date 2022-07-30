using System.Collections;
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

    [SerializeField] private bool debug = false;
    public bool IsDebug { get { return debug; } set { return; } }
    [SerializeField] private bool autoWin = false;
    [SerializeField] private bool skipConversation = false;

    #region Level Data Variables
    [Header("Used when playing level scene directly")]
    [SerializeField] string LevelOnPlay = "Level1E1";
    [SerializeField] private string directory = "Levels/";
    [Expandable, SerializeField] private LevelData m_levelData;
    public LevelData LevelData { get { return m_levelData; } }
    public SerializedLevel PresetMap { get; private set; }
    public Dictionary<ItemID, FoodSourceSpecies> FoodSources = new Dictionary<ItemID, FoodSourceSpecies>();
    public Dictionary<ItemID, AnimalSpecies> AnimalSpecies = new Dictionary<ItemID, AnimalSpecies>();
    public float Balance { get; private set; }
    #endregion

    #region Game State Variables
    [Header("Game State Variables")]
    [SerializeField] SceneNavigator SceneNavigator = default;
    [SerializeField] Button RestartButton = default;
    [SerializeField] Button NextLevelButton = default;
    [SerializeField] Toggle ObjectiveToggle = default;
    [SerializeField] Toggle InspectorToggle = default;
    [SerializeField] GameObject IngameUI = default;
    [SerializeField] GameObject GameOverHUD = default;
    [SerializeField] TextMeshProUGUI gameOverTitle = default;
    [SerializeField] TextMeshProUGUI gameOverText = default;

    [Header("UI references")]
    // I figured it was best to have these as serialized because we don't know whether
    // they are off/on at the start of the level, so we can't guarantee that a raw
    // "FindObjectWithType" will find it
    [SerializeField] NotebookUI notebookUI = default;
    public NotebookUI NotebookUI => notebookUI;
    [SerializeField] BuildUI buildUI = default;
    public BuildUI BuildUI => buildUI;
    [SerializeField] InspectorObjectiveUI inspectorObjectiveUI = default;
    public InspectorObjectiveUI InspectorObjectUI => inspectorObjectiveUI;

    [Header("Time Variables")]
    [SerializeField] int maxDay = 20;
    private int currentDay = 1;
    // Readonly accessor for the current day
    public int CurrentDay => currentDay;
    [SerializeField] Text CurrentDayText = default;
    public bool IsPaused { get; private set; }

    private HashSet<string> m_pauseStack = new HashSet<string>();

    public bool IsGameOver { get { return m_isGameOver; } }
    private bool m_isGameOver = false;
    private List<Objective> m_mainObjectives = new List<Objective>();
    private List<Objective> m_secondaryObjectives = new List<Objective>();
    public bool isMainObjectivesCompleted { get; private set; }
    public int numSecondaryObjectivesCompleted { get; private set; }

    public bool isObjectivePanelOpen { get; private set; }

    [Header("Objective Variables")]
    [SerializeField] private GameObject objectivePane = default;
    [SerializeField] private TextMeshProUGUI objectivePanelText = default;
    #endregion

    #region Need System Variables
    public NeedCache Needs { get; private set; }
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
    public TileDataController m_tileDataController { get; private set; }
    public EnclosureSystem m_enclosureSystem { get; private set; }
    public Inspector m_inspector { get; private set; }
    public PlayerController m_playerController { get; private set; }
    public CameraController m_cameraController { get; private set; }
    public MenuManager m_menuManager { get; private set; }
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
        InitializeManagers();
        InitializeUI();
        LoadLevelData();
        SetupObjectives();
        InitializeGameStateVariables();
        Needs = new NeedCache();
        Needs.RebuildIfDirty();
    }

    void Update()
    {
        if (autoWin)
        {
            DebugWin();
            autoWin = false;
        }
        if (skipConversation)
        {
            m_conversationManager.EndConversation();
            skipConversation = false;
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
        /*try
        {*/
            level = new SerializedLevel();
            level.SetPopulations(m_populationManager);
            level.SetPlot(m_plotIO.SavePlot());
        /*}
        catch
        {
            Debug.LogError("Serialization error, NOT saved to protect existing saves");
            return;
        }*/

        using (StreamWriter streamWriter = new StreamWriter(fullPath))
            streamWriter.Write(JsonUtility.ToJson(level));
        Debug.Log("Grid Saved to: " + fullPath);
    }

    public SerializedLevel LoadMap(string name = null, bool preset = true)
    {
        name = name ?? m_levelData.Level.SceneName;
        string fullPath = preset ? this.directory + name : Path.Combine(Application.persistentDataPath, name);

        SerializedLevel serializedLevel;
        try
        {
            var jsonTextFile = Resources.Load<TextAsset>(fullPath).ToString();
            serializedLevel = JsonUtility.FromJson<SerializedLevel>(jsonTextFile);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"An error occurred when trying to load map '{name}':" +
                $"\n\t{e}");
            Debug.Log("Creating Empty level");
            serializedLevel = new SerializedLevel();
            serializedLevel.SetPlot(new SerializedPlot(new SerializedMapObjects(),
                JsonUtility.FromJson<SerializedGrid>(File.ReadAllText(this.directory + "_defaultGrid.json"))));
        }
        Debug.Log("Loading " + fullPath);
        m_plotIO.LoadPlot(serializedLevel.serializedPlot);
        //Animals loaded after map to avoid path finding issues
        this.PresetMap = serializedLevel;
        Reload();
        m_cameraController?.UpdateBounds(serializedLevel.serializedPlot);
        return serializedLevel;
    }

    public void SaveNotebook(NotebookData data)
    {
        string name = "sz.notebook";
        string fullPath = Path.Combine(Application.persistentDataPath, name);


        try
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, name), json);
        }
        catch
        {
            Debug.LogError("Serialization error, NOT saved to protect existing saves");
            return;
        }
    }

    public NotebookData LoadNotebook()
    {
        string name = "sz.notebook";
        string fullPath = Path.Combine(Application.persistentDataPath, name);
        try
        {
            string json = File.ReadAllText(fullPath);
            NotebookData data = new NotebookData(NotebookUI.Config);
            JsonUtility.FromJsonOverwrite(json, data);
            return data;
        }
        catch (System.Exception e)
        {
            Debug.Log("No save data or error loading notebook data, creating new data");
            return null;
        }
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
            this.FoodSources.Add(foodSource.ID, foodSource);
        }

        // set the animal dictionary
        foreach (AnimalSpecies animalSpecies in m_levelData.AnimalSpecies)
        {
            if (AnimalSpecies.ContainsKey(animalSpecies.ID)) continue;
            this.AnimalSpecies.Add(animalSpecies.ID, animalSpecies);
        }
        LoadMap();

        if (FoodQualityVFXHandler.Instance != null)
        {
            FoodQualityVFXHandler.Instance.UpdateSpeciesList();
        }
        
        else
        {
            Debug.LogError("FoodQualityVFXHandler Instance not found");
        }
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
        m_tileDataController = FindObjectOfType<TileDataController>();
        m_enclosureSystem = FindObjectOfType<EnclosureSystem>();
        m_inspector = FindObjectOfType<Inspector>();
        m_playerController = FindObjectOfType<PlayerController>();
        m_cameraController = FindObjectOfType<CameraController>();
        m_menuManager = FindObjectOfType<MenuManager>();
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
        m_resourceManager.Initialize();
    }

    private void InitializeUI()
    {
        // If notebook is opened, then close the build ui
        notebookUI.OnNotebookToggle.AddListener(notebookIsOn =>
        {
            if (notebookIsOn) m_menuManager.SetStoreIsOn(false);

            // Set npc active only if both notebook and build ui are not open
            m_dialogueManager.SetNPCActive(!notebookUI.IsOpen && !m_menuManager.IsInStore);
        });
        notebookUI.OnEnableInspectorToggle.AddListener(inspectorEnabled => 
        {
            inspectorObjectiveUI.SetIsOpen(inspectorEnabled);
        });
        // If store is opened, then close the notebook
        m_menuManager.OnStoreToggled.AddListener(storeIsOn =>
        {
            if (storeIsOn) notebookUI.SetIsOpen(false);

            // Set npc active only if both notebook and build ui are not open
            m_dialogueManager.SetNPCActive(!notebookUI.IsOpen && !m_menuManager.IsInStore);
        });
    }

    private void SetupObjectives()
    {
        isObjectivePanelOpen = true;

        maxDay = LevelData.LevelObjectiveData.numberOfDays;

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
        EventManager.Instance.SubscribeToEvent(EventType.NewPopulation, (eventData) =>
        {
            Population population = (Population)eventData;
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
        // Game Manger no longer hanldes npc end conversation, that's the GameOverController's job
        // EventManager.Instance.SubscribeToEvent(EventType.GameOver, HandleNPCEndConversation);
        this.RestartButton.onClick.AddListener(() => { this.SceneNavigator.LoadLevel(this.SceneNavigator.RecentlyLoadedLevel); });
        this.NextLevelButton?.onClick.AddListener(() => { this.SceneNavigator.LoadLevelMenu(); });
        UpdateDayText(currentDay);
        this.IsPaused = false;
    }
    #endregion

    #region Balance Functions
    public void AddToBalance(float value)
    {
        this.Balance += value;
    }

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
    public void UpdateAccessMap()
    {
        m_reservePartitionManager.UpdateAccessMapChangedAt(m_tileDataController.ChangedTiles.ToList<Vector3Int>());
    }
    #endregion

    #region Game State Functions

    public void TryToPause(string pauseID)
    {
        // prevents accidentally unpausing when should not (two different accessors)
        if(m_pauseStack.Add(pauseID))
        {
            if (m_pauseStack.Count == 1)
                this.Pause();
        }
    }

    public void TryToUnpause(string pauseID)
    {
        // prevents accidentally pausing when should not (two different accessors)
        if (m_pauseStack.Remove(pauseID))
        {
            if (m_pauseStack.Count == 0)
                this.Unpause();
        }
    }

    public void TogglePause(string pauseID)
    {
        if (this.IsPaused)
        {
            this.TryToUnpause(pauseID);
        }
        else
        {
            this.TryToPause(pauseID);
        }
    }

    private void Pause()
    {
        Time.timeScale = 1;
        this.IsPaused = true;
        foreach (Population population in m_populationManager.Populations)
            population.PauseAnimalsMovementController();
        m_tileDataController.UpdateAnimalCellGrid();
        AudioManager.instance?.PlayOneShot(SFXType.Pause);
    }

    private void Unpause()
    {
        this.IsPaused = false;
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
        if (currentDay > maxDay)
        {
            m_dialogueManager.SetNewDialogue(m_levelData.RestartConversation);
        }
        else
        {
            m_levelData.Ending.SayEndingConversation();
        }
        m_dialogueManager.StartInteractiveConversation();
        this.IngameUI.SetActive(false);
    }

    public void HandleExitLevel()
    {
        m_tileDataController.SetGridOverlay(false);
        SaveNotebook(NotebookUI.Data);
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

    public void DebugWin()
    {
        isMainObjectivesCompleted = true;

        DebugGameOver();
    }

    public void DebugGameOver()
    {
        this.m_isGameOver = true;

        EventManager.Instance.InvokeEvent(EventType.MainObjectivesCompleted, null);

        // GameOver.cs listens for the event and handles gameover
        EventManager.Instance.InvokeEvent(EventType.GameOver, null);

        Debug.Log($"Level Completed!");
    }

    public void TurnObjectivePanelOn()
    {
        this.isObjectivePanelOpen = true;
        this.objectivePane.SetActive(true);
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

    private void CheckWinConditions()
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

            EventManager.Instance.InvokeEvent(EventType.MainObjectivesCompleted, null);

            // GameOver.cs listens for the event and handles gameover
            EventManager.Instance.InvokeEvent(EventType.GameOver, null);

            Debug.Log($"Level Completed!");
        }
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
        string displayText = "";

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
        m_tileDataController.CountDown();
        m_populationManager.UpdateAccessibleLocations();
        m_populationManager.UpdateAllPopulationRegistration();
        Needs.RebuildIfDirty();
        // Handle growth for all populations
        bool anyPopulationChange = false;
        for (int i = m_populationManager.Populations.Count - 1; i >= 0; i--)
        {
            anyPopulationChange |= m_populationManager.Populations[i].HandleGrowth();
        }

        // Rebuild population cache after population levels change
        // NOTE: this assumes population changes do not affect food sources.
        // Make sure to change this if that assumption is no longer true
        if (anyPopulationChange)
        {
            Needs.RebuildPopulationCache();
        }
        m_inspector.UpdateCurrentDisplay();
        AudioManager.instance?.PlayOneShot(SFXType.NextDay);

        UpdateDayText(++currentDay);
        CheckWinConditions();

        // Invoke next day event
        EventManager.Instance.InvokeEvent(EventType.NextDay, null);

        if (currentDay > maxDay)
        {
            Debug.Log("Time is up!");
            // GameOver.cs listens for the event and handles gameover
            EventManager.Instance.InvokeEvent(EventType.GameOver, null);
        }
    }

    public void EnableInspectorToggle(bool enabled)
    {
        InspectorToggle.interactable = enabled;
        if (!enabled)
        {
            InspectorToggle.isOn = false;
            ObjectiveToggle.isOn = true;
        }
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
