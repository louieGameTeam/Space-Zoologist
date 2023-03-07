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
    
    protected Dictionary<GameObject, int> animalsToSteps = new();
    protected Dictionary<GameObject, BehaviorCompleteCallback> animalsToBehaviorCompleteCallbacks = new();
    protected Dictionary<GameObject, ForceExitCallback> animalsToForceExitCallbacks = new();
    protected StepCompletedCallBack stepCompletedCallback;

    /// <summary>
    /// Force remove an animal from the behavior. Does not trigger callbacks, as that would
    /// likely lead to starting a new behavior (undesirable for a force exit)
    /// </summary>
    /// <param name="animals"></param>
    public void ForceRemoveAnimals(IEnumerable<GameObject> animals)
    {
        foreach (var animal in animals)
        {
            ForceRemoveAnimal(animal);
        }
    }

    public void ForceRemoveAnimal(GameObject animal)
    {
        RemoveAnimalFromDictionaries(animal);
        animal.GetComponent<AnimalBehaviorManager>().ForceExitPopulationBehavior();
    }
    
    /// <summary>
    /// Called when animal enters behavior
    /// </summary>
    ///th <param name="avalabilityToAnimals"></param>
    public void EnterBehavior(GameObject animal, BehaviorCompleteCallback callback, ForceExitCallback forceExitCallback = null)
    {
        behaviorData.behaviorName = this.name;
        behaviorData.ForceExitCallback = CallForceExitCallback;
        stepCompletedCallback = OnStepCompleted;
        animal.GetComponent<AnimalBehaviorManager>().activeBehavior = behaviorData;
        if (animalsToSteps.ContainsKey(animal))
        {
            Debug.LogError(this.name, animal);
        }
        animalsToSteps.Add(animal, 0);
        animalsToBehaviorCompleteCallbacks.Add(animal, callback);
        animalsToForceExitCallbacks.Add(animal, forceExitCallback);
        ProceedToNext(animal);
    }

    private void ForceExitPopulationBehavior(GameObject animal)
    {
        ExitBehavior(animal);
    }

    protected void CallForceExitCallback(GameObject animal)
    {
        animalsToForceExitCallbacks[animal]?.Invoke(animal);
    }

    /// <summary>
    /// Goes through each pattern in a behavior, removing behavior when finished
    /// </summary>
    /// <param name="animal"></param>
    protected virtual void ProceedToNext(GameObject animal)
    {
        if (animalsToSteps[animal] < behaviorPatterns.Count) // exit behavior when all steps are completed
        {
            // If the alternative condition is met, then exit the entire population behavior
            animal.GetComponent<AnimalBehaviorManager>().AddBehaviorPattern(
                behaviorPatterns[animalsToSteps[animal]], 
                stepCompletedCallback, 
                ExitBehavior);
        }
        else
        {
            ExitBehavior(animal);
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
    
    /// <summary>
    /// Defines how the behavior exits
    /// </summary>
    /// <param name="animal"></param>
    protected virtual void ExitBehavior(GameObject animal)
    {
        // Force out of any behavior patterns from abnormal behaviors
        var behaviorManager = animal.GetComponent<AnimalBehaviorManager>();
        behaviorManager.ForceExitBehaviorPattern();
        behaviorManager.activeBehavior = null;
        
        animalsToBehaviorCompleteCallbacks[animal].Invoke(animal);
        RemoveAnimalFromDictionaries(animal);
    }

    public bool HasAnimal(GameObject animal)
    {
        return animalsToSteps.ContainsKey(animal);
    }

    /// <summary>
    /// Removes an animal from all of the callback dictionaries
    /// </summary>
    private void RemoveAnimalFromDictionaries(GameObject animal)
    {
        if (animalsToSteps.ContainsKey(animal))
        {
            animalsToSteps.Remove(animal);
        }
        if (animalsToBehaviorCompleteCallbacks.ContainsKey(animal))
        {
            animalsToBehaviorCompleteCallbacks.Remove(animal);
        }
        if (animalsToForceExitCallbacks.ContainsKey(animal))
        {
            animalsToForceExitCallbacks.Remove(animal);
        }
    }
}
