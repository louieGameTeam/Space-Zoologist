﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedTilemap
{
    public string TilemapName;
    public SerializedTileData[] SerializedTileDatas;
    public SerializedLiquidBody[] SerializedLiquidBodies;
    public SerializedTilemap (string tilemapName, TileData[,] tiles, int width, int height, HashSet<LiquidBody> liquidBodies)
    {
        this.TilemapName = tilemapName;

        // figure out a way to parse all the tilemap information here
        List<SerializedTileData> serializedTileDataList = new List<SerializedTileData>();

        // read first tile for reference
        TileData tileData = tiles[0, 0];
        GameTile gameTile = tileData.currentTile;
        LiquidBody body = tileData.currentLiquidBody;
        bool placeable = tileData.isTilePlaceable;
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

                    // if tile does not match current
                    if (gameTile != tileData.currentTile || placeable != tileData.isTilePlaceable)
                    {
                        // create and add a new serialized tile data
                        SerializedTileData serializedTileData = new SerializedTileData(gameTile, body, placeable, repetitions);
                        serializedTileDataList.Add(serializedTileData);

                        // update other values for future matching
                        gameTile = tileData.currentTile;
                        body = tileData.currentLiquidBody;
                        placeable = tileData.isTilePlaceable;
                        repetitions = 1;
                    }
                    else
                        ++repetitions;
                }
            }
        }

        // add in the last set of tile data
        SerializedTileData lastSerializedTileData = new SerializedTileData(gameTile, body, placeable, repetitions);
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
