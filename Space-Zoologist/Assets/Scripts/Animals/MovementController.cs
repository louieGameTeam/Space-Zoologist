using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes in a path and moves sprite through it.
/// </summary>
public class MovementController : MonoBehaviour
{
    private Animal Animal { get; set; }
    private Location PathToDestination { get; set; }
    private Vector3 NextPathTile { get; set; }

    public void Start()
    {
        this.Animal = this.gameObject.GetComponent<Animal>();
    }

    public Location GetLocationPath(Vector3 start, Vector3 end) 
    {
        return Animal.Pathfinder.FindPath(Animal.PopulationInfo, start, end);
    }

    public Location MoveAlongLocationPath(Location pathToDestination)
    {
        if (this.NextSpotReached(this.NextPathTile, this.transform.position))
        {
            // The destination has been reached
            if (pathToDestination.Parent == null)
            {
                return null;
            }
            // Update to the next path tile and sprite stuff
            else
            {
                pathToDestination = pathToDestination.Parent;
                this.NextPathTile = new Vector3(pathToDestination.X + 0.5f, pathToDestination.Y + 0.5f, 0);
                // After the next path tile has been chosen, update your direction
                this.HandleDirectionChange(this.transform.position, this.NextPathTile);
                // Then determine your movement
                if (this.Animal.BehaviorsData.Speed > this.Animal.BehaviorsData.RunThreshold)
                {
                    this.Animal.BehaviorsData.MovementStatus = Movement.running;
                }
                else
                {
                    this.Animal.BehaviorsData.MovementStatus = Movement.walking;
                }
            }
        }
        this.transform.position = this.MoveTowardsTile(this.transform.position, this.NextPathTile, this.Animal.BehaviorsData.Speed);
        return pathToDestination;
    }

    private bool NextSpotReached(Vector3 destination, Vector3 currentLocation)
    {
        return currentLocation.x < destination.x + .5f && currentLocation.x > destination.x - .5f &&
        currentLocation.y < destination.y + .5f && currentLocation.y > destination.y - .5f;
    }

    // This can be modified for different movements potentially
    private Vector3 MoveTowardsTile(Vector3 currentPosition, Vector3 pathTile, float movementSpeed)
    {
        float step = movementSpeed * Time.deltaTime;
        return Vector3.MoveTowards(currentPosition, pathTile, step);
    }

    private void HandleDirectionChange(Vector3 currentPosition, Vector3 nextTile)
    {
        Vector3 direction = (nextTile - currentPosition).normalized;
        float angle = Vector3.Angle(Vector3.up, direction);
        // Moving left. Subtracting 360 and making the angle positive makes it easy to determine what the angle of direction is
        if (direction.x <= 0)
        {
            angle -= 360;
            if (angle < 0)
            {
                angle *= -1;
            }
            if (angle > 315)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.up;
            }
            else if (angle < 225)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.down;
            }
            else
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.left;
            }
        }
        else if (direction.x >= 0)
        {
            if (angle < 45)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.up;
            }
            else if (angle > 135)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.down;
            }
            else
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.right;
            }
        }
    }

}
