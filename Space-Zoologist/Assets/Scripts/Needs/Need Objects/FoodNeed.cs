using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodNeed : Need
{
    public FoodNeed(NeedConstructData needConstructData) : base(needConstructData) {}

    protected override NeedType GetNeedType()
    {
        return NeedType.FoodSource;
    }
}
