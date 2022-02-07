using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementPattern : BehaviorPattern
{
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        if (animalData.animal.PopulationInfo.AccessibleLocations.Count == 0)
        {
            ExitPattern(gameObject);
            return;
        }
        int locationIndex = this.random.Next(0, AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int end = AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations[locationIndex];
        if (animalData.animal.MovementController == null)
        {
            return;
        }
        AnimalPathfinding.PathRequestManager.RequestPath(base.TileDataController.WorldToCell(gameObject.transform.position), end, AnimalsToAnimalData[gameObject].animal.MovementController.AssignPath, AnimalsToAnimalData[gameObject].animal.PopulationInfo.Grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
            if (animalData.animal.MovementController.DestinationCancelled)
            {
                return false;
            }
            animalData.animal.MovementController.MoveTowardsDestination();
            if (animalData.animal.MovementController.DestinationReached)
            {
                return true;
            }
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
    protected override void ExitPattern(GameObject gameObject, bool callCallback = true)
    {
        gameObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        AnimalsToAnimalData[gameObject].animal.Overlay.sprite = null;
        //AnimalsToAnimalData[gameObject].animal.Overlay.enabled = false;
        base.ExitPattern(gameObject, callCallback);
    }
}