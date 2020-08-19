using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AnimalBehaviorManager : MonoBehaviour
{
    public List<BehaviorData> activeBehaviors = new List<BehaviorData>();
    public List<BehaviorPattern> activeBehaviorPatterns = new List<BehaviorPattern>();
    [SerializeField]
    private List<AnimalBehaviorTrigger> animalBehaviorTriggers = new List<AnimalBehaviorTrigger>();
    // TODO add shader animation support
    [SerializeField]
    private AnimalBehaviorTrigger startUpTrigger = default; // Trigger that activates once at wake

    private void Awake()
    {
        foreach (AnimalBehaviorTrigger animalBehavior in animalBehaviorTriggers)
        {
            //TODO subscribe to all callbacks and events
        }
        if (startUpTrigger)
        {
            startUpTrigger.EnterBehavior(this.gameObject);
        }
        
    }
    /// <summary>
    /// Adds a pattern to this animal
    /// </summary>
    /// <param name="behaviorPattern"></param>
    /// <param name="stepCompletedCallBack"></param>
    /// <param name="collaboratingAnimals"></param>
    public void AddBehaviorPattern(BehaviorPattern behaviorPattern, StepCompletedCallBack stepCompletedCallBack, List<GameObject> collaboratingAnimals = null)
    {
        activeBehaviorPatterns.Add(behaviorPattern);
        behaviorPattern.InitializePattern(this.gameObject, stepCompletedCallBack, collaboratingAnimals);
    }
    /// <summary>
    /// Force exit behavior when overriden by other behaviors. Behavior Pattern handles its exit (Usually just call exit), and ForceExitCallback will be called for behaviors to exit
    /// </summary>
    /// <param name="isDriven">A bool used to call all partners to stop, but do not reference its self back, just leave as default</param>
    public void ForceExit(bool isDriven = false)
    {
        foreach (BehaviorPattern pattern in activeBehaviorPatterns)
        {
            pattern.QueueForForceExit(this.gameObject, isDriven);
        }
        foreach (BehaviorData data in activeBehaviors)
        {
            data.ForceExitCallback.Invoke(this.gameObject);
        }
        activeBehaviors.Clear();
    }

}
