using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AnimalBehaviorManager : MonoBehaviour
{
    [Header("For testing purposes")]
    public BehaviorData activeBehavior = null;
    public BehaviorPattern activeBehaviorPattern = null;
    // TODO add shader animation support

    /// <summary>
    /// Adds a pattern to this animal
    /// </summary>
    /// <param name="behaviorPattern"></param>
    /// <param name="stepCompletedCallBack"></param>
    /// <param name="collaboratingAnimals"></param>
    public void AddBehaviorPattern(BehaviorPattern behaviorPattern, StepCompletedCallBack stepCompletedCallBack, StepCompletedCallBack alternativeCallback, List<GameObject> collaboratingAnimals = null)
    {
        activeBehaviorPattern = behaviorPattern;
        behaviorPattern.InitializePattern(this.gameObject, stepCompletedCallBack, alternativeCallback, collaboratingAnimals);
    }
    /// <summary>
    /// Force exit behavior when overridden by other behaviors. Behavior Pattern handles its exit (Usually just call exit), and ForceExitCallback will be called for behaviors to exit
    /// </summary>
    /// <param name="isDriven">A bool used to call all partners to stop, but do not reference its self back, just leave as default</param>
    public void ForceExit(bool isDriven = false)
    {
        if (activeBehaviorPattern != null) // Happens when cooperating animal gets removed
        {
            Debug.Log(this.activeBehaviorPattern);
            activeBehaviorPattern.QueueForForceExit(this.gameObject, isDriven);
            activeBehavior.ForceExitCallback.Invoke(this.gameObject);
            activeBehavior = null;
        }
    }

}
