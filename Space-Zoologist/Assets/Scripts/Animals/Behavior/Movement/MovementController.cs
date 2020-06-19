using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// TODO create functions that do the following: circle vector3, pace between vector3s, move randomly, move to location for set amount of time
/// TODO also create generic movement modifiers: change movement speed (randomly, increase, decrease, etc. ), pause length, direction
/// TODO define and prototype the behavior script that calls these functions
public class MovementController : MonoBehaviour
{
    private MovementData Data;
    // Calculated by AnimalPathfinding
    private Location PathToDestination = null;
    private AnimalPathfinding Pathfinder = default;

    public Movement MovementStatus { get; private set; }
    public Direction CurrentDirection { get; private set; }

    public void Start()
    {
        this.Pathfinder = this.gameObject.GetComponent<AnimalPathfinding>();
    }

    public void Initialize(MovementData data)
    {
        this.Data = data;
    }

    // Setup a predefined path by adding different gameobjects to a list, returns -1 when finished or starting over
    public int FollowPredefinedPath(List<GameObject> predefinedPathToFollow, int currentPathIndex)
    {
        if (predefinedPathToFollow.Count == 1 && currentPathIndex == 1)
        {
            this.MovementStatus = Movement.idle;
            return -1;
        }
        if (this.NextSpotReached(predefinedPathToFollow[currentPathIndex].transform.position, this.transform.position))
        {
            this.PathToDestination = this.Pathfinder.FindPath(this.Data, this.transform.position, predefinedPathToFollow[currentPathIndex].transform.position);
            currentPathIndex++;
            if (currentPathIndex == predefinedPathToFollow.Count && predefinedPathToFollow.Count != 1)
            {
                currentPathIndex = 0;
                return -1;
            } 
        }
        this.MoveTowardsTile(this.transform.position, predefinedPathToFollow[currentPathIndex].transform.position, this.Data.MovementSpeed);
        return currentPathIndex;
    }

    public bool IsSpotValid(Vector3 spot) 
    {
        return this.Data.AccessibleLocations.Contains(this.Data.WorldToCell(spot));
    }

    private bool NextSpotReached(Vector3 destination, Vector3 currentLocation)
    {
        return currentLocation.x < destination.x + .5f && currentLocation.x > destination.x - .5f &&
        currentLocation.y < destination.y + .5f && currentLocation.y > destination.y - .5f;
    }

    private void MoveTowardsTile(Vector3 currentPosition, Vector3 pathTile, float movementSpeed)
    {
        float step = movementSpeed * Time.deltaTime;
        this.transform.position = Vector3.MoveTowards(currentPosition, pathTile, step);
    }
}
