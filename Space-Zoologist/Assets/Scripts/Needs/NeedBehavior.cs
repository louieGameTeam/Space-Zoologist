using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeedBehavior
{
    [SerializeField] public NeedCondition Condition = default;
    [SerializeField] public PopulationBehavior Behavior = default;

    public NeedBehavior(NeedCondition needCondition)
    {
        this.Condition = needCondition;
    }
}
