using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementEmojiPattern : BehaviorPattern
{
    [SerializeField] Sprite Emoji = default;
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        int locationIndex = this.random.Next(0, animalData.animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int end = animalData.animal.PopulationInfo.AccessibleLocations[locationIndex];
        gameObject.transform.GetChild(0).GetComponent<Animator>().enabled = false;
        animalData.animal.Overlay.sprite = Emoji;
        // Debug.Log("Overlay ENABLED");
        //animalData.animal.Overlay.enabled = true;
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position), end, animalData.animal.MovementController.AssignPath, animalData.animal.PopulationInfo.Grid);
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
        //AnimalsToAnimalData[gameObject].animal.Overlay.enabled = false;
        base.ExitPattern(gameObject, callCallback);
    }

    // protected override void ForceExit(GameObject gameObject)
    // {
    //     Debug.Log("Overlay DISABLED");
    //     gameObject.GetComponent<Animal>().Overlay.enabled = false;
    //     base.ForceExit(gameObject);
    // }
}