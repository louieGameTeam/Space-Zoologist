using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementPattern : BehaviorPattern
{
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject)
    {
        int locationIndex = this.random.Next(0, AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int end = AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations[locationIndex];
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position), end, AnimalsToAnimalData[gameObject].animal.MovementController.AssignPath, AnimalsToAnimalData[gameObject].animal.PopulationInfo.grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
            animalData.animal.MovementController.MoveTowardsDestination();
            if (animalData.animal.MovementController.DestinationReached)
            {
                return true;
            }
        }
        return false;
    }
}