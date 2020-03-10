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
            CreatePopulation(speciesA, Vector2Int.zero);
        }
        // EventManager.TriggerEvent("PopulationInitialized");
    }

    // Initialize GameObject with the specs from Species and place under PopulationsManager in Heirarchy
    public void CreatePopulation(Species species, Vector2Int location)
    {
        GameObject gameObject = Instantiate(new GameObject(), this.transform);
        gameObject.name = species._speciesType + "Population";
        gameObject.AddComponent<AnimalPopulation>().InitializeFromSpecies(species);
        AnimalPopulations.Add(gameObject);
        NeedSystemManager.RegisterPopulation(gameObject.GetComponent<AnimalPopulation>(), species._speciesType);
    }

    //private void RemoveAnimalPopulation(Species populationRemoved)
    //{
    //    foreach(GameObject animalPopulation in this.AnimalPopulations)
    //    {
    //        if (animalPopulation.GetComponent<AnimalPopulation>().populationType == populationRemoved._speciesType)
    //        {
    //            this.AnimalPopulations.Remove(animalPopulation);
    //        }
    //    }
    //}

    public List<GameObject> GetAnimalPopulationGameObjects()
    {
        return this.AnimalPopulations;
    }
}
