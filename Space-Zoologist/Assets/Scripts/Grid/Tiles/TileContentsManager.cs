using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileContentsManager : MonoBehaviour
{
    public Dictionary<Vector3Int, float[]> tileContents = new Dictionary<Vector3Int, float[]>();
    public List<Vector3Int> changedTilesPositions { get { return changedAttributes.Keys.ToList(); } }
    public List<Vector3Int> addedTilePositions { get { return addedAttributes.Keys.ToList(); } }
    private Dictionary<Vector3Int, float[]> changedAttributes = new Dictionary<Vector3Int, float[]>();
    private Dictionary<Vector3Int, float[]> addedAttributes = new Dictionary<Vector3Int, float[]>();
    private Tilemap tilemap;
    private List<Vector3Int> neighborTiles = new List<Vector3Int>();
    private bool isPlacedTileNew;
    private TerrainTile terrainTile;
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
    public void SetCompostion(Vector3Int cellLocation, float[] composition)
    {
        //tileContents[cellLocation] = new float[] { composition[0], composition[1], composition[2] };
        for (int i = 0; i <3; i++)
        {
            tileContents[cellLocation][i] = composition[i];
        }
        TerrainTile tile = (TerrainTile)tilemap.GetTile(cellLocation);
        ChangeColor(cellLocation, null, tile);
    }
    public void ModifyComposition(Vector3Int cellLocation, float[] changeInComposition)
    {
        for (int i = 0; i < changeInComposition.Length; i++)
        {
            if (tileContents[cellLocation][i] + changeInComposition[i] >= 1)
            {
                tileContents[cellLocation][i] = 1;
                continue;
            }
            if (tileContents[cellLocation][i] + changeInComposition[i] <= 0)
            {
                tileContents[cellLocation][i] = 0;
                continue;
            }
            tileContents[cellLocation][i] += changeInComposition[i];
        }
    }
    public void MergeTile (Vector3Int cellLocation, TerrainTile tile, List<Vector3Int> addedTiles)
    {
        terrainTile = tile;
        float[] newcomp = new float[] { 0, 0, 0 };
        if (tile.isMergingAttributes)
        {
            switch (GetNeighborTileStatus(cellLocation))
            {
                case neighborTileStatus.None:
                    isPlacedTileNew = true;
                    if (!tileContents.ContainsKey(cellLocation))
                    {
                        tileContents.Add(cellLocation, newcomp);
                    }
                    if (!addedAttributes.ContainsKey(cellLocation))
                    {
                        addedAttributes.Add(cellLocation, tileContents[cellLocation]);
                    }
                    ChangeColor(cellLocation);
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
                        ChangeColor(location);
                    }
                    neighborTiles = new List<Vector3Int>();
                    break;
                case neighborTileStatus.Different:
                    isPlacedTileNew = false;
                    neighborTiles = new List<Vector3Int>();
                    GetNeighborCellLocations(cellLocation,addedTiles);
                    neighborTiles.Remove(cellLocation);
                    Dictionary<float[], int> neighborTileContents = new Dictionary<float[], int>();
                    foreach (Vector3Int tileLocation in neighborTiles)
                    {/*
                        if(addedAttributes.ContainsKey(tileLocation))
                        {
                            continue;
                        }*/
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
                        ChangeColor(tileLocation);
                    }
                    foreach (Vector3Int tileLocation in addedTiles)
                    {
                        tileContents[tileLocation] = tileContents[neighborTiles.First()];
                        ChangeColor(tileLocation);
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
    public void Restore(Vector3Int cellLocation)
    {
        if (changedAttributes.ContainsKey(cellLocation))
        {
            tileContents[cellLocation] = changedAttributes[cellLocation];
            changedAttributes.Remove(cellLocation);
            ChangeColor(cellLocation);
        }
    }
    public void Revert(List<Vector3Int> boxModeSupposedTiles = null)
    {
        foreach (KeyValuePair<Vector3Int, float[]> keyValuePair in changedAttributes)
        {
            tileContents[keyValuePair.Key] = keyValuePair.Value;
            ChangeColor(keyValuePair.Key);
        }
        foreach (KeyValuePair<Vector3Int, float[]> keyValuePair in addedAttributes)
        {
            tileContents.Remove(keyValuePair.Key);
        }
        changedAttributes = new Dictionary<Vector3Int, float[]>();
        addedAttributes = new Dictionary<Vector3Int, float[]>();
        if (boxModeSupposedTiles != null)
        {
            isPlacedTileNew = true;
            List<Vector3Int> virtualAddedTiles = new List<Vector3Int>();
            foreach (Vector3Int tileInBox in boxModeSupposedTiles)
            {
                if (!tileContents.ContainsKey(tileInBox))
                {
                    virtualAddedTiles.Add(tileInBox);
                    MergeTile(tileInBox, terrainTile, virtualAddedTiles);
                }
            }
        }
    }
    private void ChangeColor(Vector3Int cellLocation, TileColorManager tileColorManager = null , TerrainTile tile = null)
    {
        if(tileColorManager != null)
        {
            tileColorManager.SetTileColor(cellLocation, tile ?? terrainTile);
        }
        else if(tilemap.TryGetComponent(out TileColorManager colorManager))
        {
            colorManager.SetTileColor(cellLocation, tile ?? terrainTile);
        }
    }
    private neighborTileStatus GetNeighborTileStatus(Vector3Int cellLocation)
    {
        List<float[]> attributesToCheck = new List<float[]>();
        foreach (Vector3Int tileToCheck in GridUtils.FourNeighborTiles(cellLocation))
        {
            if (tilemap.GetTile(tileToCheck) == terrainTile && tileContents.ContainsKey(tileToCheck))
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
    private void GetNeighborCellLocations(Vector3Int cellLocation, List<Vector3Int> addedTiles)
    {
        foreach (Vector3Int tileToCheck in GridUtils.FourNeighborTiles(cellLocation))
        {
            if (
                tilemap.GetTile(tileToCheck) == terrainTile &&
                !neighborTiles.Contains(tileToCheck) &&
                tileContents.ContainsKey(tileToCheck))
            {
                neighborTiles.Add(tileToCheck);
                GetNeighborCellLocations(tileToCheck, addedTiles);
            }
        }
    }
    public void ConfirmMerge()
    {
        if(isPlacedTileNew)
        {
            float n0 = (float)Random.Range(0, 100) / 100;
            float n1 = (float)Random.Range(0, 100) / 100;
            float n2 = (float)Random.Range(0, 100) / 100;
            float[] newComp = new float[] { n0, n1, n2 };
            foreach (Vector3Int tileLocation in addedAttributes.Keys)
            {
                tileContents[tileLocation] = newComp;
                ChangeColor(tileLocation);
                //TODO call user to enter parameters

                //Debug.Log($"Placed liquid {n0}:{n1}:{n2}");
            }
            isPlacedTileNew = false;
        }
        foreach (KeyValuePair<Vector3Int,float[]> keyValuePair in addedAttributes)
        {
            if (!addedAttributes.ContainsKey(keyValuePair.Key))
            {
                addedAttributes.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        foreach (KeyValuePair<Vector3Int, float[]> keyValuePair in changedAttributes)
        {
            if (!changedAttributes.ContainsKey(keyValuePair.Key))
            {
                changedAttributes.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        changedAttributes = new Dictionary<Vector3Int, float[]>();
        addedAttributes = new Dictionary<Vector3Int, float[]>();
    }
    public void RefreshAllColors()
    {
        if (tilemap.TryGetComponent(out TileColorManager tileColorManager))
        {
            foreach (Vector3Int tileLocation in tilemap.cellBounds.allPositionsWithin)
            {
                if (tileContents.ContainsKey(tileLocation))
                {
                    ChangeColor(tileLocation, tileColorManager);
                }
            }
        }
    }
}
