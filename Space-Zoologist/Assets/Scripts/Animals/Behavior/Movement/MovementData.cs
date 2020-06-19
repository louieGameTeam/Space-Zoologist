using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Behaviors will define many of the values below in order to define how the animal will move.
/// When a new Behavior is assigned to an animal, this movement data script will be updated with the behavior's defined values and the controller will be notified.
/// This script also keeps track of data produced by the animal moving: Direction, MovementStatus, CurrentLocation, etc.
/// </summary>
public enum Movement { idle = 0, walking = 1, running = 2 }
public enum Direction { up = 0, down = 1, left = 2, right = 3 }
public class MovementData : MonoBehaviour
{
    [Header("Used to find accessible locations and translate Vector3 locations")]
    [SerializeField] public Population PopulationInfo = default;
    [SerializeField] private Tilemap Mask = default;
    public List<Vector3Int> AccessibleLocations { get; private set; }

    // Current MovementData 
    public Movement MovementStatus { get; private set; }
    public Direction CurrentDirection { get; private set; }
    public Vector3 CurrentLocation { get; private set; }

    private MovementController MovementController = default;

    // TODO define behavior interface with the public fields below this
    [Header("Behavior that will be displayed by movement/animator controller")]
    [SerializeField] public GameObject CurrentBehavior = default;

    public bool ShouldMove { get; private set; }
    public bool ShouldLoop { get; private set; }
    public bool ShouldStop { get; private set; }

    public float MovementSpeed { get; private set; }
    public float IdleLength { get; private set; }
    public float RunThreshold { get; private set; }

    public bool RandomizeMovementSpeed { get; private set; }
    public bool RandomizeIdleLength { get; private set; }

    // Will end up on a behavior
    [Header("Add gameobject waypoints to define a path")]
    [Tooltip("Moves towards gameobjects one after the other in a loop. Lock this window to select multiple gameobjects and drag them here all at once.")]
    [SerializeField] public List<GameObject> PredefinedPathToFollow = default;

    void Start()
    {
        ReservePartitionManager.ins.AddPopulation(this.PopulationInfo);
        // TODO talk to alex about how this will work if a gameobject is instantiated outside it's accessible area, since right now it doesn't work
        this.AccessibleLocations = ReservePartitionManager.ins.GetLocationWithAccess(this.PopulationInfo);
        this.MovementController = this.gameObject.GetComponent<MovementController>();
        this.MovementController.Initialize(this);
    }

    public void Update()
    {
        this.CurrentDirection = this.MovementController.CurrentDirection;
        this.MovementStatus = this.MovementController.MovementStatus;
        this.CurrentLocation = this.gameObject.transform.position;
    }

    // Functions to translate data

    public Vector3Int WorldToCell(Vector3 position)
    {
        return this.Mask.WorldToCell(position);
    }
}
