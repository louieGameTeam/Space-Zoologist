using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDesignerDataFileOperations : MonoBehaviour
{

    private TileDataController tileDataController;

    private void Start()
    {
        tileDataController = GameManager.Instance.m_tileDataController;
    }

    // Perform operations on the raw file since updating through gameplay systems is convulated
    public void ShiftMapUp()
    {
        var map = GameManager.Instance.GetSerializedMap();
        var plot = map.serializedPlot;
        ShiftFoodAndPopulations(map, new Vector3Int(0, 1, 0));
        // Move tiles ( a bit of hard coding, should fix later )
        plot.serializedGrid.serializedTilemap.SerializedTileDatas = ShiftUp(plot.serializedGrid, 1);

        GameManager.Instance.LoadMap(map);
    }
    public void ShiftMapRight()
    {
        var map = GameManager.Instance.GetSerializedMap();
        var plot = map.serializedPlot;
        ShiftFoodAndPopulations(map, new Vector3Int(1, 0, 0));
        // Move tiles ( a bit of hard coding, should fix later )
        plot.serializedGrid.serializedTilemap.SerializedTileDatas = ShiftRight(plot.serializedGrid, 1);

        GameManager.Instance.LoadMap(map);
    }

    private void ShiftFoodAndPopulations(SerializedLevel level, Vector3Int shiftValue)
    {
        // Move food
        HandleShiftFoodObjects(level.serializedPlot, shiftValue);
        // Move animals
        HandleShiftPopulations(level.serializedPopulations, shiftValue);
    }

    private void HandleShiftFoodObjects(SerializedPlot plot, Vector3Int shiftValue)
    {
        foreach (var item in plot.serializedMapObjects.gridItemSets)
        {
            for (int i = 0; i < item.coords.Length; i += 3)
            {
                item.coords[i] += shiftValue.x;
                item.coords[i + 1] += shiftValue.y;
            }
        }
    }

    private void HandleShiftPopulations(SerializedPopulation[] populations, Vector3Int shiftValue)
    {
        foreach(var serializedPopulation in populations)
        {
            var coords = serializedPopulation.population.coords;
            for(int i = 0;i < coords.Length;i+=3)
            {
                coords[i] += shiftValue.x;
                coords[i + 1] += shiftValue.y;
                coords[i + 2] += shiftValue.z;
            }
        }
    }

    private SerializedTileData[] ShiftRight(SerializedGrid grid, int dist)
    {
        var tileDatas = grid.serializedTilemap.SerializedTileDatas;
        int width = grid.width;
        int height = grid.height;
        var newTileDatas = new List<SerializedTileData>();
        grid.width += dist;
        int widthCounter = 0;
        int total = 0;
        for (int i = 0; i < tileDatas.Length;)
        {
            newTileDatas.Add(EmptyTile(dist));
            total += dist;
            while (widthCounter < width)
            {
                int diff = width - widthCounter;
                newTileDatas.Add(new SerializedTileData(tileDatas[i]));
                newTileDatas[newTileDatas.Count - 1].Repetitions = Mathf.Min(diff, tileDatas[i].Repetitions);
                total += tileDatas[i].Repetitions;
                widthCounter += tileDatas[i].Repetitions;
                i++;
            }
            // overflow
            if (widthCounter - width > 0)
            {
                i--;
                tileDatas[i].Repetitions = widthCounter - width;
            }
            widthCounter = 0;
        }
        Debug.Log(total);
        return newTileDatas.ToArray();
    }
    private SerializedTileData[] ShiftUp(SerializedGrid grid, int dist)
    {
        var tileDatas = grid.serializedTilemap.SerializedTileDatas;
        int width = grid.width;
        int height = grid.height;
        var newTileDatas = new List<SerializedTileData>();
        grid.height += dist;
        for (int i = 0; i < dist; i++)
            newTileDatas.Insert(0, EmptyTile(width));
        newTileDatas.AddRange(tileDatas);
        return newTileDatas.ToArray();
    }

    private SerializedTileData EmptyTile(int repititions)
    {
        return new SerializedTileData(new GameTile { type = (TileType)(-1) }, -1, false, repititions);
    }
}
