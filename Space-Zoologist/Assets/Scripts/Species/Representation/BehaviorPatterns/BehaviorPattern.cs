using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPattern : MonoBehaviour
{
    public Dictionary<GameObject, AnimalData> AnimalsToAnimalData = new Dictionary<GameObject, AnimalData>(); // The dictionary that holds all animal gameObjects to their data. If you want custom data to be stored, add another dictionary like this one
    protected GridSystem GridSystem = default;
    private List<GameObject> compeletedAnimals = new List<GameObject>(); //Lists to remove animals from updating before updating to avoid modifying while iterating
    private List<GameObject> forceRemoveAnimals = new List<GameObject>();
    /// <summary>
    /// Assign necessary data to this script
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="callBack"></param>
    /// <param name="collaboratingAnimals"></param>
    public void InitializePattern(GameObject gameObject, StepCompletedCallBack callBack, List<GameObject> collaboratingAnimals = null)
    {
        AnimalData animalData = new AnimalData();
        animalData.animal = gameObject.GetComponent<Animal>();
        animalData.callback = callBack;
        animalData.collaboratingAnimals = collaboratingAnimals;
        // Debug.Log(gameObject.name + " is trying to be initial");
        AnimalsToAnimalData.Add(gameObject, animalData);
        EnterPattern(gameObject);
    }

    public void SetupDependencies(GridSystem gridSystem)
    {
        this.GridSystem = gridSystem;
    }
    /// <summary>
    /// Excutes once after initialization, override if you have additional initializations
    /// </summary>
    /// <param name="gameObject"></param>
    protected virtual void EnterPattern(GameObject gameObject)
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
                compeletedAnimals.Add(animal);
            }
        }
        foreach (GameObject animal in compeletedAnimals)
        {
            ExitPattern(animal);
        }
        compeletedAnimals.Clear();
    }
    /// <summary>
    /// Excuted every update, applies update and returns true if the pattern is completed
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="animalData"></param>
    /// <returns>Return True if animal has completed the pattern</returns>
    protected virtual bool IsPatternFinishedAfterUpdate(GameObject animal, AnimalData animalData)
    {
        return true;
    }

    /// <summary>
    /// Actions taken upon compeletion of pattern. All following are generally necessary when exit. It is recommended to call base when overriden.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="isCallingCallback">Set to false when force exiting without completion, leave as default</param>
    public virtual void ExitPattern(GameObject gameObject)
    {
        gameObject.GetComponent<AnimalBehaviorManager>().activeBehaviorPatterns.Remove(this);
        AnimalsToAnimalData[gameObject].callback?.Invoke(gameObject, AnimalsToAnimalData[gameObject].collaboratingAnimals);
        AnimalsToAnimalData.Remove(gameObject);
    }
    public void QueueForForceExit(GameObject gameObject, bool isDriven = false)
    {
        forceRemoveAnimals.Add(gameObject);
        if (isDriven)
        {
            foreach (GameObject collab in AnimalsToAnimalData[gameObject].collaboratingAnimals)
            {
                collab.GetComponent<AnimalBehaviorManager>().ForceExit(true);
            }
        }

    }
    /// <summary>
    /// Called when behavior is overriden by other behaviors
    /// </summary>
    /// <param name="gameObject"></param>
    protected virtual void ForceExit(GameObject gameObject)
    {
        gameObject.GetComponent<AnimalBehaviorManager>().activeBehaviorPatterns.Remove(this);
        // Debug.Log(gameObject.name + " has been forced exited");
        AnimalsToAnimalData.Remove(gameObject);
    }
    public struct AnimalData
    {
        public Animal animal;
        public StepCompletedCallBack callback;
        public List<GameObject> collaboratingAnimals;
    }
}
