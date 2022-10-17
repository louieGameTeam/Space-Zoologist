using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPattern : UniversalAnimatorPattern
{

    public override void Init()
    {
        base.Init();
    }

    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        // spawn facing left
        base.EnterPattern(animal, animalData);
        SetAnimDirectionFloat(animal, 0, -1);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        animalData.animal.MovementController.StandStill();
        return base.IsPatternFinishedAfterUpdate(animal, animalData);
    }
}