using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Stone, Sand, Dirt, Grass, Liquid };
[CreateAssetMenu]
public class TerrainTile : RuleTile<TerrainTile.Neighbor> 
{
	[HideInInspector] public Tilemap targetTilemap;
	[HideInInspector] public List<Tilemap> replacementTilemap;
	[HideInInspector] public List<Tilemap> constraintTilemap;
	public TileType type;
	public GridUtils.TileLayer targetLayer;
	public List<GridUtils.TileLayer> replacementLayers;
	public List<GridUtils.TileLayer> constraintLayers;
	public bool isRepresentative;
	public bool isMergingAttributes;
	public int priority;
	public class Neighbor
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
