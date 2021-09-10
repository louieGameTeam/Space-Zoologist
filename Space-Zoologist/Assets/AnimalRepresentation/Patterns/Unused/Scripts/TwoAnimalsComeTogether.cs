using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAnimalsComeTogether : BehaviorPattern
{
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        Vector3Int homeLocation = base.GridSystem.WorldToCell(animalData.animal.PopulationInfo.transform.position);
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.WorldToCell(gameObject.transform.position),
        homeLocation, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.Grid);
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
        }
        else
        {
            // TODO make animal idle while waiting for other goat
            animalData.animal.MovementData.MovementStatus = Movement.idle;
        }
        if (animalData.animal.MovementController.DestinationReached)
        {
            Vector3Int test = base.GridSystem.WorldToCell(this.gameObject.transform.position);
            animalData.animal.MovementController.ResetPathfindingConditions();
            return true;
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
        return false;
    }
}