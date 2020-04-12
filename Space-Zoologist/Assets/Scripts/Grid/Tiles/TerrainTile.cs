using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Rock, Sand, Dirt, Grass, Liquid, Wall };
[CreateAssetMenu]
public class TerrainTile : RuleTile<TerrainTile.Neighbor> {
	public TileType type;
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
	public float[] contentValues;
	public bool isRepresentative;
	public bool isMergingAttributes;
	public bool isChangingColor;
	public float[,] interpolationArray = null;
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
	public Color GetTileColor()
	{
		if (contentValues.Length == 0)
		{
			return new Color(1, 1, 1, 1);
		}
		else
		{
			return RYBConverter.ToRYBColor(contentValues, interpolationArray);
		}
	}
}
