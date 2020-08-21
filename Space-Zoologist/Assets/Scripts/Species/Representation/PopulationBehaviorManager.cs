using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum Availability { Free, Concurrent, Override, Occupied }
public class PopulationBehaviorManager : MonoBehaviour
{
    public Dictionary<string, SpecieBehaviorTrigger> ActiveBehaviors = new Dictionary<string, SpecieBehaviorTrigger>();
    private Population population = default;
    public Dictionary<GameObject, List<string>> animalToActiveBehaviors = new Dictionary<GameObject, List<string>>();
    // Remove when finished testing
    [Header("For reference only")]
    [SerializeField] private List<SpecieBehaviorTrigger> activeBehaviors = default;
    public bool isPaused = false;

    private void Start()
    {
        this.population = this.gameObject.GetComponent<Population>();
    }

    public void InitializeBehaviors(Dictionary<string, Need> _needs)
    {
        foreach (KeyValuePair<string, Need> needs in _needs)
        {
            foreach (NeedBehavior needBehavior in needs.Value.Behaviors)
            {
                if (!this.ActiveBehaviors.ContainsKey(needs.Key))
                {
                    this.ActiveBehaviors.Add(needs.Key, null);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            // TODO figure out a better way to filter the activebehaviors for testing
            activeBehaviors.Clear();
            foreach (KeyValuePair<string, SpecieBehaviorTrigger> specieBehaviorTrigger in this.ActiveBehaviors)
            {
                if (specieBehaviorTrigger.Value != null && specieBehaviorTrigger.Value.IsConditionSatisfied())
                {
                    specieBehaviorTrigger.Value.ResetCondition();
                    activeBehaviors.Add(specieBehaviorTrigger.Value);
                    Trigger(specieBehaviorTrigger.Value);
                }
            }
        }
    }

    /// <summary>
    /// Checks if there are enough animals to perform behavior, then checks for conflicts between animals running a behavior and the triggered behavior,
    /// then checks for all available animals and sends that list to the behavior
    /// </summary>
    /// <param name="trigger"></param>
    /// TODO refactor animal.GetComponent out
    private void Trigger(SpecieBehaviorTrigger trigger)
    {
        int maxNumberApplicable = Mathf.Min(trigger.maxAffectedNumber, Mathf.RoundToInt(trigger.maxAffectedPortion * this.population.Count));
        if (maxNumberApplicable < trigger.numberTriggerdPerLoop) // Not enough animals to perform behavior
        {
            Debug.Log("Return, Not enough animals to perform behavior");
            return;
        }
        int numberAlreadyRunningBehavior = 0;
        Dictionary<Availability, List<GameObject>> avalabilityToAnimals = new Dictionary<Availability, List<GameObject>>();
        foreach (Availability availability in Enum.GetValues(typeof(Availability))) //Construct dictionary
        {
            avalabilityToAnimals.Add(availability, new List<GameObject>());
        }
        foreach (GameObject animal in this.population.AnimalPopulation)
        {
            List<BehaviorData> activeBehaviors = animal.GetComponent<AnimalBehaviorManager>().activeBehaviors;
            if (activeBehaviors.Count == 0) //No behavior
            {
                avalabilityToAnimals[Availability.Free].Add(animal);
                // Debug.Log("Free Animal Found");
                continue;
            }
            if (activeBehaviors.Contains(trigger.behaviorData)) // Same behavior
            {
                numberAlreadyRunningBehavior++;
                if (maxNumberApplicable - numberAlreadyRunningBehavior < trigger.numberTriggerdPerLoop) // Already enough running the behavior
                {
                    //Debug.Log("Return, Already enough running the behavior");
                    return;
                }
                continue;
            }
            if (BehaviorUtils.IsBehaviorConflicting(activeBehaviors, trigger.behaviorData)) // Determine if behavior is conflicting
            {
                bool isOverriding = true;
                foreach (BehaviorData animalBehaviorData in activeBehaviors)
                {
                    if (animalBehaviorData.priority >= trigger.behaviorData.priority) // Add lower priority to list
                    {
                        isOverriding = false;
                        break;
                    }
                }
                if (isOverriding)
                {
                    avalabilityToAnimals[Availability.Override].Add(animal);
                }
                continue;
            }
            else
            {
                avalabilityToAnimals[Availability.Concurrent].Add(animal); // Add concurrent possible animals
                continue;
            }
        }
        int totalAvaliable = 0;
        foreach (Availability key in avalabilityToAnimals.Keys)
        {
            totalAvaliable += avalabilityToAnimals[key].Count();
        }
        if (totalAvaliable < trigger.numberTriggerdPerLoop)
        {
            return;
        }
        trigger.EnterBehavior(avalabilityToAnimals);
    }
}
