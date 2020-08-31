using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimalsComeTogether : BehaviorPattern
{
    [SerializeField] float OptimalDistance = 0.6f;
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        Vector3Int homeLocation = base.GridSystem.Grid.WorldToCell(animalData.animal.PopulationInfo.transform.position);
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position),
        homeLocation, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
            animalData.animal.MovementController.MoveTowardsDestination();
        }
        if (!AnimalsToAnimalData.ContainsKey(animalData.collaboratingAnimals[0]))
        {
            Debug.Log("Unable to reference collaborating animal");
            return false;
        }
        // both animals have reached home location
        if (AnimalsToAnimalData[animalData.collaboratingAnimals[0]].animal.MovementController.DestinationReached && animalData.animal.MovementController.DestinationReached)
        {
            return this.AnimalsLinedUp(animal, animalData.collaboratingAnimals[0], animalData);
        }
        return false;
    }

    private bool AnimalsLinedUp(GameObject animal, GameObject collaboratingAnimal, AnimalData animalData)
    {
        bool isLinedUp = true;
        // first line up the y
        if (animal.transform.position.y < collaboratingAnimal.transform.position.y - 0.1f)
        {
            animalData.animal.MovementController.ForceMoveInDirection(Direction.up);
            Debug.Log("Moving up");
            isLinedUp = false;
        }
        else if (animal.transform.position.y > collaboratingAnimal.transform.position.y + 0.1f)
        {
            animalData.animal.MovementController.ForceMoveInDirection(Direction.down);
            Debug.Log("Moving down");
            isLinedUp = false;
        }
        // move left animal more left if not far enough apart
        if (animal.transform.position.x <= collaboratingAnimal.transform.position.x)
        {
            float distanceBetweenAnimals = collaboratingAnimal.transform.position.x - animal.transform.position.x;
            if (!(distanceBetweenAnimals <= 1f && distanceBetweenAnimals >= .9f))
            {
                animalData.animal.MovementController.ForceMoveInDirection(Direction.left);
                isLinedUp = false;
                Debug.Log("Moving left");
            }
        }
        // move right animal more right if not far enough apart
        else if (animal.transform.position.x >= collaboratingAnimal.transform.position.x)
        {
            float distanceBetweenAnimals = animal.transform.position.x - collaboratingAnimal.transform.position.x;
            if (!(distanceBetweenAnimals <= 1f && distanceBetweenAnimals >= .9f))
            {
                animalData.animal.MovementController.ForceMoveInDirection(Direction.right);
                isLinedUp = false;
                Debug.Log("Moving right");
            }
        }
        return isLinedUp;
    }
}
