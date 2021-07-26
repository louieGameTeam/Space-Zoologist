using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Dirt, Grass, Stone, Sand, Swamp, Wall, Liquid, TypesOfTiles };
[CreateAssetMenu]
[System.Serializable]
public class GameTile : Tile
{
	public string TileName;
	public TileType type;
	public float[] defaultContents = null;
}