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
        Vector3Int end = Animal.PopulationInfo.AccessibleLocations.ElementAt(locationIndex);
        base.Controller.GetPath(new AnimalPathfinding.Node(0, base.Animal.PopulationInfo.WorldToCell(transform.position).x, base.Animal.PopulationInfo.WorldToCell(transform.position).y), new AnimalPathfinding.Node(0, end.x, end.y), base.PathFound);
        base.EnterBehavior(callback);
    }   
    
    // Default behavior moves along a random path
    protected override void Update()
    {
        if (!base.IsCalculatingPath())
        {
            this.PathToDestination = base.Controller.MoveAlongLocationPath(this.PathToDestination);
            if (this.PathToDestination == null)
            {
                base.ExitBehavior();
            }
        }
    }
}