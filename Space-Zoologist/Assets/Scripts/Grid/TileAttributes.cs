using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileAttributes : MonoBehaviour
{
    public Dictionary<Vector3Int, float[]> keyValuePairs = new Dictionary<Vector3Int, float[]>();
    private Dictionary<Vector3Int, float[]> changedAttributes = new Dictionary<Vector3Int, float[]>();
    private Dictionary<Vector3Int, float[]> addedAttributes = new Dictionary<Vector3Int, float[]>();
    private Tilemap tilemap;
    private List<Vector3Int> neighborTiles = new List<Vector3Int>();
    private enum neighborTileStatus
    {
        None,
        Same,
        Different
    }
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }
    public void MergeTile (Vector3Int cellLocation, TerrainTile tile, List<Vector3Int> addedTiles)
    {
        if (tile.isMergingAttributes)
        {
            switch (GetNeighborTileStatus(cellLocation, tile, addedTiles))
            {
                case neighborTileStatus.None:
                    float n0 = (float)Random.Range(0, 100) / 100;
                    float n1 = (float)Random.Range(0, 100) / 100;
                    float n2 = (float)Random.Range(0, 100) / 100;
                    keyValuePairs.Add(cellLocation,new float[]{n0,n1,n2 });
                    if (!addedAttributes.ContainsKey(cellLocation))
                    {
                        addedAttributes.Add(cellLocation, keyValuePairs[cellLocation]);
                    }
                    ChangeColor(cellLocation, tile);
                    return;
                    // call user to enter parameters
                case neighborTileStatus.Same:
                    foreach (Vector3Int location in addedTiles)
                    {
                        keyValuePairs[location] = keyValuePairs[neighborTiles.First()];
                        if (!addedAttributes.ContainsKey(location))
                        {
                            addedAttributes.Add(location, keyValuePairs[location]);
                        }
                        ChangeColor(location, tile);
                    }
                    neighborTiles = new List<Vector3Int>();
                    break;
                case neighborTileStatus.Different:
                    neighborTiles = new List<Vector3Int>();
                    GetNeighborCellLocations(cellLocation, tile, addedTiles);
                    Dictionary<float[], int> Attributes = new Dictionary<float[], int>();
                    neighborTiles.Remove(cellLocation);
                    foreach (Vector3Int location in neighborTiles)
                    {
                        float[] attribute = keyValuePairs[location];
                        if (!changedAttributes.ContainsKey(location))
                        {
                            changedAttributes.Add(location, attribute);
                        }
                        if (Attributes.ContainsKey(attribute))
                        {
                            Attributes[attribute] += 1;
                        }
                        else
                        {
                            Attributes.Add(attribute, 1);
                        }
                    }
                    float[] averageValue = new float[Attributes.Keys.First().Length];
                    for (int i = 0; i < Attributes.Keys.First().Length; i++)
                    {
                        float totalValue = 0;
                        int totalOccurrence = 0;
                        foreach (KeyValuePair<float[], int> keyValuePair in Attributes)
                        {
                            totalValue += keyValuePair.Key[i] * (float)keyValuePair.Value;
                            totalOccurrence += keyValuePair.Value;
                        }
                        averageValue[i] = totalValue / (float)totalOccurrence;
                    }
                    foreach (Vector3Int location in neighborTiles)
                    {
                        keyValuePairs[location] = averageValue;
                        ChangeColor(location, tile);
                    }
                    foreach (Vector3Int location in addedTiles)
                    {
                        keyValuePairs[location] = keyValuePairs[neighborTiles.First()];
                        ChangeColor(location, tile);
                    }
                    neighborTiles = new List<Vector3Int>();
                    break;
                default:
                    return;
            }
        }
    }
    public void RemoveTile (Vector3Int cellLocation)
    {
        changedAttributes.Add(cellLocation, keyValuePairs[cellLocation]);
        keyValuePairs.Remove(cellLocation);
    }
    public void Revert()
    {
        foreach(KeyValuePair<Vector3Int,float[]> keyValuePair in changedAttributes)
        {
            keyValuePairs[keyValuePair.Key] = keyValuePair.Value;
            tilemap.SetTileFlags(keyValuePair.Key, TileFlags.None);
            tilemap.SetColor(keyValuePair.Key, RYBConverter.ToRYBColor(keyValuePairs[keyValuePair.Key]));
        }
        foreach (KeyValuePair<Vector3Int, float[]> keyValuePair in addedAttributes)
        {
            keyValuePairs.Remove(keyValuePair.Key);
        }
    }
    private void ChangeColor(Vector3Int cellLocation, TerrainTile tile)
    {
        if (tile.isChangingColor)
        {
            tilemap.SetTileFlags(cellLocation, TileFlags.None);
            tilemap.SetColor(cellLocation, RYBConverter.ToRYBColor(keyValuePairs[cellLocation]));
        }
    }
    private neighborTileStatus GetNeighborTileStatus(Vector3Int cellLocation, TerrainTile tile, List<Vector3Int> addedTiles)
    {
        Vector3Int posX0 = new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z);
        Vector3Int posX1 = new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z);
        Vector3Int posy0 = new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z);
        Vector3Int posy1 = new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z);
        List<Vector3Int> tilesToCheck = new List<Vector3Int> { posX0, posX1, posy0, posy1 };
        List<float[]> attributesToCheck = new List<float[]>();
        foreach (Vector3Int tileToCheck in tilesToCheck)
        {
            if (tilemap.GetTile(tileToCheck) == tile)
            {
                neighborTiles.Add(tileToCheck);
                attributesToCheck.Add(keyValuePairs[tileToCheck]);
            }
        }
        if (attributesToCheck.Count == 0)
        {
            return neighborTileStatus.None;
        }
        for (int i = 0; i < attributesToCheck.Count - 1; i++)
        {
            if (!attributesToCheck[i].SequenceEqual(attributesToCheck[i + 1]))
            {
                return neighborTileStatus.Different;
            }
        }
        return neighborTileStatus.Same;
    }
    private void GetNeighborCellLocations(Vector3Int cellLocation, TerrainTile tile, List<Vector3Int> addedTiles)
    {
        Vector3Int posX0 = new Vector3Int(cellLocation.x - 1, cellLocation.y, cellLocation.z);
        Vector3Int posX1 = new Vector3Int(cellLocation.x + 1, cellLocation.y, cellLocation.z);
        Vector3Int posy0 = new Vector3Int(cellLocation.x, cellLocation.y - 1, cellLocation.z);
        Vector3Int posy1 = new Vector3Int(cellLocation.x, cellLocation.y + 1, cellLocation.z);
        List<Vector3Int> tilesToCheck = new List<Vector3Int> { posX0, posX1, posy0, posy1 };
        foreach (Vector3Int tileToCheck in tilesToCheck)
        {
            if (
                !neighborTiles.Contains(tileToCheck) && 
                tilemap.GetTile(tileToCheck) == tile)
            {
                neighborTiles.Add(tileToCheck);
                GetNeighborCellLocations(tileToCheck, tile, addedTiles);
            }
        }
    }
    public void RefreshAllColors()
    {
    }
    public void ConfirmMerge ()
    {
        changedAttributes = new Dictionary<Vector3Int, float[]>();
        addedAttributes = new Dictionary<Vector3Int, float[]>();
    }
    public void Debug_AssignRandomValuesToTiles()
    {

    }
}
