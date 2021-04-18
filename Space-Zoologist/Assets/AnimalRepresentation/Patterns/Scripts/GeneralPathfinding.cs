using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPathfinding : BehaviorPattern
{
    [Header("Terrain used in place of liquid")]
    [SerializeField] ItemType Destination = default;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        int locationIndex = animalData.animal.PopulationInfo.random.Next(0, animalData.animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int destination = animalData.animal.PopulationInfo.AccessibleLocations[locationIndex];
        Debug.Log("Pathfidning towards " + this.Destination.ToString() + " located at " + destination.x + ", " + destination.y);
        Debug.Log("World to cell: " + base.GridSystem.Grid.WorldToCell(gameObject.transform.position) + " movement controller: " + animalData.animal.MovementController.ToString());// + " grid: " + animalData.animal.PopulationInfo.Grid.ToString());
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position), destination, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.Grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
            if (animalData.animal.MovementController.DestinationCancelled)
            {
                return true;
            }
            animalData.animal.MovementController.MoveTowardsDestination();
            if (animalData.animal.MovementController.DestinationReached)
            {
                animalData.animal.MovementController.ResetPathfindingConditions();
                // Debug.Log(animal.name + " has reached their destination of " + this.Destination.ToString());
                return true;
            }
        }
        return false;
    }

    protected override bool IsAlternativeConditionSatisfied(GameObject animal, AnimalData animalData)
    {
        //if (animalData.animal.MovementController.HasPath)
        //{
        //    if (animalData.animal.MovementController.DestinationCancelled)
        //    {
        //        return true;
        //    }
        //}
        return false;
    }
}