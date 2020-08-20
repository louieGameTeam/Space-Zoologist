using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO 1figure out how to inject GridSystem dependency for WorldToCell and
// TODO 2 figure out where PathCalculation should occur for pathfinding
public class SampleMovementBehavior : BehaviorPattern
{
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject)
    {
        int locationIndex = this.random.Next(0, AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int end = AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations[locationIndex];
        // PathRequestManager is static
        // TODO 1
        // AnimalPathfinding.PathRequestManager.RequestPath(TilemapUtil.ins.WorldToCell(gameObject.transform.position), end, AnimalsToAnimalData[gameObject].animal.MovementController.PathFound, AnimalsToAnimalData[gameObject].animal.PopulationInfo.grid);
    }
    // Default behavior moves along a random path
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        // TODO 2
        // if (!animalData.animal.MovementController.IsCalculatingPath())
        // {
        //     animalData.animal.MovementController.MoveTowardsDestination();
        //     if (animalData.animal.MovementController.DestinationReached)
        //     {
        //         return true;
        //     }
        // }
        return false;
    }
}
