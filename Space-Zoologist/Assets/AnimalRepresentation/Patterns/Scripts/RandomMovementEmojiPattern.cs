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
        animalData.animal.Overlay.sprite = Emoji;
        // Debug.Log("Overlay ENABLED");
        animalData.animal.Overlay.enabled = true;
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
                animalData.animal.MovementController.ResetPathfindingConditions();
                animalData.animal.Overlay.enabled = false;
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
                animalData.animal.Overlay.enabled = false;
                return true;
            }
        }
        return false;
    }

    protected override void ForceExit(GameObject gameObject)
    {
        Debug.Log("Overlay DISABLED " + gameObject.GetComponent<AnimalBehaviorManager>().activeBehaviorPatterns[0].gameObject.name);
        gameObject.GetComponent<Animal>().Overlay.enabled = false;
        base.ForceExit(gameObject);
    }
}