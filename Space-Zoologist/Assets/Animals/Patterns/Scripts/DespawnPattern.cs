using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnPattern : UniversalAnimatorPattern
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
        animalData.animal.MovementController.StandStill();
    }

    protected override void ExitPattern(GameObject animal, bool callCallback)
    {
        base.ExitPattern(animal, callCallback);
    }

}