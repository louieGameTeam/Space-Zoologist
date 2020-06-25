using System.Collections.Generic;
using UnityEngine;

// Demos movement between different locations
public class Eating : Behavior
{
    private Location PathToDestination { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void EnterBehavior(BehaviorFinished callback)
    {
        List<Vector3> foodLocations = new List<Vector3>();
        foreach (GameObject foodSource in base.Animal.BehaviorsData.FoodSourceLocations)
        {
            foodLocations.Add(foodSource.transform.position);
        }
        // this.PathToDestination = base.Controller.CreatePredefinedPath(foodLocations);
        base.EnterBehavior(callback);
    } 

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
}
