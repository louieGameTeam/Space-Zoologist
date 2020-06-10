using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Rock, Sand, Dirt, Grass, Liquid, Wall, TypesOfTiles };
[CreateAssetMenu]
public class TerrainTile : RuleTile<TerrainTile.Neighbor>
{
	public Tilemap targetTilemap;
	public List<Tilemap> replacementTilemap;
	public List<Tilemap> constraintTilemap;
	public TileType type;
	public GridUtils.TileLayer targetLayer;
	public List<GridUtils.TileLayer> replacementLayers;
	public List<GridUtils.TileLayer> constraintLayers;
	public List<TerrainTile> auxillaryTiles = new List<TerrainTile>();
	public bool isRepresentative;
	public bool isMergingAttributes;
	public int priority;
	public class Neighbor : RuleTile.TilingRule.Neighbor
	{
		public const int Sibing = 3;
		public const int Any = 4;
	}
	public override bool RuleMatch(int neighbor, TileBase other)
	{
		switch (neighbor)
		{
			case Neighbor.Sibing: return other != null;
			case Neighbor.Any: return other == this || other != null;
		}
		return base.RuleMatch(neighbor, other);
	}
}