using System.Collections.Generic;
using UnityEngine;

public delegate void BehaviorCompleteCallback(GameObject animal);
public class PopulationBehaviorManager : MonoBehaviour
{
    private Population population = default;
    [SerializeField] public Dictionary<GameObject, BehaviorExecutionData> animalsToExecutionData = new Dictionary<GameObject, BehaviorExecutionData>();
    [SerializeField] public List<PopulationBehavior> defaultBehaviors;
    private BehaviorCompleteCallback BehaviorCompleteCallback;

    public void Initialize()
    {
        BehaviorCompleteCallback = OnBehaviorComplete;
        foreach (PopulationBehavior behavior in defaultBehaviors)
        {
            behavior.AssignCallback(BehaviorCompleteCallback);
        }
        this.population = this.gameObject.GetComponent<Population>();
        int j = -1;
        for (int i = 0; i < population.Count; i++)
        {
            j++;
            if (j >= defaultBehaviors.Count)
            {
                j = 0;
            }
            animalsToExecutionData.Add(this.population.AnimalPopulation[i], new BehaviorExecutionData(j));
        }
    }
    
    public void OnBehaviorComplete(GameObject animal)
    {
        if (!animalsToExecutionData.ContainsKey(animal))// Discriminate force exited callbacks from removing animals
        {
            return;
        }
        PopulationBehavior behavior = animalsToExecutionData[animal].NextBehavior(defaultBehaviors);
        if (behavior == null)
        {
            return;
        }
        behavior.EnterBehavior(animal);
    }
    public void RemoveAnimal(GameObject animal)
    {
        animalsToExecutionData.Remove(animal);
        animal.GetComponent<AnimalBehaviorManager>().ForceExit();
    }
}
