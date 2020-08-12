using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Example of how a behavior could be setup.
/// </summary>
public class RandomMovement : Behavior
{
    private System.Random random = new System.Random();

    protected override void Awake()
    {
        base.Awake();
    }

    public override void ExitBehavior()
    {
        base.ExitBehavior();
    }

    // ERROR ArgumentOutOfRangeException: means the population gameobject's start location isn't in an accessible area
    // Use the Animal reference in base to access to behavior data.
    // Base is called last, enabling the component and thus enabling Update.
    public override void EnterBehavior(BehaviorFinished callback)
    {
        // int locationIndex = this.random.Next(0, Animal.PopulationInfo.AccessibleLocations.Count);
        // Vector3Int end = Animal.PopulationInfo.AccessibleLocations[locationIndex];
        // // PathRequestManager is static
        // AnimalPathfinding.PathRequestManager.RequestPath(TilemapUtil.ins.WorldToCell(this.transform.position), end, base.PathFound, base.Animal.PopulationInfo.grid);
        // base.EnterBehavior(callback);
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