using UnityEngine;

public class RandomMovement : Behavior
{
    private Location PathToDestination { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }
    
    // Use the Animal reference in base to access to behavior data.
    // Base is called last, enabling the component and thus enabling Update.
    public override void EnterBehavior(BehaviorFinished callback)
    {
        this.PathToDestination = this.PickRandomPath(base.Animal.BehaviorsData, this.transform.position);
        base.EnterBehavior(callback);
    }   
    
    // Default behavior moves along a random path
    protected override void Update()
    {
        if (this.PathToDestination == null)
        {
            // This behavior is simple so ExitBehavior doesn't need to be overriden
            base.ExitBehavior();
        }
        else
        {
            this.PathToDestination = base.Controller.MoveAlongLocationPath(this.PathToDestination);
        }
    }

    // ERROR ArgumentOutOfRangeException: means the populations start location isn't in an accessible area
    private Location PickRandomPath(BehaviorsData data, Vector3 currentPosition)
    {
        var r = new System.Random();
        int locationIndex = r.Next(0, Animal.PopulationInfo.AccessibleLocations.Count);
        return base.Animal.Pathfinder.FindPath(base.Animal.PopulationInfo, currentPosition, base.Animal.PopulationInfo.AccessibleLocations[locationIndex]);
    }
}