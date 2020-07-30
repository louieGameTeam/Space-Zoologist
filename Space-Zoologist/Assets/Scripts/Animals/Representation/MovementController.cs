using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// Takes in a path (List<Vector3>) and moves the attached gameobject through it.
/// </summary>
public class MovementController : MonoBehaviour
{
    private Animal Animal { get; set; }
    private List<Vector3> PathToDestination { get; set; }
    private int PathIndex = 0;
    private Vector3 NextPathTile { get; set; }
    public bool DestinationReached { get; private set; }
    public bool IsPaused = false;
    // private Vector3 NextPathTile { get; set; }

    public void Start()
    {
        this.Animal = this.gameObject.GetComponent<Animal>();
    }

    /// <summary>
    /// Called before update to assign a path.
    /// </summary>
    /// <param name="pathToDestination"></param>
    public void AssignPath(List<Vector3> pathToDestination)
    {
        this.PathToDestination = pathToDestination;
        this.NextPathTile = TilemapUtil.ins.GridToWorld(pathToDestination[0], 0.5f);
        this.DestinationReached = false;
        this.PathIndex = 0;
        this.UpdateVisualLogic(this.NextPathTile);
    }

    /// <summary>
    /// Called in update to move towards destination. Returns true when destination reached.
    /// </summary>
    /// <returns></returns>
    public void MoveTowardsDestination()
    {
        if (IsPaused)
        {
            this.Animal.BehaviorsData.MovementStatus = Movement.idle;
            return;
        }
        if (this.PathToDestination.Count == 0)
        {
            this.PathIndex = 0;
            this.DestinationReached = true;
            return;
        }
        if (this.NextPathVectorReached(this.NextPathTile, this.transform.position))
        {
            this.PathIndex++;
            // Destination reached
            if (this.PathIndex == this.PathToDestination.Count)
            {
                this.PathIndex = 0;
                this.DestinationReached = true;
                return;
            }
            // Update to the next path tile and visual logic stuff
            else
            {
                // Need to translate back from grid to world
                this.NextPathTile = TilemapUtil.ins.GridToWorld(this.PathToDestination[this.PathIndex], 0.5f);
                // Debug.Log("("+this.NextPathTile.x+"),"+"("+this.NextPathTile.y+")");
                this.UpdateVisualLogic(this.NextPathTile);
            }
        }
        this.transform.position = this.MoveTowardsTile(this.transform.position, this.NextPathTile, this.Animal.BehaviorsData.Speed);
    }

    public void MoveInDirection(Direction direction)
    {
        if (IsPaused)
        {
            this.Animal.BehaviorsData.MovementStatus = Movement.idle;
            return;
        }
        Vector3 vectorDirection = new Vector3(0, 0, 0);
        float speed = this.Animal.BehaviorsData.Speed * Time.deltaTime;
        switch(direction)
        {
            case Direction.up:
            {
                vectorDirection = new Vector3(this.transform.position.x, this.transform.position.y + speed , 0);
                break;
            }
            case Direction.down:
            {
                vectorDirection = new Vector3(this.transform.position.x, this.transform.position.y + -speed, 0);
                break;
            }
            case Direction.left:
            {
                vectorDirection = new Vector3(this.transform.position.x + -speed, this.transform.position.y, 0);
                break;
            }
            case Direction.right:
            {
                vectorDirection = new Vector3(this.transform.position.x + speed, this.transform.position.y, 0);
                break;
            }
            case Direction.upRight:
            {
                vectorDirection = new Vector3(this.transform.position.x + speed, this.transform.position.y + speed, 0);
                break;
            }
            case Direction.upLeft:
            {
                vectorDirection = new Vector3(this.transform.position.x + -speed,this.transform.position.y + speed, 0);
                break;
            }
            case Direction.downRight:
            {
                vectorDirection = new Vector3(this.transform.position.x + speed,this.transform.position.y + -speed, 0);
                break;
            }
            case Direction.downLeft:
            {
                vectorDirection = new Vector3(this.transform.position.x + -speed,this.transform.position.y + -speed, 0);
                break;
            }
        }
        this.UpdateVisualLogic(vectorDirection);
        this.transform.position = vectorDirection;
    }

    public void StandStill()
    {
        this.Animal.BehaviorsData.MovementStatus = Movement.idle;
        this.Animal.BehaviorsData.CurrentDirection = Direction.down;
    }

    // Can modify pointReachedOffset to have more precise movement towards each destination point
    private bool NextPathVectorReached(Vector3 destination, Vector3 currentLocation)
    {
        float pointReachedOffset = 0.5f;
        return currentLocation.x < destination.x + pointReachedOffset && currentLocation.x > destination.x - pointReachedOffset &&
        currentLocation.y < destination.y + pointReachedOffset && currentLocation.y > destination.y - pointReachedOffset;
    }

    // Can be modified for different movements potentially
    private Vector3 MoveTowardsTile(Vector3 currentPosition, Vector3 pathTile, float movementSpeed)
    {
        float step = movementSpeed * Time.deltaTime;
        return Vector3.MoveTowards(currentPosition, pathTile, step);
    }

    public void UpdateVisualLogic(Vector3 destination)
    {
        this.HandleDirectionChange(this.transform.position, destination);
        if (this.Animal.BehaviorsData.Speed > this.Animal.BehaviorsData.RunThreshold)
        {
            this.Animal.BehaviorsData.MovementStatus = Movement.running;
        }
        else
        {
            this.Animal.BehaviorsData.MovementStatus = Movement.walking;
        }
    }

    // Can be modified for different angles of direction change
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
            if (angle > 310)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.up;
            }
            else if (angle < 230)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.down;
            }
            else
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.right;
            }
        }
        else if (direction.x > 0)
        {
            if (angle < 50)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.up;
            }
            else if (angle > 130)
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.down;
            }
            else
            {
                this.Animal.BehaviorsData.CurrentDirection = Direction.left;
            }
        }
    }

}
