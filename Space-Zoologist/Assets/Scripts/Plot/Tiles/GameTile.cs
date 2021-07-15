using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Stone, Sand, Dirt, Grass, Liquid, Wall, Swamp, TypesOfTiles};
[CreateAssetMenu]
[System.Serializable]
public class GameTile : Tile
{
	public Texture[] tileTextures;
	public string TileName;
	public TileType type;
	public float[] defaultContents = null;
}