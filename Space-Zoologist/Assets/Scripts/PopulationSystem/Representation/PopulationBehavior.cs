using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For creating more behaviors, inherit like how IdleRandomTrigger is setup
public delegate void StepCompletedCallBack(GameObject gameObject);
[CreateAssetMenu(fileName = "PopulationBehavior", menuName = "SpeciesBehavior/PopulationBehavior")]
public class PopulationBehavior : ScriptableObject
{
    public BehaviorData behaviorData;
    public int numberOfAnimalsRequired = 1;
    public List<BehaviorPattern> behaviorPatterns = default;
    protected Dictionary<GameObject, int> animalsToSteps = new Dictionary<GameObject, int>();
    protected StepCompletedCallBack stepCompletedCallback;
    protected BehaviorCompleteCallback behaviorCompleteCallback;
    public void AssignCallback(BehaviorCompleteCallback callback)
    {
        behaviorCompleteCallback = callback;
    }
    /// <summary>
    /// Called when animal enters behavior
    /// </summary>
    /// <param name="avalabilityToAnimals"></param>
    public void EnterBehavior(GameObject animal)
    {
        behaviorData.behaviorName = this.name;
        behaviorData.ForceExitCallback = RemoveBehavior;
        stepCompletedCallback = OnStepCompleted;
        animal.GetComponent<AnimalBehaviorManager>().activeBehavior = behaviorData;
        animalsToSteps.Add(animal, 0);
        ProceedToNext(animal);
    }

    /// <summary>
    /// Goes through each pattern in a behavior, removing behavior when finished
    /// </summary>
    /// <param name="animal"></param>
    protected virtual void ProceedToNext(GameObject animal)
    {
        if (animalsToSteps[animal] < behaviorPatterns.Count) // exit behavior when all steps are completed
        {
            animal.GetComponent<AnimalBehaviorManager>().AddBehaviorPattern(behaviorPatterns[animalsToSteps[animal]], stepCompletedCallback, RemoveBehavior);
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
        if (animalsToSteps.ContainsKey(animal))
        {
            animalsToSteps.Remove(animal);
            animal.GetComponent<AnimalBehaviorManager>().activeBehavior = null;
            behaviorCompleteCallback.Invoke(animal);
        }
    }
    /// <summary>
    /// Callback function which increases the step count and calls to proceeds to next step
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="collaboratingAnimals"></param>
    protected void OnStepCompleted(GameObject animal)
    {
        animalsToSteps[animal]++;
        ProceedToNext(animal);
    }
}
