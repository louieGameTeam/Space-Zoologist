using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TerrainTile : RuleTile<TerrainTile.Neighbor> {

    public string tileId;
	public List<TileBase> sibings = new List<TileBase>();
	public class Neighbor : RuleTile.TilingRule.Neighbor
	{
		public const int Sibing = 3;
		public const int Any = 4;
	}
	public override bool RuleMatch(int neighbor, TileBase other)
	{
		switch (neighbor)
		{
			case Neighbor.Sibing: return sibings.Contains(other);
			case Neighbor.Any: return other == this || sibings.Contains(other);
		}
		return base.RuleMatch(neighbor, other);
	}
}
