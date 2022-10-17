using System.Collections.Generic;
using UnityEngine;

public delegate void BehaviorCompleteCallback(GameObject animal);
public class PopulationBehaviorManager : MonoBehaviour
{
    private Population population = default;
    [SerializeField] public Dictionary<GameObject, int> animalsToBehaviorIndex = new Dictionary<GameObject, int>();
    [Header("Behaviors")]
    [SerializeField] public PopulationBehavior spawnBehavior;
    [SerializeField] public PopulationBehavior despawnBehavior;
    [SerializeField] public List<PopulationBehavior> defaultBehaviors;
    private BehaviorCompleteCallback BehaviorCompleteCallback;
    private BehaviorCompleteCallback despawnCompleteCallback;


    public void Initialize()
    {
        this.BehaviorCompleteCallback = this.OnBehaviorComplete;
        // Only initializes callbacks for each behavior *once* per population, even if defaultBehaviors contains duplicates
        List<int> uniqueBehaviorIDs = new List<int>();
        foreach (PopulationBehavior behavior in GetAllUsedBehaviors())
        {
            if (!uniqueBehaviorIDs.Contains(behavior.GetInstanceID())){
                behavior.AssignCallback(this.BehaviorCompleteCallback);
            }
            uniqueBehaviorIDs.Add (behavior.GetInstanceID());
        }
        this.population = this.gameObject.GetComponent<Population>();
        int currentBehaviorIndex = 0;
        foreach (GameObject animal in population.AnimalPopulation)
        {
            animalsToBehaviorIndex.Add(animal, currentBehaviorIndex);
            currentBehaviorIndex++;
        }
    }

    string readableDict(Dictionary<GameObject, int> dict) {
        string output = "";
        foreach (var pair in animalsToBehaviorIndex) {
            output += pair.Key.GetInstanceID() + ": " + pair.Value.ToString() + "\n";
        }
        return output;
    }
    
    public void OnBehaviorComplete(GameObject animal)
    {
        if (!animalsToBehaviorIndex.ContainsKey(animal))// Discriminate force exited callbacks from removing animals
        {
            return;
        }
        if(animalsToBehaviorIndex[animal] == -1)
        {
            FinishDespawnAnimal(animal);
            return;
        }
        int nextBehaviorIndex = animalsToBehaviorIndex[animal] + 1;
        nextBehaviorIndex = nextBehaviorIndex >= defaultBehaviors.Count ? 0 : nextBehaviorIndex;
        animalsToBehaviorIndex[animal] = nextBehaviorIndex;
        PopulationBehavior behavior = defaultBehaviors[nextBehaviorIndex];
        if (behavior == null)
        {
            return;
        }
        behavior.EnterBehavior(animal, transform.GetSiblingIndex());
    }
    
    public void AddAnimal(GameObject animal)
    {
        bool spawnBehaviorExists = (spawnBehavior != null);
        // If spawn behavior, then set the current behavior index to the total
        // so that it iterates to 0 after spawn behavior ends
        animalsToBehaviorIndex.Add(animal, spawnBehaviorExists ? defaultBehaviors.Count - 1 : 0);
        if(spawnBehaviorExists)
            spawnBehavior.EnterBehavior(animal, transform.GetSiblingIndex());
        else
            defaultBehaviors[0].EnterBehavior(animal, transform.GetSiblingIndex());
    }

    public void SetDespawnCallback(BehaviorCompleteCallback callback)
    {
        despawnCompleteCallback = callback;
    }

    public void StartDespawnAnimal(GameObject animal)
    {
        if(despawnBehavior == null)
        {
            FinishDespawnAnimal(animal);
        }
        else if(!despawnBehavior.HasAnimal(animal))
        {
            despawnBehavior.EnterBehavior(animal, transform.GetSiblingIndex());
            animalsToBehaviorIndex[animal] = -1;
        }
    }

    private void FinishDespawnAnimal(GameObject animal)
    {
        Debug.Log("REMOVE");
        RemoveAnimal(animal);
        despawnCompleteCallback(animal);
    }

    
    private void RemoveAnimal(GameObject animal)
    {
        animalsToBehaviorIndex.Remove(animal);
        animal.GetComponent<AnimalBehaviorManager>().ForceExit();
    }

    private void OnDestroy() {
        foreach (PopulationBehavior behavior in GetAllUsedBehaviors())
        {
            behavior.RemoveCallback(transform.GetSiblingIndex());
        }
    }

    public IEnumerable<PopulationBehavior> GetAllUsedBehaviors()
    {
        if (spawnBehavior != null)
            yield return spawnBehavior;
        foreach (var behavior in defaultBehaviors)   
            yield return behavior;
        if (despawnBehavior)
            yield return despawnBehavior;
    }
}
