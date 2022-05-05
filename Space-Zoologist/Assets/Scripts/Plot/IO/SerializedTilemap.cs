using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedTilemap
{
    public string TilemapName;
    public SerializedTileData[] SerializedTileDatas;
    public SerializedLiquidBody[] SerializedLiquidBodies;
    public SerializedTilemap (string tilemapName, TileData[,] tiles, int width, int height, List<LiquidBody> liquidBodies)
    {
        this.TilemapName = tilemapName;

        // figure out a way to parse all the tilemap information here
        List<SerializedTileData> serializedTileDataList = new List<SerializedTileData>();

        // read first tile for reference
        TileData tileData = tiles[0, 0];
        GameTile gameTile = tileData.currentTile;
        bool placeable = tileData.isTilePlaceable;
        Vector3Int prevTilePos = tileData.tilePosition;
        int repetitions = 1;

        // loop through tilemap to parse information
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                // ignore first tile because already read
                if (!(i == 0 && j == 0))
                {
                    tileData = tiles[i, j];

                    if (tileData.currentTile == null) {
                        tileData.currentTile = new GameTile();
                        tileData.currentTile.TileName = null;
                        tileData.currentTile.type = TileType.Air;
                    }

                    // if tile does not match current
                    if (tileData.currentTile == null || gameTile.type != tileData.currentTile.type || placeable != tileData.isTilePlaceable)
                    {
                        // create and add a new serialized tile data
                        int liquidBodyID = -1;
                        if (gameTile.type == TileType.Liquid) {
                            foreach (LiquidBody l in liquidBodies) {
                                if (l.ContainsTile(prevTilePos)) {
                                    liquidBodyID = l.bodyID;
                                }
                            }
                            //Debug.Log("should be " + liquidBodyID);
                            if (liquidBodyID == -1) {
                                Debug.LogError ("failed to save at " + prevTilePos.x + ", " + prevTilePos.y);
                            }
                        }
                        // TODO: LOOK HERE
                        //Debug.Log("before " + liquidBodyID);
                        SerializedTileData serializedTileData = new SerializedTileData(gameTile, liquidBodyID, placeable, repetitions);
                        //Debug.Log("after " + serializedTileData.LiquidBodyID);
                        serializedTileDataList.Add(serializedTileData);
                        //Debug.Log("stored " + serializedTileDataList[serializedTileDataList.Count - 1].LiquidBodyID);

                        // update other values for future matching
                        gameTile = tileData.currentTile;
                        placeable = tileData.isTilePlaceable;
                        prevTilePos = tileData.tilePosition;
                        repetitions = 1;
                    }
                    else {
                        ++repetitions;
                    }
                }
            }
        }

        foreach (SerializedTileData data in serializedTileDataList) {
            if (data.TileID == 6) {
                Debug.Log(data.TileID + " is in pool " + data.LiquidBodyID);
            }
        }

        // add in the last set of tile data
        int lastLiquidBodyID = -1;
        foreach (LiquidBody l in liquidBodies) {
            if (l.ContainsTile(tileData.tilePosition)) {
                lastLiquidBodyID = l.bodyID;
            }
        }
        SerializedTileData lastSerializedTileData = new SerializedTileData(gameTile, lastLiquidBodyID, placeable, repetitions);
        serializedTileDataList.Add(lastSerializedTileData);

        // turn list into array
        SerializedTileDatas = serializedTileDataList.ToArray();

        int index = 0;
        this.SerializedLiquidBodies = new SerializedLiquidBody[liquidBodies.Count];
        foreach (LiquidBody liquidBody in liquidBodies)
        {
            this.SerializedLiquidBodies[index] = liquidBody.Serialize();
            index++;
        }
    }
}
