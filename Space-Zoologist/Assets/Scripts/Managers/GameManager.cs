using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    #region Managers
    public NeedSystemManager m_needSystemManager { get; private set; }
    public PopulationManager m_populationManager { get; private set; }
    [SerializeField] private PlotIO m_plotIO;
    
    #endregion

    #region Monobehaviour Callbacks
    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        // load everything from resources first
        LoadLevelData();
        LoadResources();
        // load the plot
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

    private void LoadResources()
    {
        // load everything from resources folder here and I mean EVERYTHING
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
