using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    #region Level Data
    [Expandable, SerializeField] private LevelData m_levelData;
    public LevelData LevelData { get { return m_levelData; } }
    public Dictionary<string, FoodSourceSpecies> FoodSources = new Dictionary<string, FoodSourceSpecies>();
    public Dictionary<string, AnimalSpecies> AnimalSpecies = new Dictionary<string, AnimalSpecies>();
    public float Balance { get; private set; }
    [SerializeField] private string directory = "Levels/";
    private string sceneName;
    public SerializedLevel presetMap { get; private set; }
    #endregion

    #region Need System Variables
    private Dictionary<NeedType, NeedSystem> m_needSystems;
    public Dictionary<NeedType, NeedSystem> NeedSystems { get { return m_needSystems; } }
    #endregion

    #region Managers
    public ReservePartitionManager m_reservePartitionManager { get; private set; }
    public FoodSourceManager m_foodSourceManager { get; private set; }
    public PopulationManager m_populationManager { get; private set; }
    public ResourceManager m_resourceManager { get; private set; }
    public BuildBufferManager m_buildBufferManager { get; private set; }
    public PauseManager m_pauseManager { get; private set; }
    public TilePlacementController m_tilePlacementController { get; private set; }
    public PlotIO m_plotIO { get; private set; }
    public GridSystem m_gridSystem { get; private set; }
    #endregion

    #region Monobehaviour Callbacks
    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
        
        SetManagers();
        LoadResources();
        SetNeedSystems();
        LoadLevelData();
        InitializeManagers();
        InitialNeedSystemUpdate();
    }

    void Update()
    {

    }
    #endregion

    #region Loading Functions
    public void SaveMap(string name = null, bool preset = true)
    {
        name = name ?? this.sceneName;
        name = name + ".json";
        string fullPath = preset ? this.directory + name : Path.Combine(Application.persistentDataPath, name);

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
        name = name ?? this.sceneName;
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
        m_plotIO.LoadPlot(serializedLevel.serializedPlot);
        //Animals loaded after map to avoid path finding issues
        this.presetMap = serializedLevel;
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
                    if (item.Type.Equals(ItemType.Food) && item.ID.Equals(foodSource.SpeciesName))
                    {
                        this.FoodSources.Add(item.ID, foodSource);
                    }
                }
            }
        }

        // set the animal dictionary
        foreach (AnimalSpecies animalSpecies in m_levelData.AnimalSpecies)
            this.AnimalSpecies.Add(animalSpecies.SpeciesName, animalSpecies);

        // load in the tilemap
        this.sceneName = SceneManager.GetActiveScene().name;
        LoadMap();
    }

    private void SetManagers()
    {
        // add the references to the managers here
        // temporary find function until scene reorganization

        // no longer necessary
        // m_needSystemManager = FindObjectOfType<NeedSystemManager>();

        m_reservePartitionManager = FindObjectOfType<ReservePartitionManager>();
        m_foodSourceManager = FindObjectOfType<FoodSourceManager>();
        m_populationManager = FindObjectOfType<PopulationManager>();
        m_resourceManager = FindObjectOfType<ResourceManager>();
        m_buildBufferManager = FindObjectOfType<BuildBufferManager>();
        m_pauseManager = FindObjectOfType<PauseManager>();
        m_tilePlacementController = FindObjectOfType<TilePlacementController>();
        m_plotIO = FindObjectOfType<PlotIO>();
        m_gridSystem = FindObjectOfType<GridSystem>();
    }

    private void LoadResources()
    {
        // load everything from resources folder here and I mean EVERYTHING
        m_tilePlacementController.LoadResources();
        m_foodSourceManager.LoadResources();
    }

    private void InitializeManagers()
    {
        m_foodSourceManager.Initialize();
        m_buildBufferManager.Initialize();
        m_resourceManager.Initialize();
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
    }

    private void InitialNeedSystemUpdate()
    {
        this.UpdateAllNeedSystems();
        m_populationManager.UpdateAllGrowthConditions();
        m_pauseManager.TogglePause();
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

    #region Other Functions
    // no idea what these two do
    public void ClearAnimals()
    {
        SerializedLevel level = new SerializedLevel();
        level.SetPopulations(m_populationManager);
        level.SetPlot(m_plotIO.SavePlot());
        level.serializedPopulations = new SerializedPopulation[0];
        this.presetMap = level;

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
