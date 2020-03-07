using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * InitializeAnimalPopulations on Start
 * FoodDistribtuion called then UpdateAnimalPopulations called every time GameState changes in UpdateManager.
 */

// [CreateAssetMenuAttribute(fileName = "AnimalPopulationsManager", menuName = "AnimalPopulations/NewAnimalPopulationsManager")]
public class AnimalPopulationsManager : MonoBehaviour
{
    [Header("Current Populations")]
    [Expandable] public List<Species> speciesList;
    private List<GameObject> AnimalPopulations = new List<GameObject>();

    public void Start()
    {
        EventManager.StartListening("Initialize", InitializeAnimalPopulations);
    }

    public void TriggerInitialize()
    {
        EventManager.TriggerEvent("Initialize");
    }

    public void InitializeAnimalPopulations()
    {
        foreach (Species speciesA in this.speciesList)
        {
            AddAnimalPopulationGameObject(speciesA);
        }
        EventManager.TriggerEvent("PopulationInitialized");
    }

    /*
     * 1. Pull data from somewhere to see what was added/removed
     * - Maybe player object has info or maybe we have an object that holds populations which are ready to be introduced/removed
     * - Maybe evolution creates a new Population??? Predation removes a population?
     * 2. Send out the message that the Population List<GameObjects> has been updated so they can get the new list
     */
    public void AnimalPopulationNeedsToBeUpdated()
    {
        // if added
        // AddAnimalPopulationGameObject(PopulationAdded);
        // if removed
        // RemoveAnimalPopulationGameObject(PopulationRemoved);
        EventManager.TriggerEvent("UpdatedPopulations");
    }

    // Initialize GameObject with the specs from Species and place under PopulationsManager in Heirarchy
    private void AddAnimalPopulationGameObject(Species populationAdded)
    {
        GameObject newAnimalPopulationGameObject = new GameObject(populationAdded._speciesType + "Population");
        newAnimalPopulationGameObject.AddComponent<AnimalPopulation>();
        AnimalPopulation newAnimalPopulation = newAnimalPopulationGameObject.GetComponent<AnimalPopulation>();
        newAnimalPopulation.InitializeAnimalPopulation(populationAdded.GetPopGrowth(),
            populationAdded.GetNeeds(), populationAdded.GetPopulationSize(), populationAdded._speciesType);
        newAnimalPopulationGameObject.transform.parent = this.transform;
        this.AnimalPopulations.Add(newAnimalPopulationGameObject);
    }

    private void RemoveAnimalPopulation(Species populationRemoved)
    {
        foreach(GameObject animalPopulation in this.AnimalPopulations)
        {
            if (animalPopulation.GetComponent<AnimalPopulation>().populationType == populationRemoved._speciesType)
            {
                this.AnimalPopulations.Remove(animalPopulation);
            }
        }
    }

    public List<GameObject> GetAnimalPopulationGameObjects()
    {
        return this.AnimalPopulations;
    }
}
