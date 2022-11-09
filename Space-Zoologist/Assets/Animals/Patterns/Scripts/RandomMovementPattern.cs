using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementPattern : BehaviorPattern
{
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject, AnimalCallbackData animalCallbackData)
    {
        if (animalCallbackData.animal.PopulationInfo.AccessibleLocations.Count == 0)
        {
            ExitPattern(gameObject);
            return;
        }
        int locationIndex = this.random.Next(0, AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int end = AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations[locationIndex];
        if (animalCallbackData.animal.MovementController == null)
        {
            return;
        }
        AnimalPathfinding.PathRequestManager.RequestPath(base.TileDataController.WorldToCell(gameObject.transform.position), end, AnimalsToAnimalData[gameObject].animal.MovementController.AssignPath, AnimalsToAnimalData[gameObject].animal.PopulationInfo.Grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        if (animalCallbackData.animal.MovementController.HasPath)
        {
            if (animalCallbackData.animal.MovementController.DestinationCancelled)
            {
                return false;
            }
            animalCallbackData.animal.MovementController.MoveTowardsDestination();
            if (animalCallbackData.animal.MovementController.DestinationReached)
            {
                return true;
            }
        }
        return false;
    }
    protected override bool IsAlternativeConditionSatisfied(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        if (animalCallbackData.animal.MovementController.HasPath)
        {
            if (animalCallbackData.animal.MovementController.DestinationCancelled)
            {
                return true;
            }
        }
        return false;
    }
    protected override void ExitPattern(GameObject gameObject, bool callCallback = true)
    {
        gameObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        AnimalsToAnimalData[gameObject].animal.Overlay.sprite = null;
        //AnimalsToAnimalData[gameObject].animal.Overlay.enabled = false;
        base.ExitPattern(gameObject, callCallback);
    }
}