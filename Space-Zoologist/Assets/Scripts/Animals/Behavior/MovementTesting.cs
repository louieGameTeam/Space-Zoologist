using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


// TODO figure out how this should be refactored for clarity and scalability

public class MovementTesting : MonoBehaviour
{
    [Header("For determing accessible locations")]
    [SerializeField] Population PopulationInfo = default;
    [SerializeField] AnimalPathfinding Pathfinder = default;
    [SerializeField] float MovementSpeed = 1f;
    [SerializeField] float IdleLength = 1f;
    [SerializeField] float RunThreshold = 2;
    [Tooltip("Moves towards gameobjects one after the other in a loop. Lock this window to select multiple gameobjects and drag them here all at once.")]
    [SerializeField] List<GameObject> PredefinedPathToFollow = default;
    [SerializeField] Tilemap Mask = default;

    private Animator Animator = default;
    private List<Vector3Int> AccessibleLocations = default;
    private float IdleTime = 0f;
    private int PredefinedPathIndex = 0;
    // For moving towards destinations
    private Location PathToDestination = null;
    private Vector3 NextPathTile = default;
    // For sprite
    private Direction Direction = Direction.down;
    private Movement Movement = Movement.idle;
    // Start is called before the first frame update
    void Start()
    {
        MonoBehaviour[] scriptComponents = this.gameObject.GetComponents<MonoBehaviour>();    
        foreach(MonoBehaviour script in scriptComponents) {
            Debug.Log(script.GetType().ToString());
        }
        ReservePartitionManager.ins.AddPopulation(this.PopulationInfo);
        // TODO talk to alex about how this will work if a gameobject is instantiated outside it's accessible area, since right now it doesn't work
        this.AccessibleLocations = ReservePartitionManager.ins.GetLocationWithAccess(this.PopulationInfo);
        if (!this.gameObject.TryGetComponent(out this.Animator))
        {
            this.Animator = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // this.MoveSprite();
        // if (this.Animator != null)
        // {
        //     this.UpdateAnimations();
        // }
    }

    // If the current path's destination has been reached, update to next part of path or choose new path, then continue moving towards tile
    private void MoveSprite()
    {
        if (this.IsIdle(this.IdleLength, this.IdleTime))
        {
            this.Movement = Movement.idle;
            this.IdleTime += Time.deltaTime;
            return;
        }
        else if (this.PathToDestination == null)
        {
            if (this.PredefinedPathToFollow.Count == 0)
            {
                //this.PathToDestination = this.PickRandomPath(this.AccessibleLocations, this.PopulationInfo, this.transform.position);
            }
            else
            {
                this.HandlePredefinedPathLogic();
            }
            this.NextPathTile = new Vector3(this.PathToDestination.X + 0.5f, this.PathToDestination.Y + 0.5f, 0);
        }
        else if (this.NextSpotReached(this.NextPathTile, this.transform.position))
        {
            // The final destination has been reached so reset idle time, find a new path, and update your next path tile
            if (this.PathToDestination.Parent == null)
            {
                this.IdleTime = 0f;
                if (this.PredefinedPathToFollow.Count == 0)
                {
                    //this.PathToDestination = this.PickRandomPath(this.AccessibleLocations, this.PopulationInfo, this.transform.position);
                }
                else
                {
                    this.HandlePredefinedPathLogic();
                }
                this.NextPathTile = new Vector3(this.PathToDestination.X + 0.5f, this.PathToDestination.Y + 0.5f, 0);
            }
            // Update to the next path tile
            else
            {
                this.PathToDestination = this.PathToDestination.Parent;
                this.NextPathTile = new Vector3(this.PathToDestination.X + 0.5f, this.PathToDestination.Y + 0.5f, 0);
            }
            // After the next path tile has been chosen, update your direction
            this.HandleDirectionChange(this.transform.position, this.NextPathTile);
            // Then determine your movement
            if (this.MovementSpeed > this.RunThreshold)
            {
                this.Movement = Movement.running;
            }
            else
            {
                this.Movement = Movement.walking;
            }
        }
        this.transform.position = this.MoveTowardsTile(this.transform.position, this.NextPathTile, this.MovementSpeed);
    }

    private void HandlePredefinedPathLogic()
    {
        if (this.PredefinedPathToFollow.Count == 1 && this.PredefinedPathIndex == 1)
        {
            this.Movement = Movement.idle;
        }
        else if (this.AccessibleLocations.Contains(this.Mask.WorldToCell(this.PredefinedPathToFollow[this.PredefinedPathIndex].transform.position)))
        {
            Debug.Log("Path found");
            //this.PathToDestination = this.Pathfinder.FindPath(this.PopulationInfo, this.transform.position, this.PredefinedPathToFollow[this.PredefinedPathIndex].transform.position);
            this.PredefinedPathIndex++;
            if (this.PredefinedPathIndex == this.PredefinedPathToFollow.Count && this.PredefinedPathToFollow.Count != 1)
            {
                this.PredefinedPathIndex = 0;
            } 
        }
        else
        {
            Debug.Log("Path not found");
            // Could assign different behavior
            this.Movement = Movement.idle;
        }
    }

    private void UpdateAnimations()
    {
        this.Animator.SetInteger("Movement", (int)this.Movement);
        this.Animator.SetInteger("Direction", (int)this.Direction);
    }

    private Vector3 MoveTowardsTile(Vector3 currentPosition, Vector3 pathTile, float movementSpeed)
    {
        float step = movementSpeed * Time.deltaTime;
        return Vector3.MoveTowards(currentPosition, pathTile, step);
    }

    private bool IsIdle(float idleLength, float idleTime)
    {
        return idleLength > 0f && idleTime < idleLength;
    }

    private bool NextSpotReached(Vector3 destination, Vector3 currentLocation)
    {
        return currentLocation.x < destination.x + .5f && currentLocation.x > destination.x - .5f &&
        currentLocation.y < destination.y + .5f && currentLocation.y > destination.y - .5f;
    }

    // private Location PickRandomPath(List<Vector3Int> accessibleLocations, Population populationInfo, Vector3 currentPosition)
    // {
    //     var r = new System.Random();
    //     int locationIndex = r.Next(0, accessibleLocations.Count);
    //     return this.Pathfinder.FindPath(populationInfo, currentPosition, accessibleLocations[locationIndex]);
    // }

    // finds the angle between the direction and a vector pointing up to determine angle of direction
    private void HandleDirectionChange(Vector3 currentPosition, Vector3 nextTile)
    {
        Vector3 direction = (nextTile - currentPosition).normalized;
        float angle = Vector3.Angle(Vector3.up, direction);
        // Moving left, subtracting 360 and making the angle positive makes it easy to determine what the angle of direction is
        if (direction.x <= 0)
        {
            angle -= 360;
            if (angle < 0)
            {
                angle *= -1;
            }
            if (angle > 315)
            {
                this.Direction = Direction.up;
            }
            else if (angle < 225)
            {
                this.Direction = Direction.down;
            }
            else
            {
                this.Direction = Direction.left;
            }
        }
        else if (direction.x >= 0)
        {
            if (angle < 45)
            {
                this.Direction = Direction.up;
            }
            else if (angle > 135)
            {
                this.Direction = Direction.down;
            }
            else
            {
                this.Direction = Direction.right;
            }
        }
    }
}
