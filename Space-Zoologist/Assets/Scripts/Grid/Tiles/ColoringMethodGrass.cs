using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColoringMethodGrass : ColoringMethod
{
    private float[] gasComposition = new float[] { 0.5f, 0.2f, 0.3f };
    private float[] colorShitfDirt = new float[] { 0, 0.3f, 0.3f };
    private float[] colorShitfSand = new float[] { -0.2f, 0.4f, -0.1f };
    public override void SetTileColor(float[] composition, Vector3Int cellLocation, TerrainTile tile, Tilemap tilemap, List<TerrainTile> managedTiles, List<TerrainTile> linkedTiles, TileSystem tileSystem, TilePlacementController tilePlacementController)
    {
        TerrainTile liquid = linkedTiles[0];
        TerrainTile dirt = linkedTiles[1];
        TerrainTile sand = linkedTiles[2];
        float distance = tileSystem.DistanceToClosestTile(cellLocation, liquid, affectedRange);
        float[] newRYBValues = new float[] { 0, 0, 0 };
        for (int i = 0; i < 3; i++)
        {
            newRYBValues[i] += gasComposition[i];
        }
        if (tileSystem.TileExistsAtLocation(cellLocation, dirt))
        {
            for (int i = 0; i < 3; i++)
            {
                newRYBValues[i] += colorShitfDirt[i];
            }
        }
        if (tileSystem.TileExistsAtLocation(cellLocation, sand))
        {
            for (int i = 0; i < 3; i++)
            {
                newRYBValues[i] += colorShitfSand[i];
            }
        }
        Color baseColor = RYBConverter.ToRYBColor(newRYBValues);
        if (distance == -1)
        {
            baseColor.a = 1;
            tilemap.SetTileFlags(cellLocation, TileFlags.None);
            tilemap.SetColor(cellLocation, baseColor);
        }
        else
        {
            Color finalColor = new Color();
            float liquidChannelRed = 0;
            float liquidChannelGreen = 0;
            float liquidChannelBlue = 0;
            List<Vector3Int> liquidTileLocations = tileSystem.CellLocationsOfClosestTiles(cellLocation, liquid, affectedRange);
            foreach (Vector3Int liquidTile in liquidTileLocations)
            {
                Tilemap targetTilemap = tilePlacementController.tilemapList[(int)liquid.tileLayer];
                Color color = targetTilemap.GetColor(liquidTile);
                liquidChannelRed += color.r;
                liquidChannelGreen += color.g;
                liquidChannelBlue += color.b;
            }
            int channelCount = liquidTileLocations.Count;
            float ratio = Mathf.Sqrt(distance / (affectedRange * Mathf.Sqrt(2)));
            finalColor.r = baseColor.r * ratio + liquidChannelRed / channelCount * (1 - ratio);
            finalColor.g = baseColor.g * ratio + liquidChannelGreen / channelCount * (1 - ratio);
            finalColor.b = baseColor.b * ratio + liquidChannelBlue / channelCount * (1 - ratio);
            finalColor.a = 1;
            tilemap.SetTileFlags(cellLocation, TileFlags.None);
            tilemap.SetColor(cellLocation, finalColor);
        }
    }
}
