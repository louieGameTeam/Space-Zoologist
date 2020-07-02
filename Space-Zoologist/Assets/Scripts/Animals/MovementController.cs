using UnityEngine;

/// <summary>
/// Takes in a path and moves sprite through it.
/// </summary>
public class MovementController : MonoBehaviour
{
    private Animal Animal { get; set; }
    private AnimalPathfinding.Node PathToDestination { get; set; }
    private Vector3 NextPathTile { get; set; }
    public bool DestinationReached = true;
    // private Vector3 NextPathTile { get; set; }

    public void Start()
    {
        this.Animal = this.gameObject.GetComponent<Animal>();
    }

    public void AssignPath(AnimalPathfinding.Node pathToDestination)
    {
        this.PathToDestination = pathToDestination;
        this.NextPathTile = MapToGridUtil.ins.GridToCell(pathToDestination, 0.5f);
        this.DestinationReached = false;
    }

    /// <summary>
    /// Called in update to move towards destination. Returns true when destination reached.
    /// </summary>
    /// <returns></returns>
    public void MoveTowardsDestination()
    {
        if (this.NextPathNodeReached(this.NextPathTile, this.transform.position))
        {
            // The destination has been reached
            if (this.PathToDestination == null || this.PathToDestination.parent == null)
            {
                this.DestinationReached = true;
                return;
            }
            // Update to the next path tile and sprite stuff
            else
            {
                this.PathToDestination = this.PathToDestination.parent;
                // Need to translate back from grid to cell
                this.NextPathTile = MapToGridUtil.ins.GridToCell(this.PathToDestination, 0.5f);
                //Debug.Log("("+pathToDestination.gridX+"),"+"("+pathToDestination.gridY+")");
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
    }

    private bool NextPathNodeReached(Vector3 destination, Vector3 currentLocation)
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
        int angle = (int)Vector3.Angle(Vector3.up, direction);
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
