using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorExecutionData
{
    public int currentBehaviorIndex = 0;
    public int avaliableAnimalCount = 1;
    public PopulationBehavior pendingBehavior = null; // filled with behavior if the corresponding gameobject is initiating a coordinated behavior

    public BehaviorExecutionData(int behaviorIndex)
    {
        currentBehaviorIndex = behaviorIndex;
    }

    public PopulationBehavior NextBehavior(List<PopulationBehavior> behaviors)
    {
        currentBehaviorIndex++;
        if (currentBehaviorIndex >= behaviors.Count)
        {
            currentBehaviorIndex = 0;// Start another loop if finished list OR list reduced
        }
        return behaviors[currentBehaviorIndex];
    }
}
