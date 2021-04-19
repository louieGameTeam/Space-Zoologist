using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Temporary Class for testing only, do not integrate
/// </summary>
public class PlotIO : MonoBehaviour
{
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
    public void RegisterManager(GridObjectManager gridObjectManager)
    {
        this.gridObjectManagers.Add(gridObjectManager);
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
                    Debug.Log("Loaded from resources");
                    break;
                }
            }

            if (!tilemapFound)
            {
                Debug.LogError("Tilemap '" + serializedTilemap.TilemapName + "' was not found");
            }
        }
        List<string> mapObjectNames = new List<string>();
        foreach (GridObjectManager gridObjectManager in this.gridObjectManagers)
        {
            gridObjectManager.Parse(this.SerializedPlot.serializedMapObjects);
            mapObjectNames.Add(gridObjectManager.name);
        }
        // Notify if a saved object is not been serialized
        foreach (string name in this.SerializedPlot.serializedMapObjects.names)
        {
            if (!mapObjectNames.Contains(name))
            {
                Debug.LogError("Map object set '" + name + "' is not found in the map object managers.");
            }
        }
    }
}
