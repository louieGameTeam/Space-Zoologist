using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimalFaceEachOther : BehaviorPattern
{
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        if (animal.transform.position.x < animalData.collaboratingAnimals[0].transform.position.x)
        {
            animalData.animal.MovementData.CurrentDirection = Direction.left;
        }
        else
        {
            animalData.animal.MovementData.CurrentDirection = Direction.right;
        }
    }
}
