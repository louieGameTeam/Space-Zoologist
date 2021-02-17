using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Tilemaps;
/// <summary>
/// Temporary Class for testing only, do not integrate
/// </summary>
public class GridIO : MonoBehaviour
{
    private TilePlacementController tilePlacementController;
    [SerializeField] private string resourceDirectory = "Grid/";
    [SerializeField] private string directory = "Assets/Resources/Grid/";
    private string sceneName;
    private TileLayerManager[] tileLayerManagers;
    private bool initialized = false;
    // Start is called before the first frame update
    public void Initialize()
    {
        this.tilePlacementController = this.gameObject.GetComponent<TilePlacementController>();
        this.sceneName = SceneManager.GetActiveScene().name;
        this.tileLayerManagers = GetComponentsInChildren<TileLayerManager>();
    }
    public void SaveGrid(string name = null)
    {
        name = name ?? this.sceneName;
        name = name + ".json";
        string fullPath = Path.Combine(Application.persistentDataPath, name);
        // Debug.Log("Saving Grid to " + fullPath);
        if (File.Exists(fullPath))
        {
            // Debug.Log("Overwriting file at " + fullPath);
        }
        using (StreamWriter streamWriter = new StreamWriter(fullPath))
        {
            streamWriter.Write(JsonUtility.ToJson(new SerializedGrid(this.tileLayerManagers)));
        }
        // Debug.Log("Grid Saved to: " + fullPath);
    }
    public void SaveAsPresetGrid(string name = null)
    {
        name = name ?? this.sceneName;
        name = name + ".json";
        string fullPath = this.directory + name; // preset map
        // Debug.Log("Saving Grid to " + fullPath);
        if (File.Exists(fullPath))
        {
            // Debug.Log("Overwriting file at " + fullPath);
        }
        using (StreamWriter streamWriter = new StreamWriter(fullPath))
        {
            streamWriter.Write(JsonUtility.ToJson(new SerializedGrid(this.tileLayerManagers)));
        }
        // Debug.Log("Grid Saved to: " + fullPath);
    }
    public void LoadGrid(string name = null)
    {
        name = name ?? this.sceneName;
        string filename = name + ".json";
        string fullPath = Path.Combine(Application.persistentDataPath, filename); // in-game save
        if (!initialized) // in-game save doesn't exist - use preset map instead
        {
            initialized = true;
            LoadPresetGrid(name);
            return;
        }
        Debug.Log("Loading map save: " + fullPath);

        SerializedGrid serializedGrid;
        try
        {
            serializedGrid = JsonUtility.FromJson<SerializedGrid>(File.ReadAllText(fullPath));
        }
        catch
        {
            Debug.LogError("No map save found for this scene, create a map using map designer or check your spelling");
            return;
        }
        foreach (SerializedTilemap serializedTilemap in serializedGrid.serializedTilemaps)
        {
            bool found = false;
            foreach (TileLayerManager tileLayerManager in this.tileLayerManagers)
            {
                if (tileLayerManager.gameObject.name.Equals(serializedTilemap.TilemapName))
                {
                    tileLayerManager.ParseSerializedTilemap(serializedTilemap, this.tilePlacementController.gameTiles);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Debug.LogError("Tilemap '" + serializedTilemap.TilemapName + "' was not found");
            }
        }
    }
    public void LoadPresetGrid(string name = null)
    {
        name = name ?? this.sceneName;
        string fullPath = this.resourceDirectory + name; // preset map
        Debug.Log("Loading map save: " + fullPath);

        SerializedGrid serializedGrid;
        try
        {
            var jsonTextFile = Resources.Load<TextAsset>(fullPath).text;
            serializedGrid = JsonUtility.FromJson<SerializedGrid>(jsonTextFile);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError("No map save found for this scene, create a map using map designer or check your spelling");
            return;
        }
        foreach (SerializedTilemap serializedTilemap in serializedGrid.serializedTilemaps)
        {
            bool found = false;
            foreach (TileLayerManager tileLayerManager in this.tileLayerManagers)
            {
                if (tileLayerManager.gameObject.name.Equals(serializedTilemap.TilemapName))
                {
                    tileLayerManager.ParseSerializedTilemap(serializedTilemap, this.tilePlacementController.gameTiles);
                    found = true;
                    Debug.Log("Loaded from resources");
                    break;
                }
            }
            if (!found)
            {
                Debug.LogError("Tilemap '" + serializedTilemap.TilemapName + "' was not found");
            }
        }
    }
}
