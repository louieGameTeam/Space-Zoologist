using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFood : Behavior
{
    private System.Random random = new System.Random();

    protected override void Awake()
    {
        base.Awake();
    }

    // ERROR ArgumentOutOfRangeException: means the population gameobject's start location isn't in an accessible area
    // Use the Animal reference in base to access to behavior data.
    // Base is called last, enabling the component and thus enabling Update.
    public override void EnterBehavior(BehaviorFinished callback)
    {
        List<GameObject> FoodSources = Animal.PopulationInfo.AnimalPopulation;
        GameObject EdibleFood = null;
        foreach (GameObject food in FoodSources) {
            if (//Animal.PopulationInfo.Species.Needs.ContainsKey(food.SpeciesName) &&
              food != gameObject && ReservePartitionManager.ins.CanAccess(Animal.PopulationInfo, FindObjectOfType<TileSystem>().WorldToCell(food.transform.position))) {
                EdibleFood = food;
                break;
            }
        }
        if (EdibleFood != null)
        {
            Vector3Int end = FindObjectOfType<TileSystem>().WorldToCell(EdibleFood.transform.position);
            // PathRequestManager is static
            AnimalPathfinding.PathRequestManager.RequestPath(TilemapUtil.ins.WorldToCell(this.transform.position), end, base.PathFound, base.Animal.PopulationInfo.grid);
            base.EnterBehavior(callback);
        }
        else {
            Debug.Log("No Food Nearby.");
            Vector3Int end = FindObjectOfType<TileSystem>().WorldToCell(Animal.PopulationInfo.transform.position);
            // PathRequestManager is static
            AnimalPathfinding.PathRequestManager.RequestPath(TilemapUtil.ins.WorldToCell(this.transform.position), end, base.PathFound, base.Animal.PopulationInfo.grid);
            base.EnterBehavior(callback);
        }
        
    }

    // Default behavior moves along a random path
    protected override void Update()
    {
        if (!base.IsCalculatingPath())
        {
            base.MovementController.MoveTowardsDestination();
            if (base.MovementController.DestinationReached)
            {
                base.ExitBehavior();
            }
        }
    }
}
