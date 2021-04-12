using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPathfinding : BehaviorPattern
{
    [Header("Terrain used in place of liquid")]
    [SerializeField] ItemType Destination = default;

    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        Vector3Int destination = base.GridSystem.FindClosestItem(animalData.animal.PopulationInfo, gameObject, Destination);
        if (destination.Equals(new Vector3Int(-1, -1, -1)))
        {
            // TODO figure out how to exit pattern since condition not satisfied
            int locationIndex = animalData.animal.PopulationInfo.random.Next(0, animalData.animal.PopulationInfo.AccessibleLocations.Count);
            destination = animalData.animal.PopulationInfo.AccessibleLocations[locationIndex];
            // Debug.Log(this.Destination.ToString() + " location not found, pathfinding to random location instead");
        }
        // Debug.Log("Pathfidning towards " + this.Destination.ToString() + " located at " + destination.x + ", " + destination.y);
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