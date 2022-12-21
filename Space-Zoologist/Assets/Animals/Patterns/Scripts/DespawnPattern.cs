using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnPattern : UniversalAnimatorPattern
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
        animalCallbackData.animal.MovementController.StandStill();
    }

    protected override void ExitPattern(GameObject animal, bool callCallback)
    {
        base.ExitPattern(animal, callCallback);
    }

}