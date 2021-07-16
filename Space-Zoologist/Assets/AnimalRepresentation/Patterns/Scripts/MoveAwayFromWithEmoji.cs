using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A pattern that cause the animal to move away from their current location while displaying an emoji
/// </summary>
public class MoveAwayFromWithEmoji : BehaviorPattern
{
    [Tooltip("Distance to be travelled from the current location")]
    [SerializeField] float distance = default;
    [SerializeField] Sprite Emoji = default;
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        int locationIndex = 0;
        Vector3Int end = new Vector3Int();
        int iter = 0; // how many times we've tried to find a valid location
        do
        {
            locationIndex = this.random.Next(0, animalData.animal.PopulationInfo.AccessibleLocations.Count);
            end = animalData.animal.PopulationInfo.AccessibleLocations[locationIndex];
            iter++;
        } while (iter < 20 && Vector3.Distance(base.GridSystem.Grid.WorldToCell(gameObject.transform.position), end) < distance);
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
        AnimalsToAnimalData[gameObject].animal.Overlay.sprite = null;
        //AnimalsToAnimalData[gameObject].animal.Overlay.enabled = false;
        base.ExitPattern(gameObject, callCallback);
    }

    protected override void ForceExit(GameObject gameObject)
    {
        // Debug.Log("Overlay DISABLED " + gameObject.GetComponent<AnimalBehaviorManager>().activeBehaviorPatterns[0].gameObject.name);
        gameObject.GetComponent<Animal>().Overlay.enabled = false;
        base.ForceExit(gameObject);
    }
}
