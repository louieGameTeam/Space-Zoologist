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
    protected MovementController Controller { get; set; }

    /// <summary>
    /// Disables the behavior component right away. All behaviors should call base.Awake first in overriden method.
    /// </summary>
    protected virtual void Awake()
    {
        // This script is attached after the initial start is called for scripts, so these dependencies will already be available in Awake.
        this.Animal = this.gameObject.GetComponent<Animal>();
        this.Controller = this.gameObject.GetComponent<MovementController>();
        this.enabled = false;
    }

    /// <summary>
    /// Grabs necessary references for behaviors. All behaviors should call base.Start first in their overriden method.
    /// </summary>
    protected virtual void Start()
    {
        
    }

    /// <summary>
    /// Implement some sort of behavior logic
    /// </summary>
    protected virtual void Update()
    {

    }

    /// <summary>
    /// Enables the component so the behaviors Update is active. Call base at the end of the overriden method.
    /// </summary>
    public virtual void EnterBehavior(BehaviorFinished callback)
    {
        this.enabled = true;
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