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
    [SerializeField] private string directory = "Assets/Resources/Grid/";
    private string sceneName;
    private TileLayerManager[] tileLayerManagers;
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
    public void LoadGrid(string name = null)
    {
        name = name ?? this.sceneName;
        name = name + ".json";
        string fullPath = Path.Combine(Application.persistentDataPath, name);
        // Debug.Log("Loading map save: " + fullPath);

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
}
