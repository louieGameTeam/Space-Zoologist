using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPattern : UniversalAnimatorPattern
{

    public override void Init()
    {
        base.Init();
    }

    protected override void EnterPattern(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        // spawn facing left
        base.EnterPattern(animal, animalCallbackData);
        SetAnimDirectionFloat(animal, 0, -1);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        animalCallbackData.animal.MovementController.StandStill();
        return base.IsPatternFinishedAfterUpdate(animal, animalCallbackData);
    }
}