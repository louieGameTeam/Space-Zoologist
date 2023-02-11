using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralPathfinding : BehaviorPattern
{
    [Header("Terrain used in place of liquid")]
    [SerializeField] 
    ItemRegistry.Category Destination = default;

    protected override void EnterPattern(GameObject gameObject, AnimalCallbackData animalCallbackData)
    {
        Vector3Int destination = base.TileDataController.WorldToCell(gameObject.transform.position);
        if (Destination.Equals(ItemRegistry.Category.Tile))
        {
            destination = base.TileDataController.FindClosestLiquidSource(animalCallbackData.animal.PopulationInfo, gameObject);
        }
        else
        {
            int locationIndex = animalCallbackData.animal.PopulationInfo.random.Next(0, animalCallbackData.animal.PopulationInfo.AccessibleLocations.Count);
            if (animalCallbackData.animal.PopulationInfo.AccessibleLocationsExist)
                destination = animalCallbackData.animal.PopulationInfo.AccessibleLocations[locationIndex];
        }
        AnimalPathfinding.PathRequestManager.RequestPath(
            base.TileDataController.WorldToCell(gameObject.transform.position), 
            destination, 
            animalCallbackData.animal.MovementController.AssignPath, 
            animalCallbackData.animal.PopulationInfo.Grid);
    }
    protected override bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        MovementController animalMovementController = animalCallbackData.animal.MovementController;
        
        if (animalMovementController.HasPath)
        {
            var target = animalMovementController.transform.position;

            var targetTileData = GameManager.Instance.m_tileDataController.GetGameTileAt(target);

            if (!animalCallbackData.animal.PopulationInfo.species.AccessibleTerrain.Contains(targetTileData.type))
            {
                animalMovementController.TryToCancelDestination();
            }

            animalMovementController.MoveTowardsDestination();
            if (animalMovementController.DestinationReached)
            {
                animalMovementController.ResetPathfindingConditions();
                //Debug.Log(animal.name + " has reached their destination of " + this.Destination.ToString());
                return true;
            }
            return false;
        }
        return true;
    }

    protected override bool IsAlternativeConditionSatisfied(GameObject animal, AnimalCallbackData animalCallbackData)
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