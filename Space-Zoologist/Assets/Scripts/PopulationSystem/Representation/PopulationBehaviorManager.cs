using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void BehaviorCompleteCallback(GameObject animal);
public class PopulationBehaviorManager : MonoBehaviour
{

    [Header("Behaviors")]
    [SerializeField] public PopulationBehavior spawnBehavior;
    [SerializeField] public PopulationBehavior despawnBehavior;
    [SerializeField] public List<PopulationBehavior> defaultBehaviors;

    public List<GameObject> animals;
    public List<int> behaviorIndex;

    // Private fields
    private Population population = default;
    private Dictionary<GameObject, int> animalsToBehaviorIndex = new Dictionary<GameObject, int>();
    
    // Callbacks
    private BehaviorCompleteCallback behaviorCompleteCallback;
    private BehaviorCompleteCallback despawnCompleteCallback;


    public void Initialize()
    {
        behaviorCompleteCallback = OnBehaviorComplete;
        population = gameObject.GetComponent<Population>();
        foreach (GameObject animal in population.AnimalPopulation)
        {
            animalsToBehaviorIndex.Add(animal, 0);
        }
    }

    void Update()
    {
        animals = animalsToBehaviorIndex.Keys.ToList();
        behaviorIndex = animalsToBehaviorIndex.Values.ToList();
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

    public void OnBehaviorComplete(GameObject animal)
    {
        if (!animalsToBehaviorIndex.ContainsKey(animal))// Discriminate force exited callbacks from removing animals
        {
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
        behavior.EnterBehavior(animal, behaviorCompleteCallback);
    }
    
    public void AddAnimal(GameObject animal)
    {
        bool spawnBehaviorExists = (spawnBehavior != null);
        // If spawn behavior, then set the current behavior index to -1
        // so that it iterates to 0 after spawn behavior ends
        animalsToBehaviorIndex.Add(animal, spawnBehaviorExists ? -1 : 0);
        if(spawnBehaviorExists)
            spawnBehavior.EnterBehavior(animal, behaviorCompleteCallback);
        else
            defaultBehaviors[0].EnterBehavior(animal, behaviorCompleteCallback);
    }

    public void SetDespawnCallback(BehaviorCompleteCallback callback)
    {
        despawnCompleteCallback = callback;
    }

    /// <summary>
    /// Force exit the animal from its current behavior and enter its despawn behavior
    ///  Use a despawn callback provided elsewhere
    /// </summary>
    /// <param name="animal"></param>
    public void StartDespawnAnimal(GameObject animal)
    {
        if(despawnBehavior == null)
        {
            FinishDespawnAnimal(animal);
        }
        else if(!despawnBehavior.HasAnimal(animal))
        {
            var currentBehavior = GetAnimalCurrentPopulationBehavior(animal);
            currentBehavior.ForceRemoveAnimal(animal);
            despawnBehavior.EnterBehavior(animal, FinishDespawnAnimal);
        }
    }

    private PopulationBehavior GetAnimalCurrentPopulationBehavior(GameObject animal)
    {
        int index = animalsToBehaviorIndex[animal];
        if (index == -1)
            return spawnBehavior;
        return defaultBehaviors[index];
    }

    private void FinishDespawnAnimal(GameObject animal)
    {
        RemoveAnimal(animal);
        despawnCompleteCallback(animal);
    }

    
    private void RemoveAnimal(GameObject animal)
    {
        animalsToBehaviorIndex.Remove(animal);
        animal.GetComponent<AnimalBehaviorManager>().ForceExitPopulationBehavior();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            foreach (PopulationBehavior behavior in GetAllUsedBehaviors())
            {
                behavior.ForceRemoveAnimal(child);
            }
        }
    }

    private string DictToDisplayString(Dictionary<GameObject, int> dict) {
        string output = "";
        foreach (var pair in animalsToBehaviorIndex) {
            output += pair.Key.GetInstanceID() + ": " + pair.Value.ToString() + "\n";
        }
        return output;
    }
}
