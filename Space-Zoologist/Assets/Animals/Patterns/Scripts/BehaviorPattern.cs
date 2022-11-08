using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPattern : MonoBehaviour
{
    public Dictionary<GameObject, AnimalCallbackData> AnimalsToAnimalData; // The dictionary that holds all animal gameObjects to their data. If you want custom data to be stored, add another dictionary like this one
    protected TileDataController TileDataController;
    private List<GameObject> completedAnimals; //Lists to remove animals from updating before updating to avoid modifying while iterating
    private List<GameObject> alternativeCompletedAnimals;
    private List<GameObject> forceRemoveAnimals;
    
    public virtual void Init()
    {
        TileDataController = GameManager.Instance.m_tileDataController;
        this.AnimalsToAnimalData = new Dictionary<GameObject, AnimalCallbackData>();
        this.completedAnimals = new List<GameObject>();
        this.alternativeCompletedAnimals = new List<GameObject>();
        this.forceRemoveAnimals = new List<GameObject>();
    }
    /// <summary>
    /// Assign necessary data to this script
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="callBack"></param>
    /// <param name="collaboratingAnimals"></param>
    public void InitializePattern(GameObject animal, StepCompletedCallBack callBack, StepCompletedCallBack alternativeCallback)
    {
        AnimalCallbackData animalCallbackData = new AnimalCallbackData();
        animalCallbackData.animal = animal.GetComponent<Animal>();
        animalCallbackData.callback = callBack;
        animalCallbackData.alternativeCallback = alternativeCallback;
        
        AnimalsToAnimalData.Add(animal, animalCallbackData);
        EnterPattern(animal, animalCallbackData);
    }
    /// <summary>
    /// Executes once after initialization, override if you have additional initializations
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="animalCallbackData"></param>
    protected virtual void EnterPattern(GameObject animal, AnimalCallbackData animalCallbackData)
    {

    }
    /// <summary>
    /// Called each frame by the updater, handles completions and exits all together to avoid modifying while iterating
    /// </summary>
    public void UpdatePattern()
    {
        foreach (GameObject animal in forceRemoveAnimals)
        {
            ForceExit(animal);
        }
        forceRemoveAnimals.Clear();
        foreach (GameObject animal in AnimalsToAnimalData.Keys)
        {
            if (IsPatternFinishedAfterUpdate(animal, AnimalsToAnimalData[animal]))
            {
                // Debug.Log("Pattern finished after updating for " + animal.name);
                completedAnimals.Add(animal);
            }
            else if (IsAlternativeConditionSatisfied(animal, AnimalsToAnimalData[animal]))
            {
                // Debug.Log("Alternate exit condition satisfied for " + animal.name);
                alternativeCompletedAnimals.Add(animal);
                continue;
            }
        }
        foreach (GameObject animal in completedAnimals)
        {
            ExitPattern(animal);
        }
        foreach (GameObject animal in alternativeCompletedAnimals)
        {
            ExitPatternAlternative(animal);
        }
        alternativeCompletedAnimals.Clear();
        completedAnimals.Clear();
    }

    /// <summary>
    /// Executed every update, applies update and returns true if the pattern is completed
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="animalCallbackData"></param>
    /// <returns>Return True if animal has completed the pattern</returns>
    protected virtual bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        return true;
    }
    /// <summary>
    /// Return true if an alternative condition is satisfied (Do not update)
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="animalCallbackData"></param>
    /// <returns></returns>
    protected virtual bool IsAlternativeConditionSatisfied(GameObject animal, AnimalCallbackData animalCallbackData)
    {
        return false;
    }
    /// <summary>
    /// Actions taken upon completion of pattern. All following are generally necessary when exit. It is recommended to call base when overridden.
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="isCallingCallback">Set to false when force exiting without completion, leave as default</param>
    protected virtual void ExitPattern(GameObject animal, bool callCallback = true)
    {
        animal.GetComponent<AnimalBehaviorManager>().activeBehaviorPattern = null;
        StepCompletedCallBack callback = 
            AnimalsToAnimalData.ContainsKey(animal) ? 
            AnimalsToAnimalData[animal].callback : null;

        AnimalsToAnimalData.Remove(animal);
        if (callCallback)
        {
            callback?.Invoke(animal);
        }
    }
    protected virtual void ExitPatternAlternative(GameObject animal)
    {
        animal.GetComponent<AnimalBehaviorManager>().activeBehaviorPattern = null;
        AnimalCallbackData animalCallback = AnimalsToAnimalData[animal];
        AnimalsToAnimalData.Remove(animal);
        animalCallback.alternativeCallback?.Invoke(animal);
    }
    public void ForceExit(GameObject animal)
    {
        ExitPattern(animal, false);
    }

    public struct AnimalCallbackData
    {
        public Animal animal;
        public StepCompletedCallBack callback;
        public StepCompletedCallBack alternativeCallback;
        public List<GameObject> collaboratingAnimals;
    }
}
