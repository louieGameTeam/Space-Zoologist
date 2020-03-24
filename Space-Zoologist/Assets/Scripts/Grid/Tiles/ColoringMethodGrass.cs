using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ColoringMethodGrass : ColoringMethod
{
    private float[] gasComposition = new float[] { 0.5f, 0.2f, 0.3f };
    private float[] colorShitfDirt = new float[] { 0, 0.3f, 0.3f };
    private float[] colorShitfSand = new float[] { -0.2f, 0.4f, -0.1f };
    public override void SetColor(float[] composition, Vector3Int cellLocation, TerrainTile tile, Tilemap tilemap, List<TerrainTile> managedTiles, List<TerrainTile> linkedTiles, TileSystem tileSystem, TilePlacementController tilePlacementController)
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
        if (distance != -1)
        {
            Dictionary<float[], float> compositionDistancePairs = tileSystem.DistancesToClosestTilesOfEachBody(cellLocation, liquid, affectedRange, true);
            float[] weightedComposition = new float[] { 0, 0, 0 };
            float totalWeight = 1;
            foreach (KeyValuePair<float[], float> compositionDistancePair in compositionDistancePairs)
            {
                float weight = ColorGradient(compositionDistancePair.Value);
                totalWeight += weight;
                for (int i = 0; i < 3; i++)
                {
                    weightedComposition[i] += compositionDistancePair.Key[i] * weight;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                newRYBValues[i] = (newRYBValues[i] + weightedComposition[i]) / totalWeight;
            }
        }
        Color baseColor = RYBConverter.ToRYBColor(newRYBValues);
        baseColor.a = 1;
        tilemap.SetTileFlags(cellLocation, TileFlags.None);
        tilemap.SetColor(cellLocation, baseColor);
    }
    private float ColorGradient (float distance)
    {
        float ratio = (1 - Mathf.Sqrt(distance / affectedRange) - 0.4f);
        if (ratio < 0)
        {
            ratio = 0;
        }
        return ratio;
    }
}
