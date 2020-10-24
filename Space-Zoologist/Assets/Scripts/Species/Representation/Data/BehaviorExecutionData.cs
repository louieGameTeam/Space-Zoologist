using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DequeueCoordinatedBehavior(GameObject cardinal);
public class BehaviorExecutionData
{
    public int currentBehaviorIndex = 0;
    public int avaliableAnimalCount = 1;
    public Queue<KeyValuePair<DequeueCoordinatedBehavior, GameObject>> QueuedCoordinatedBehaviorsToInitiators = new Queue<KeyValuePair<DequeueCoordinatedBehavior, GameObject>>();
    public SpecieBehaviorTrigger pendingBehavior = null; // filled with behavior if the corresponding gameobject is initiating a coordinated behavior
    public List<GameObject> cooperatingAnimals = new List<GameObject>();
    public List<SpecieBehaviorTrigger> behaviorsToSkip = new List<SpecieBehaviorTrigger>();// Avoids behaviors involving multiple animals to be displayed more frequent than usual
    public BehaviorExecutionData(int behaviorIndex)
    {
        currentBehaviorIndex = behaviorIndex;
    }
    public bool QueueBehavior(DequeueCoordinatedBehavior dequeueCoordinatedBehavior, SpecieBehaviorTrigger specieBehaviorTrigger, GameObject initiator, int maxQueueLength)
    {
        if (QueuedCoordinatedBehaviorsToInitiators.Count <= maxQueueLength)
        {
            KeyValuePair<DequeueCoordinatedBehavior, GameObject> newPair = new KeyValuePair<DequeueCoordinatedBehavior, GameObject>(dequeueCoordinatedBehavior, initiator);
            behaviorsToSkip.Add(specieBehaviorTrigger);
            QueuedCoordinatedBehaviorsToInitiators.Enqueue(newPair);
            return true;
        }
        return false;
    }
    public SpecieBehaviorTrigger NextBehavior(List<SpecieBehaviorTrigger> behaviors, SpecieBehaviorTrigger defaultBehavior)
    {
        if (QueuedCoordinatedBehaviorsToInitiators.Count != 0)
        {
            Debug.Log("queued");
            KeyValuePair<DequeueCoordinatedBehavior, GameObject> keyValuePair = QueuedCoordinatedBehaviorsToInitiators.Dequeue();
            keyValuePair.Key.Invoke(keyValuePair.Value);
            return null;
        }
        currentBehaviorIndex++;
        if (currentBehaviorIndex >= behaviors.Count)
        {
            currentBehaviorIndex = 0;// Start another loop if finished list OR list reduced
            if (behaviors.Count == 0)
            {
                Debug.LogError("Behavior list is empty, entering default behavior");
                return defaultBehavior;
            }
        }
        if (behaviorsToSkip.Contains(behaviors[currentBehaviorIndex])) // Skip behavior executed with other animal
        {
            behaviorsToSkip.Remove(behaviors[currentBehaviorIndex]);
            return NextBehavior(behaviors, defaultBehavior);
        }
        return behaviors[currentBehaviorIndex];
    }
}
