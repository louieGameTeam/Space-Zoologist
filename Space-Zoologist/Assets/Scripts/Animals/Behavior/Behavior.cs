using UnityEngine;

// TODO create more behaviors, e.g., circle object, pace between objects, seek object, etc.
// TODO determine how behavior modifiers (e.g., speed, direction, etc.) should be handled.

/// <summary>
/// Override the virtual methods to create unique behaviors
/// </summary>
public abstract class Behavior : MonoBehaviour
{
    private BehaviorFinished Callback { get; set; }
    // Use Animal to access the BehaviorData, Pathfinder, and PopulationInfo.AccessibleLocations
    protected Animal Animal { get; set; }
    protected MovementController MovementController { get; set; }
    protected AnimalPathfinding.Node PathToDestination { get; set; }
    protected bool isCalculatingPath { get; set; }

    /// <summary>
    /// Disables the behavior component right away. All behaviors should call base.Awake first in overriden method.
    /// </summary>
    protected virtual void Awake()
    {
        // This script is attached after the initial start is called for scripts, so these dependencies will already be available in Awake.
        this.Animal = this.gameObject.GetComponent<Animal>();
        this.MovementController = this.gameObject.GetComponent<MovementController>();
        this.isCalculatingPath = false;
        this.enabled = false;
    }

    /// <summary>
    /// Grabs necessary references for behaviors. All behaviors should call base.Start first in their overriden method.
    /// </summary>
    protected virtual void Start()
    {

    }

    /// <summary>
    /// Implement some sort of behavior logic. Idles while path is being calculated.
    /// </summary>
    protected virtual void Update()
    {

    }

    /// <summary>
    /// Callback given to CalculatePath for when the path finishes being calculated.
    /// Consider modifying the else to calculate a different path if a path isn't found.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pathFound"></param>
    protected virtual void PathFound(AnimalPathfinding.Node path, bool pathFound)
    {
        if (pathFound)
        {
            this.PathToDestination = path;
            this.MovementController.AssignPath(path);
            this.isCalculatingPath = false;
        }
        else
        {
            Debug.Log("Issue with pathfinding, exiting behavior");
            this.ExitBehavior();
        }
    }

    /// <summary>
    /// Should be called in every update before path is used.
    /// </summary>
    /// <returns></returns>
    protected bool IsCalculatingPath()
    {
        if (this.isCalculatingPath)
        {
            // Debug.Log("IsBlocked");
            this.Animal.BehaviorsData.MovementStatus = Movement.idle;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Enables the component so the behaviors Update is active. Call base at the end of the overriden method.
    /// </summary>
    public virtual void EnterBehavior(BehaviorFinished callback)
    {
        this.enabled = true;
        this.isCalculatingPath = true;
        this.Callback = callback;
    }

    /// <summary>
    /// Disables the component so the behaviors Update is inactive. Call base at the beginning of the overriden method.
    /// Can be called by behavior when finished or used to interrupt a behavior.
    /// </summary>
    public virtual void ExitBehavior()
    {
        this.enabled = false;
        if (this.Callback != null)
        {
            this.Callback.Invoke();
        }
    }
}