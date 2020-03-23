﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassColorManager : TileColorManager
{
    [SerializeField] private TerrainTile dirt;
    [SerializeField] private TerrainTile sand;
    [SerializeField] private TerrainTile liquid;
    public float[] gasComposition = new float[] { 0.5f, 0.2f, 0.3f };
    private float[] colorShitfDirt = new float[] { 0, 0.1f, 0 };
    private float[] colorShitfSand = new float[] { 0, -0.2f, -0.1f };

/*    public override void SetTileColor(float[] composition, Vector3Int cellLocation, TerrainTile tile)
    {
        composition = gasComposition;
        float distance = tileSystem.DistanceToClosestTile(cellLocation, liquid, affectedRange);
        if (distance == -1)
        {
            float[] newRYBValues = gasComposition;
            if (tileSystem.GetTerrainTileAtLocation(cellLocation) == dirt)
            {
                for (int i = 0; i < 3; i++)
                {
                    newRYBValues[i] += colorShitfDirt[i];
                }
            }
            if (tileSystem.GetTerrainTileAtLocation(cellLocation) == sand)
            {
                for (int i = 0; i < 3; i++)
                {
                    newRYBValues[i] += colorShitfSand[i];
                }
            }
            base.SetTileColor(newRYBValues, cellLocation, tile);
        }
        else
        {
            float[] newRYBValues = gasComposition;
            if (tileSystem.GetTerrainTileAtLocation(cellLocation) == dirt)
            {
                for (int i = 0; i < 3; i++)
                {
                    newRYBValues[i] += colorShitfDirt[i];
                }
            }
            if (tileSystem.GetTerrainTileAtLocation(cellLocation) == sand)
            {
                for (int i = 0; i < 3; i++)
                {
                    newRYBValues[i] += colorShitfSand[i];
                }
            }
            Color baseColor = RYBConverter.ToRYBColor(newRYBValues);
            Color finalColor = new Color();
            float liquidChannelRed = 0;
            float liquidChannelGreen = 0;
            float liquidChannelBlue = 0;
            List<Vector3Int> liquidTileLocations = tileSystem.CellLocationsOfClosestTiles(cellLocation, tile, affectedRange);
            foreach(Vector3Int liquidTile in liquidTileLocations)
            {
                Tilemap targetTilemap = tilePlacementController.tilemapList[(int)liquid.tileLayer];
                Color color = targetTilemap.GetColor(liquidTile);
                liquidChannelRed += color.r;
                liquidChannelGreen += color.g;
                liquidChannelBlue += color.b;
            }
            int channelCount = liquidTileLocations.Count;
            finalColor.r = (baseColor.r * (distance - 1) + liquidChannelRed / channelCount) / affectedRange / Mathf.Sqrt(2);
            finalColor.g = (baseColor.g * (distance - 1) + liquidChannelGreen / channelCount) / affectedRange / Mathf.Sqrt(2);
            finalColor.b = (baseColor.b * (distance - 1) + liquidChannelBlue / channelCount) / affectedRange / Mathf.Sqrt(2);
            Debug.Log("fuck");
            tilemap.SetTileFlags(cellLocation, TileFlags.None);
            tilemap.SetColor(cellLocation, finalColor);
        }
    }*/
}
