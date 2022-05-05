using System.Collections.Generic;
using UnityEngine;

public delegate void BehaviorCompleteCallback(GameObject animal);
public class PopulationBehaviorManager : MonoBehaviour
{
    private Population population = default;
    [SerializeField] public Dictionary<GameObject, int> animalsToBehaviorIndex = new Dictionary<GameObject, int>();
    [SerializeField] public List<PopulationBehavior> defaultBehaviors;
    private BehaviorCompleteCallback BehaviorCompleteCallback;


    public void Initialize()
    {
        this.BehaviorCompleteCallback = this.OnBehaviorComplete;
        // Only initializes callbacks for each behavior *once* per population, even if defaultBehaviors contains duplicates
        List<int> uniqueBehaviorIDs = new List<int>();
        foreach (PopulationBehavior behavior in defaultBehaviors)
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
        animalsToBehaviorIndex.Add(animal, 0);
        defaultBehaviors[0].EnterBehavior(animal, transform.GetSiblingIndex());
    }

    public void RemoveAnimal(GameObject animal)
    {
        animalsToBehaviorIndex.Remove(animal);
        animal.GetComponent<AnimalBehaviorManager>().ForceExit();
    }

    private void OnDestroy() {
        foreach (PopulationBehavior behavior in defaultBehaviors)
        {
            behavior.RemoveCallback(transform.GetSiblingIndex());
        }
    }
}
