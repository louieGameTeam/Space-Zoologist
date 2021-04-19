using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
public class LevelIO : MonoBehaviour
{
    private TilePlacementController tilePlacementController;
    [SerializeField] private string directory = "Assets/SaveData/Grid/";
    private string sceneName;
    private TileLayerManager[] tileLayerManagers;
    private PlotIO plotIO;
    private PopulationManager populationManager;
    // Start is called before the first frame update
    public void Start()
    {
        this.tilePlacementController = this.gameObject.GetComponent<TilePlacementController>();
        this.sceneName = SceneManager.GetActiveScene().name;
        this.tileLayerManagers = GetComponentsInChildren<TileLayerManager>();
        this.plotIO = FindObjectOfType<PlotIO>();
        this.populationManager = FindObjectOfType<PopulationManager>();
    }
    public void Save(string name = null)
    {
        name = name ?? this.sceneName;
        string fullPath = this.directory + name + ".json";
        Debug.Log("Saving Grid to " + fullPath);
        if (File.Exists(fullPath))
        {
            Debug.Log("Overwriting file at " + fullPath);
        }
        try
        {
            SerializedLevel level = new SerializedLevel();
            // Serialize plot
            level.SetPlot(this.plotIO.SavePlot());
            // Serialize Animals
            level.SetPopulations(this.populationManager);
        }
        catch
        {
            Debug.LogError("Serialization error, NOT saved to protect existing saves");
            return;
        }
        this.WriteToFile(fullPath);
    }
    private void WriteToFile(string path)
    {
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            streamWriter.Write(JsonUtility.ToJson(new SerializedGrid(this.tileLayerManagers)));
        }
        Debug.Log("Grid Saved to: " + path);
    }
    public SerializedMapObjects SerializeMapObjects()
    {
        SerializedMapObjects serializedMapObjects = new SerializedMapObjects();
        return serializedMapObjects;
    }
    public void Load(string name = null)
    {

    }
}
