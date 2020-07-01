using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Takes in a path and moves sprite through it.
/// </summary>
public class MovementController : MonoBehaviour
{
    [Header("For testing, grid should be created by rpm")]
    [SerializeField] private AnimalPathfinding.Pathfinding pathfinder = default;
    [SerializeField] private Tilemap testingMap = default;
    private Animal Animal { get; set; }
    private AnimalPathfinding.Node PathToDestination { get; set; }
    private Vector3 NextPathTile { get; set; }
    TileSystem _tileSystem; //GetTerrainTile API from Virgil

    public void Start()
    {
        this.Animal = this.gameObject.GetComponent<Animal>();
         _tileSystem = FindObjectOfType<TileSystem>();
    }

    // TODO this shouldn't be needed and should instead be a part of the pathfinding system.
    // The pathfinding system should always have the most updated map in grid format
    public void GetPath(AnimalPathfinding.Node start, AnimalPathfinding.Node end, System.Action<AnimalPathfinding.Node, bool> callback) 
    {
        // Debug.Log("Start cell position: ");
        // Debug.Log("("+start.gridX+","+start.gridY+")");
        // Debug.Log("End cell position: ");
        // Debug.Log("("+end.gridX+","+end.gridY+")");
        AnimalPathfinding.PathRequestManager.RequestPath(start, end, callback, this.Animal.PopulationInfo.grid);
    }

    /// <summary>
    /// Function to be called in Update. When the NextPathNode is reached, the direction and movement are upated and the next node in the path is returned.
    /// </summary>
    /// <param name="pathToDestination"></param>
    /// <returns></returns>
    public AnimalPathfinding.Node MoveAlongLocationPath(AnimalPathfinding.Node pathToDestination)
    {
        if (this.NextPathNodeReached(this.NextPathTile, this.transform.position))
        {
            // The path wasn't properly found or the destination has been reached
            if (pathToDestination == null || pathToDestination.parent == null)
            {
                return null;
            }
            // Update to the next path tile and sprite stuff
            else
            {
                pathToDestination = pathToDestination.parent;
                this.NextPathTile = new Vector3(pathToDestination.gridX + 0.5f, pathToDestination.gridY + 0.5f, 0);
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
        return pathToDestination;
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
