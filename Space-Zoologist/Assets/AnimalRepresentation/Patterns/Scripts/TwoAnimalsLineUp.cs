using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimalsLineUp : BehaviorPattern
{
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        bool isLinedUp = true;
        // line up the y with small threshold
        if (animal.transform.position.y < animalData.collaboratingAnimals[0].transform.position.y - 0.1f)
        {
            animalData.animal.MovementController.ForceMoveInDirection(Direction.up);
            isLinedUp = false;
        }
        else if (animal.transform.position.y > animalData.collaboratingAnimals[0].transform.position.y + 0.1f)
        {
            animalData.animal.MovementController.ForceMoveInDirection(Direction.down);
            isLinedUp = false;
        }
        // then move left animal more left if not far enough apart and vice versa
        if (animal.transform.position.x < animalData.collaboratingAnimals[0].transform.position.x)
        {
            float distanceBetweenAnimals = animalData.collaboratingAnimals[0].transform.position.x - animal.transform.position.x;
            // If not close together but also too far apart, don't move
            if (!(distanceBetweenAnimals <= 1f && distanceBetweenAnimals >= .9f))
            {
                animalData.animal.MovementController.ForceMoveInDirection(Direction.left);
                isLinedUp = false;
            }
        }
        else if (animal.transform.position.x > animalData.collaboratingAnimals[0].transform.position.x)
        {
            float distanceBetweenAnimals = animal.transform.position.x - animalData.collaboratingAnimals[0].transform.position.x;
            if (!(distanceBetweenAnimals <= 1f && distanceBetweenAnimals >= .9f))
            {
                animalData.animal.MovementController.ForceMoveInDirection(Direction.right);
                isLinedUp = false;
            }
        }
        return isLinedUp;
    }

    protected override bool IsAlternativeConditionSatisfied(GameObject animal, AnimalData animalData)
    {
        float distanceBetweenAnimals = animalData.collaboratingAnimals[0].transform.position.x - animal.transform.position.x;
        if (!(distanceBetweenAnimals >= -3f && distanceBetweenAnimals <=3f))
        {
            Debug.Log("Couldn't line animals up, exiting early");
            return true;
        }
        return false;
    }
}
