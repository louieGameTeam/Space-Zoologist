using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementEmojiPattern : BehaviorPattern
{
    [SerializeField] Sprite Emoji = default;
    private System.Random random = new System.Random();
    protected override void EnterPattern(GameObject gameObject, AnimalData animalData)
    {
        int locationIndex = this.random.Next(0, AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int end = AnimalsToAnimalData[gameObject].animal.PopulationInfo.AccessibleLocations[locationIndex];
        AnimalsToAnimalData[gameObject].animal.Overlay.sprite = Emoji;
        AnimalsToAnimalData[gameObject].animal.Overlay.enabled = true;
        AnimalPathfinding.PathRequestManager.RequestPath(base.GridSystem.Grid.WorldToCell(gameObject.transform.position), end, AnimalsToAnimalData[gameObject].animal.MovementController.AssignPath, AnimalsToAnimalData[gameObject].animal.PopulationInfo.grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        if (animalData.animal.MovementController.HasPath)
        {
<<<<<<< HEAD
=======
            if (animalData.animal.MovementController.DestinationCancelled)
            {
                Debug.Log("Pathfinding pattern force exited due to update");
                return true;
            }
>>>>>>> f6c9eaeded97730decbefda1e5e7f8c4ccccdbc6
            animalData.animal.MovementController.MoveTowardsDestination();
            if (animalData.animal.MovementController.DestinationReached)
            {
                AnimalsToAnimalData[animal].animal.Overlay.enabled = false;
                return true;
            }
        }
        return false;
    }
}