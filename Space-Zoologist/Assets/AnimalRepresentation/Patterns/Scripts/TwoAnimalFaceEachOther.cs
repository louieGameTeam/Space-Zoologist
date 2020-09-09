using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimalFaceEachOther : TimedPattern
{
    protected override void EnterPattern(GameObject animal, AnimalData animalData)
    {
        if (animal.transform.position.x < animalData.collaboratingAnimals[0].transform.position.x)
        {
            animalData.animal.MovementData.CurrentDirection = Direction.right;
        }
        else
        {
            animalData.animal.MovementData.CurrentDirection = Direction.left;
        }
        base.EnterPattern(animal, animalData);
    }
}
