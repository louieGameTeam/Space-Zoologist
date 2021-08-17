using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidNeed : Need
{
    public LiquidNeed(NeedConstructData needConstructData) : base(needConstructData) {}

    protected override NeedType GetNeedType()
    {
        return NeedType.Liquid;
    }
}
