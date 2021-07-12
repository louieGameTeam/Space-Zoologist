using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Stone, Sand, Dirt, Grass, Liquid, Wall, Swamp, Placable, TypesOfTiles };
[CreateAssetMenu]
[System.Serializable]
public class GameTile : RuleTile<GameTile.Neighbor>
{
	[HideInInspector] public Tilemap targetTilemap;
	public TileType type;
	public string TileName;
	public int TileID;
	public GridSystem.TileLayer targetLayer;
	public float[] defaultContents = null;
	public class Neighbor : RuleTile.TilingRule.Neighbor
	{
		public const int Other = 3;
		public const int Any = 4;
	}

	public override bool RuleMatch(int neighbor, TileBase other)
	{
		switch (neighbor)
		{
			case Neighbor.Other: return other == this || other == null;
			case Neighbor.Any: return other == this || other != null;
		}
		return base.RuleMatch(neighbor, other);
	}
}