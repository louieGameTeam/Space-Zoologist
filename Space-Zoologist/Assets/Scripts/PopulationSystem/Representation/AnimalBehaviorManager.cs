using UnityEngine;


public class AnimalBehaviorManager : MonoBehaviour
{
    public BehaviorData activeBehavior = null;
    public BehaviorPattern activeBehaviorPattern = null;

    /// <summary>
    /// Adds a pattern to this animal
    /// </summary>
    /// <param name="behaviorPattern"></param>
    /// <param name="stepCompletedCallBack"></param>
    /// <param name="collaboratingAnimals"></param>
    public void AddBehaviorPattern(BehaviorPattern behaviorPattern, StepCompletedCallBack stepCompletedCallBack, StepCompletedCallBack alternativeCallback)
    {
        activeBehaviorPattern = behaviorPattern;
        behaviorPattern.InitializePattern(this.gameObject, stepCompletedCallBack, alternativeCallback);
    }
    /// <summary>
    /// Force exit behavior when overridden by other behaviors. Behavior Pattern handles its exit (Usually just call exit), and ForceExitCallback will be called for behaviors to exit
    /// </summary>
    /// <param name="isDriven">A bool used to call all partners to stop, but do not reference its self back, just leave as default</param>
    public void ForceExit()
    {
        if (activeBehaviorPattern != null) // Happens when cooperating animal gets removed
        {
            activeBehaviorPattern.ForceExit(this.gameObject);
            activeBehavior.ForceExitCallback.Invoke(this.gameObject);
            activeBehavior = null;
        }
    }

}
