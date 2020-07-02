using UnityEngine;
using System.Linq;

public class RandomMovement : Behavior
{
    protected override void Awake()
    {
        base.Awake();
    }

    // ERROR ArgumentOutOfRangeException: means the populations start location isn't in an accessible area
    // Use the Animal reference in base to access to behavior data.
    // Base is called last, enabling the component and thus enabling Update.
    public override void EnterBehavior(BehaviorFinished callback)
    {
        var r = new System.Random();
        int locationIndex = r.Next(0, Animal.PopulationInfo.AccessibleLocations.Count);
        Vector3Int end = Animal.PopulationInfo.AccessibleLocations[locationIndex];
        AnimalPathfinding.PathRequestManager.RequestPath(MapToGridUtil.ins.WorldToCell(this.transform.position), end, base.PathFound, base.Animal.PopulationInfo.grid);
        base.EnterBehavior(callback);
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