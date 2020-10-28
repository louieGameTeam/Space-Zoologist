using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public delegate void BehaviorCompleteCallback(GameObject animal);
public class PopulationBehaviorManager : MonoBehaviour
{
    public Dictionary<string, PopulationBehavior> ActiveBehaviors = new Dictionary<string, PopulationBehavior>();
    private Population population = default;
    [SerializeField] public Dictionary<GameObject, BehaviorExecutionData> animalToExecutionData = new Dictionary<GameObject, BehaviorExecutionData>();
    [SerializeField] public List<PopulationBehavior> tempBehaviors = new List<PopulationBehavior>();
    [SerializeField] private PopulationBehavior defaultBehavior;
    private DequeueCoordinatedBehavior DequeueCoordinatedBehavior;
    private BehaviorCompleteCallback BehaviorCompleteCallback;

    public void Initialize()
    {
        DequeueCoordinatedBehavior = OnDequeue;
        BehaviorCompleteCallback = OnBehaviorComplete;
        foreach (PopulationBehavior behavior in tempBehaviors)
        {
            behavior.AssignCallback(BehaviorCompleteCallback);
        }
        this.population = this.gameObject.GetComponent<Population>();
        int j = -1;
        for (int i = 0; i < population.Count; i++)
        {
            j++;
            if (j >= tempBehaviors.Count)
            {
                j = 0;
            }
            animalToExecutionData.Add(this.population.AnimalPopulation[i], new BehaviorExecutionData(j));
        }
        foreach (GameObject animal in this.population.AnimalPopulation)
        {
            if (tempBehaviors[animalToExecutionData[animal].currentBehaviorIndex].numberOfAnimalsRequired == 1)
            {
                List<GameObject> animals = new List<GameObject>();
                animals.Add(animal);
                tempBehaviors[animalToExecutionData[animal].currentBehaviorIndex].EnterBehavior(animals);
                continue;
            }
            QueueGroupBehavior(animal, animalToExecutionData[animal].currentBehaviorIndex, tempBehaviors[animalToExecutionData[animal].currentBehaviorIndex].numberOfAnimalsRequired);
        }
    }
    private void QueueGroupBehavior(GameObject initiator, int behaviorIndex, int numToFind)
    {
        animalToExecutionData[initiator].pendingBehavior = tempBehaviors[behaviorIndex];
        animalToExecutionData[initiator].cooperatingAnimals.Add(initiator);// Add self to list
        int numFound = 1;
        int maxQueueLength = 0;
        while (numFound != numToFind || numToFind > animalToExecutionData.Count)
        {
            if (maxQueueLength == 5) //Queue too long, skip Group behavior. The queue length actually stabilizes between 0 and 2, but just in case it exceeds for whatever reason
            {
                break;
            }
            for (int i = 1; i < tempBehaviors.Count; i++) //Prioritizes preceding behaviors to avoid clustering of behaviors
            {
                int currentIndex = behaviorIndex - i - 1;
                if (currentIndex < 0)
                {
                    currentIndex += tempBehaviors.Count();
                }
                foreach (KeyValuePair<GameObject, BehaviorExecutionData> animalsToData in animalToExecutionData)
                {
                    if (animalsToData.Key == initiator)//Skip self(It will not find itself anyways, but just in case)
                    {
                        continue;
                    }
                    if (animalsToData.Value.currentBehaviorIndex == currentIndex)
                    {
                        if (animalsToData.Value.QueueBehavior(DequeueCoordinatedBehavior, tempBehaviors[behaviorIndex], initiator, maxQueueLength)) //Queue is small enough to be added
                        {
                            animalToExecutionData[initiator].cooperatingAnimals.Add(animalsToData.Key);
                            numFound++;
                            if(numFound == numToFind)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            maxQueueLength++;
        }
        // When not enough animal or queue too long, go to next behavior
        animalToExecutionData[initiator].pendingBehavior = null;
        animalToExecutionData[initiator].cooperatingAnimals.Clear();
        OnBehaviorComplete(initiator);
    }
    private void OnDequeue(GameObject initiator)
    {
        animalToExecutionData[initiator].avaliableAnimalCount++;
        if (animalToExecutionData[initiator].avaliableAnimalCount == animalToExecutionData[initiator].pendingBehavior.numberOfAnimalsRequired) //last animal ready will trigger the behavior
        {
            animalToExecutionData[initiator].avaliableAnimalCount = 1;
            animalToExecutionData[initiator].pendingBehavior.EnterBehavior(animalToExecutionData[initiator].cooperatingAnimals);
            animalToExecutionData[initiator].cooperatingAnimals.Clear();
        }
    }
    private void OnBehaviorComplete(GameObject animal)
    {
        PopulationBehavior behavior = animalToExecutionData[animal].NextBehavior(tempBehaviors, defaultBehavior);
        if (behavior == null)
        {
            return;
        }
        if (behavior.numberOfAnimalsRequired == 1)
        {
            List<GameObject> animals = new List<GameObject>();
            animals.Add(animal);
            tempBehaviors[animalToExecutionData[animal].currentBehaviorIndex].EnterBehavior(animals);
            return;
        }
        QueueGroupBehavior(animal, animalToExecutionData[animal].currentBehaviorIndex, behavior.numberOfAnimalsRequired);
    }
    // If there's a bad condition behavior, initialize to that. Otherwise initialize to null.
    public void InitializeBehaviors(Dictionary<string, Need> _needs)
    {
        foreach (KeyValuePair<string, Need> needs in _needs)
        {
            foreach (NeedBehavior needBehavior in needs.Value.Behaviors)
            {
                if (needBehavior.Condition.Equals(NeedCondition.Bad))
                {
                    if (!this.ActiveBehaviors.ContainsKey(needs.Key))
                    {
                        if (needBehavior.Behavior != null)
                        {
                            this.ActiveBehaviors.Add(needs.Key, needBehavior.Behavior);
                        }
                        else
                        {
                            this.ActiveBehaviors.Add(needs.Key, null);
                        }
                    }
                }
            }
        }
    }
}
