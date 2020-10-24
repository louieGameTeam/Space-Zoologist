using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimalBehaviorTrigger : ScriptableObject
{
    public BehaviorData behaviorData;
    public int maxAffectedNumber = default;
    [Range(0, 1)]
    public float maxAffectedPortion = default;
    public float refreshPeriod = default;
    [HideInInspector]
    public float elapsedTime = 0;
    public int numberTriggerdPerLoop = 1;
    public List<BehaviorPattern> behaviorPatterns = default;
    protected Dictionary<GameObject, int> animalsToSteps = new Dictionary<GameObject, int>();
    protected StepCompletedCallBack stepCompletedCallback;
    protected StepCompletedCallBack alternativeCallback;
    /// <summary>
    /// Called by a event
    /// </summary>
    /// <param name="animal"></param>
    public void EnterBehavior(GameObject animal)
    {
        behaviorData.behaviorName = this.GetType().ToString();
        behaviorData.ForceExitCallback = OnForceExit;
        stepCompletedCallback = OnStepCompleted; // Setup Callback
        Initialize(animal);
    }
    /// <summary>
    /// Initialization, Adding animal to dictionary, and add behavior data to animal behavior manager
    /// </summary>
    /// <param name="animal">The animal is being processed</param>
    protected virtual void Initialize(GameObject animal)
    {
        animal.GetComponent<AnimalBehaviorManager>().activeBehaviors.Add(behaviorData);
        animalsToSteps.Add(animal, 0);
        ProceedToNext(animal);
    }
    /// <summary>
    /// Called every "step", decides what to do after a step is completed. By default, it proceeds to next step.
    /// Can also set to loop by overriding
    /// </summary>
    /// <param name="animal"></param>
    protected virtual void ProceedToNext(GameObject animal) //Looping can be achieved by overriding this function, as well as synchronization among all animals
    {
        if (animalsToSteps[animal] < behaviorPatterns.Count) // exit behavior when all steps are completed
        {
            animal.GetComponent<AnimalBehaviorManager>().AddBehaviorPattern(behaviorPatterns[animalsToSteps[animal]], stepCompletedCallback, alternativeCallback);
        }
        else
        {
            RemoveBehavior(animal);
        }
    }
    /// <summary>
    /// Defines how the behavior exits
    /// </summary>
    /// <param name="animal"></param>
    protected virtual void RemoveBehavior(GameObject animal)
    {
        animalsToSteps.Remove(animal);
        animal.GetComponent<AnimalBehaviorManager>().activeBehaviors.Remove(behaviorData);
    }
    /// <summary>
    /// Callback function which increases the step count and calls to proceeds to next step
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="collaboratingAnimals">Having this just to fit the format, should always be null</param>
    protected void OnStepCompleted(GameObject animal, List<GameObject> collaboratingAnimals = null)
    {
        animalsToSteps[animal]++;
        ProceedToNext(animal);
    }
    /// <summary>
    /// Define what needs to be done when this behavior is force exiting, usually just call remove behavior
    /// </summary>
    /// <param name="animal"></param>
    protected virtual void OnForceExit(GameObject animal)
    {
        RemoveBehavior(animal);
    }
}
