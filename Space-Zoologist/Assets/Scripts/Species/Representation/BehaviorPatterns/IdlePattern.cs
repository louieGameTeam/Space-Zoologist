using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO 1 figure out how to inject GridSystem dependency for WorldToCell and
public class IdlePattern : BehaviorPattern
{
    private float TimeElapsed = 0f;

    protected override void EnterPattern(GameObject gameObject)
    {
        this.TimeElapsed = 0f;
    }
    // Default behavior moves along a random path
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        // TODO 2
        if (this.TimeElapsed > 5f)
        {
            return true;
        }
        this.TimeElapsed += Time.deltaTime;
        return false;
    }
}
