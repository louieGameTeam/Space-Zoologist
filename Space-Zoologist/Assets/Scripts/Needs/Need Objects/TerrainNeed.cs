using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainNeed : Need
{
    public TerrainNeed(NeedConstructData needConstructData) : base(needConstructData) {}

    protected override NeedType GetNeedType()
    {
        return NeedType.Terrain;
    }
}
