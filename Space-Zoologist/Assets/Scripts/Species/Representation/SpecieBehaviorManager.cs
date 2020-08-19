using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public enum Availability { Free, Concurrent, Override, Occupied }
public class SpecieBehaviorManager : MonoBehaviour
{
    public List<SpecieBehaviorTrigger> behaviorTriggers = default;
    [SerializeField]
    private List<GameObject> animals = default;
    public Dictionary<GameObject, List<string>> animalToActiveBehaviors = new Dictionary<GameObject, List<string>>();
    public bool isPaused = false;

    private void Awake()
    {
        BehaviorPatternUpdater updater = FindObjectOfType<BehaviorPatternUpdater>();
        animals = GetComponent<Population>().AnimalPopulation;
        foreach (SpecieBehaviorTrigger behaviorTrigger in behaviorTriggers)
        {
            foreach (BehaviorPattern behaviorPattern in behaviorTrigger.behaviorPatterns)
            {
                updater.RegisterPattern(behaviorPattern);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isPaused)
        {
            foreach (SpecieBehaviorTrigger specieBehaviorTrigger in behaviorTriggers)
            {
                if (specieBehaviorTrigger.IsConditionSatisfied())
                {
                    specieBehaviorTrigger.ResetCondition();
                    Trigger(specieBehaviorTrigger);
                }
            }
        }
    }
    private void Trigger(SpecieBehaviorTrigger trigger)
    {
        int maxNumberApplicable = Mathf.Min(trigger.maxAffectedNumber, Mathf.RoundToInt(trigger.maxAffectedPortion * animals.Count));
        if (maxNumberApplicable < trigger.numberTriggerdPerLoop) // Not enough animals to perform behavior
        {
            //Debug.Log("Return, Not enough animals to perform behavior");
            return;
        }
        int numberAlreadyRunningBehavior = 0;
        Dictionary<Availability, List<GameObject>> avalabilityToAnimals = new Dictionary<Availability, List<GameObject>>();
        foreach (Availability availability in Enum.GetValues(typeof(Availability))) //Construct dictionary
        {
            avalabilityToAnimals.Add(availability, new List<GameObject>());
        }
        foreach (GameObject animal in animals)
        {
            List<BehaviorData> activeBehaviors = animal.GetComponent<AnimalBehaviorManager>().activeBehaviors;
            if (activeBehaviors.Count == 0) //No behavior
            {
                avalabilityToAnimals[Availability.Free].Add(animal);
                //Debug.Log("Free Animal Found");
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
                    if (animalBehaviorData.priority > trigger.behaviorData.priority) // Add lower priority to list
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
