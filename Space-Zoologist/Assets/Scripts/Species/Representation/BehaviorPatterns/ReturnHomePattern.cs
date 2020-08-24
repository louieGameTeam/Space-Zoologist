using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHomePattern : BehaviorPattern
{
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject)
    {
        int locationIndex = this.random.Next(0, AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int start = base.GridSystem.Grid.WorldToCell(gameObject.transform.position);
        Vector3Int end = base.GridSystem.Grid.WorldToCell(AnimalsToAnimalData[gameObject].animal.PopulationInfo.transform.position);
        Debug.Log("Attempting to pathfind to : " + AnimalsToAnimalData[gameObject].animal.PopulationInfo.transform.position.x + ", " + AnimalsToAnimalData[gameObject].animal.PopulationInfo.transform.position.y);
        AnimalPathfinding.PathRequestManager.RequestPath(start, end, AnimalsToAnimalData[gameObject].animal.MovementController.AssignPath, AnimalsToAnimalData[gameObject].animal.PopulationInfo.grid);
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
