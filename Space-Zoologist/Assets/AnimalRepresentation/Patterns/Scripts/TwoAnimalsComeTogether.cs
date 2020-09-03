using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimalsComeTogether : BehaviorPattern
{
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        Vector3Int homeLocation = base.GridSystem.Grid.WorldToCell(animalData.animal.PopulationInfo.transform.position);
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position),
        homeLocation, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.Grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
            animalData.animal.MovementController.MoveTowardsDestination();
        }
        else
        {
            // TODO make animal idle while waiting for other goat
        }
        if (!AnimalsToAnimalData.ContainsKey(animalData.collaboratingAnimals[0]))
        {
            // AlternativeConditionSatisfied but let that return
            return false;
        }
        // both animals have reached home location
        if (AnimalsToAnimalData[animalData.collaboratingAnimals[0]].animal.MovementController.DestinationReached && animalData.animal.MovementController.DestinationReached)
        {
            return this.AnimalsLinedUp(animal, animalData.collaboratingAnimals[0], animalData);
        }
        return false;
    }

    protected override bool IsAlternativeConditionSatisfied(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
            if (animalData.animal.MovementController.DestinationCancelled)
            {
                return true;
            }
        }
        if (!AnimalsToAnimalData.ContainsKey(animalData.collaboratingAnimals[0]))
        {
            Debug.Log("Unable to reference collaborating animal, exiting behavior");
            return true;
        }
        return false;
    }

    private bool AnimalsLinedUp(GameObject animal, GameObject collaboratingAnimal, AnimalData animalData)
    {
        bool isLinedUp = true;
        // line up the y with small threshold
        if (animal.transform.position.y < collaboratingAnimal.transform.position.y - 0.1f)
        {
            animalData.animal.MovementController.ForceMoveInDirection(Direction.up);
            isLinedUp = false;
        }
        else if (animal.transform.position.y > collaboratingAnimal.transform.position.y + 0.1f)
        {
            animalData.animal.MovementController.ForceMoveInDirection(Direction.down);
            isLinedUp = false;
        }
        // then move left animal more left if not far enough apart and vice versa
        if (animal.transform.position.x <= collaboratingAnimal.transform.position.x)
        {
            float distanceBetweenAnimals = collaboratingAnimal.transform.position.x - animal.transform.position.x;
            // Seems to be the best distance
            if (!(distanceBetweenAnimals <= 1f && distanceBetweenAnimals >= .9f))
            {
                animalData.animal.MovementController.ForceMoveInDirection(Direction.left);
                isLinedUp = false;
            }
        }
        else if (animal.transform.position.x >= collaboratingAnimal.transform.position.x)
        {
            float distanceBetweenAnimals = animal.transform.position.x - collaboratingAnimal.transform.position.x;
            if (!(distanceBetweenAnimals <= 1f && distanceBetweenAnimals >= .9f))
            {
                animalData.animal.MovementController.ForceMoveInDirection(Direction.right);
                isLinedUp = false;
            }
        }
        return isLinedUp;
    }
}
