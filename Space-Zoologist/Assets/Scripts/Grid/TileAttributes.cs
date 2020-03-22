using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileAttributes : MonoBehaviour
{
    public Dictionary<Vector3Int, float[]> tileContents = new Dictionary<Vector3Int, float[]>();
    private Dictionary<Vector3Int, float[]> changedAttributes = new Dictionary<Vector3Int, float[]>();
    private Dictionary<Vector3Int, float[]> addedAttributes = new Dictionary<Vector3Int, float[]>();
    private Tilemap tilemap;
    private List<Vector3Int> neighborTiles = new List<Vector3Int>();
    private bool isPlacedTileNew;
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
            switch (GetNeighborTileStatus(cellLocation, tile))
            {
                case neighborTileStatus.None:
                    isPlacedTileNew = true;
                    tileContents.Add(cellLocation,new float[]{ 0,0,0 });
                    if (!addedAttributes.ContainsKey(cellLocation))
                    {
                        addedAttributes.Add(cellLocation, tileContents[cellLocation]);
                    }
                    ChangeColor(cellLocation, tile);
                    return;
                case neighborTileStatus.Same:
                    isPlacedTileNew = false;
                    foreach (Vector3Int location in addedTiles)
                    {
                        tileContents[location] = tileContents[neighborTiles.First()];
                        if (!addedAttributes.ContainsKey(location))
                        {
                            addedAttributes.Add(location, tileContents[location]);
                        }
                        ChangeColor(location, tile);
                    }
                    neighborTiles = new List<Vector3Int>();
                    break;
                case neighborTileStatus.Different:
                    isPlacedTileNew = false;
                    neighborTiles = new List<Vector3Int>();
                    GetNeighborCellLocations(cellLocation, tile, addedTiles);
                    neighborTiles.Remove(cellLocation);
                    Dictionary<float[], int> neighborTileContents = new Dictionary<float[], int>();
                    foreach (Vector3Int tileLocation in neighborTiles)
                    {
                        float[] contents = tileContents[tileLocation];
                        if (!changedAttributes.ContainsKey(tileLocation))
                        {
                            changedAttributes.Add(tileLocation, contents);
                        }
                        if (neighborTileContents.ContainsKey(contents))
                        {
                            neighborTileContents[contents] += 1;
                        }
                        else
                        {
                            neighborTileContents.Add(contents, 1);
                        }
                    }
                    float[] averageContentValues = new float[neighborTileContents.Keys.First().Length];
                    for (int i = 0; i < neighborTileContents.Keys.First().Length; i++)
                    {
                        float totalValue = 0;
                        int totalFrequency = 0;
                        foreach (KeyValuePair<float[], int> keyValuePair in neighborTileContents)
                        {
                            totalValue += keyValuePair.Key[i] * (float)keyValuePair.Value;
                            totalFrequency += keyValuePair.Value;
                        }
                        averageContentValues[i] = totalValue / (float)totalFrequency;
                    }
                    foreach (Vector3Int tileLocation in neighborTiles)
                    {
                        tileContents[tileLocation] = averageContentValues;
                        ChangeColor(tileLocation, tile);
                    }
                    foreach (Vector3Int tileLocation in addedTiles)
                    {
                        tileContents[tileLocation] = tileContents[neighborTiles.First()];
                        ChangeColor(tileLocation, tile);
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
        changedAttributes.Add(cellLocation, tileContents[cellLocation]);
        tileContents.Remove(cellLocation);
    }
    public void Revert()
    {
        foreach(KeyValuePair<Vector3Int,float[]> keyValuePair in changedAttributes)
        {
            tileContents[keyValuePair.Key] = keyValuePair.Value;
            tilemap.SetTileFlags(keyValuePair.Key, TileFlags.None);
            tilemap.SetColor(keyValuePair.Key, RYBConverter.ToRYBColor(tileContents[keyValuePair.Key]));
        }
        foreach (KeyValuePair<Vector3Int, float[]> keyValuePair in addedAttributes)
        {
            tileContents.Remove(keyValuePair.Key);
        }
    }
    private void ChangeColor(Vector3Int cellLocation, TerrainTile tile)
    {
        if (tile.isChangingColor)
        {
            tilemap.SetTileFlags(cellLocation, TileFlags.None);
            Color color = tile.GetTileColor(tileContents[cellLocation]);
            tilemap.SetColor(cellLocation, color);
        }
    }
    private neighborTileStatus GetNeighborTileStatus(Vector3Int cellLocation, TerrainTile tile)
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
                if (isPlacedTileNew && addedAttributes.ContainsKey(tileToCheck))
                {
                    continue;
                }
                neighborTiles.Add(tileToCheck);
                attributesToCheck.Add(tileContents[tileToCheck]);
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
    public void ConfirmMerge(TerrainTile tile)
    {
        if(isPlacedTileNew)
        {
            float n0 = (float)Random.Range(0, 100) / 100;
            float n1 = (float)Random.Range(0, 100) / 100;
            float n2 = (float)Random.Range(0, 100) / 100;
            foreach(Vector3Int tileLocation in addedAttributes.Keys)
            {
                tileContents[tileLocation] = new float[] { n0, n1, n2 };
                ChangeColor(tileLocation, tile);
                //TODO call user to enter parameters
            }
            isPlacedTileNew = false;
        }
        changedAttributes = new Dictionary<Vector3Int, float[]>();
        addedAttributes = new Dictionary<Vector3Int, float[]>();
    }
    public void RefreshAllColors()
    {
        foreach (Vector3Int tileLocation in tilemap.cellBounds.allPositionsWithin)
        {
            TerrainTile tile = (TerrainTile)tilemap.GetTile(tileLocation);
            ChangeColor(tileLocation, tile);
        }
    }

    public void Debug_AssignRandomContentsToTiles()
    {

    }
}
