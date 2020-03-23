using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TerrainTile : RuleTile<TerrainTile.Neighbor> {

    public enum TileLayer
	{
		BaseLayer,
		Terrain,
		LiquidBackground,
		Liquid,
		LiquidSurface,
		LiquidTexture,
		Grass
	}
	public TileLayer tileLayer;
	public List<TileLayer> replacementLayers = new List<TileLayer>();
	public List<TileLayer> constraintLayers = new List<TileLayer>();
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
