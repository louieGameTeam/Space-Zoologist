using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//public enum SelectionType { All, FreeOnly, ConcurrentOnly, OverrideOnly, FreeAndConcurrent, FreeAndOverride, ConcurrentAndOverride };
public class BehaviorUtils
{
    public static bool IsTargetValueReached(float original, float target, float current)
    {
        if (target > original)
        {
            if (current < target)
            {
                return false;
            }
        }
        else if(target < original)
        {
            if (current > target)
            {
                return false;
            }
        }
        return true;
    }
}
