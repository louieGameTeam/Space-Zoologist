using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlotIO : MonoBehaviour
{
    [SerializeField] LevelIO levelIO = default;
    private TilePlacementController tilePlacementController;
    private TileLayerManager[] tileLayerManagers;
    private List<GridObjectManager> gridObjectManagers = new List<GridObjectManager>();
    private SerializedPlot SerializedPlot;
    // Start is called before the first frame update
    public void Initialize()
    {
        this.tilePlacementController = this.gameObject.GetComponent<TilePlacementController>();
        this.tileLayerManagers = this.tilePlacementController.gameObject.GetComponentsInChildren<TileLayerManager>();
        this.ParseSerializedObjects();
    }
    public SerializedPlot SavePlot()
    {
        SerializedGrid serializedGrid =  new SerializedGrid(this.tileLayerManagers);
        SerializedMapObjects serializedMapObjects = new SerializedMapObjects();
        foreach (GridObjectManager gridObjectManager in this.gridObjectManagers)
        {
            gridObjectManager.Serialize(serializedMapObjects);
        }
        return new SerializedPlot(serializedMapObjects, serializedGrid);
    }
    public void LoadPlot(SerializedPlot serializedPlot)
    {
        //Debug.Log(serializedPlot.serializedMapObjects.names);
        this.SerializedPlot = serializedPlot;
    }
    public void ParseSerializedObjects()
    {
        foreach (SerializedTilemap serializedTilemap in this.SerializedPlot.serializedGrid.serializedTilemaps)
        {
            bool tilemapFound = false;
            foreach (TileLayerManager tileLayerManager in this.tileLayerManagers)
            {
                if (tileLayerManager.gameObject.name.Equals(serializedTilemap.TilemapName))
                {
                    tileLayerManager.ParseSerializedTilemap(serializedTilemap, this.tilePlacementController.gameTiles);
                    tilemapFound = true;
                    // Debug.Log("Loaded map from resources");
                    break;
                }
            }

            if (!tilemapFound)
            {
                Debug.LogError("Tilemap '" + serializedTilemap.TilemapName + "' was not found");
            }
        }
        List<string> mapObjectNames = new List<string>();
        this.gridObjectManagers = FindObjectsOfType<GridObjectManager>().ToList();
        foreach (GridObjectManager gridObjectManager in this.gridObjectManagers)
        {
            gridObjectManager.Store(this.SerializedPlot.serializedMapObjects);
            mapObjectNames.Add(gridObjectManager.MapObjectName);
        }
        // Notify if a saved object is not been serialized
        if (this.SerializedPlot.serializedMapObjects.names == null)
        {
            return;
        }
        foreach (string name in this.SerializedPlot.serializedMapObjects.names)
        {
            if (!mapObjectNames.Contains(name))
            {
                Debug.LogError("Map object set '" + name + "' is not found in the map object managers.");
            }
        }
    }

    public void ReloadGridObjectManagers()
    {
        foreach (GridObjectManager gridObjectManager in this.gridObjectManagers)
        {
            gridObjectManager.Parse();
        }
    }
}
