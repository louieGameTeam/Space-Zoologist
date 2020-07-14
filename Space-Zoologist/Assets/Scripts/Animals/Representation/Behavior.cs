using UnityEngine;
using System.Collections.Generic;

// TODO create more behaviors, e.g., circle object, pace between objects, seek object, etc.
// TODO determine how behavior modifiers (e.g., speed, direction, etc.) should be handled.

/// <summary>
/// Provides base functionality for entering and exiting behavior and calculating a path.
/// </summary>
public abstract class Behavior : MonoBehaviour
{
    private BehaviorFinished Callback { get; set; }
    // Use Animal to access the BehaviorData, Pathfinder, and PopulationInfo.AccessibleLocations
    protected Animal Animal { get; set; }
    protected MovementController MovementController { get; set; }
    protected bool isCalculatingPath { get; set; }

    /// <summary>
    /// Sets up dependencies and disables the behavior component. All behaviors should call base.Awake first in overriden method.
    /// </summary>
    protected virtual void Awake()
    {
        // Behavior scripts are created and attached after the initial start, so these dependencies should already be available in Awake.
        this.Animal = this.gameObject.GetComponent<Animal>();
        this.MovementController = this.gameObject.GetComponent<MovementController>();
        this.isCalculatingPath = false;
        this.enabled = false;
    }

    /// <summary>
    /// Grabs necessary references for behaviors.
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
    /// Consider modifying the else to do something else if a path isn't found.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="pathFound"></param>
    protected virtual void PathFound(List<Vector3> path, bool pathFound)
    {
        if (pathFound)
        {
            this.MovementController.AssignPath(path);
            this.isCalculatingPath = false;
        }
        else
        {
            Debug.Log("Path not found, exiting behavior without callback");
            this.enabled = false;
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
    /// Enables the component so the behaviors Update is active. Best to call base at the end of the overriden method.
    /// </summary>
    public virtual void EnterBehavior(BehaviorFinished callback)
    {
        this.enabled = true;
        this.isCalculatingPath = true;
        this.Callback = callback;
    }

    /// <summary>
    /// Disables the component so the behaviors Update is inactive. Best to call base at the beginning of the overriden method.
    /// Can also be called by behavior when finished or used to interrupt a behavior.
    /// </summary>
    public virtual void ExitBehavior()
    {
        this.enabled = false;
        if (this.Callback != null)
        {
            this.Callback.Invoke();
        }
        else
        {
            Debug.Log("Callback null, behavior exited");
        }
    }
}