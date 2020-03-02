using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GameTile : RuleTile
{
    public enum TileType { Grass, Stone, Dirt, Sand, Wall }
    public TileType tileType = default;
}
