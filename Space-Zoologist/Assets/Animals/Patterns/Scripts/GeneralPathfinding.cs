using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
            // First move
            Vector3 moveStep = animalMovementController.MoveTowardsDestination();
            
            // Check if moved onto an invalid tile
            var target = animalMovementController.transform.position;
            var targetTileData = GameManager.Instance.m_tileDataController.GetGameTileAt(target);
            if (!animalCallbackData.animal.PopulationInfo.species.AccessibleTerrain.Contains(targetTileData.type))
            {
                // If so, then undo the movestep and cancel the entire behavior 
                // TODO: Maybe have the pathfinding try again?
                animalMovementController.MoveVector(-moveStep);
                animalMovementController.TryToCancelDestination();
                return false;
            }

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

    private static float timeList = 0f;

    /// <summary>
    /// If the path is cancelled, such as due to tiles being placed in the way, then kick out of the entire pop behavior.
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="animalCallbackData"></param>
    /// <returns></returns>
    protected override bool IsAlternativeConditionSatisfied(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        MovementController animalMovementController = animalCallbackData.animal.MovementController;
        if (animalMovementController.HasPath)
        {
            if (animalMovementController.DestinationCancelled)
            {
                return true;
            }
        }
        return false;
    }
}