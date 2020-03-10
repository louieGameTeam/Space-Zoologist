using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * InitializeAnimalPopulations on Start
 * FoodDistribtuion called then UpdateAnimalPopulations called every time GameState changes in UpdateManager.
 */

// [CreateAssetMenuAttribute(fileName = "AnimalPopulationsManager", menuName = "AnimalPopulations/NewAnimalPopulationsManager")]
public class AnimalPopulationsManager : MonoBehaviour, INeedSystem
{
    [Header("Current Populations")]
    [Expandable] public List<Species> speciesList;
    private List<AnimalPopulation> AnimalPopulations = new List<AnimalPopulation>();

    public void Start()
    {
        EventManager.StartListening("Initialize", InitializeAnimalPopulations);
    }

    public void RegisterPopulation(AnimalPopulation population)
    {
        AnimalPopulations.Add(population);
    }

    public void UnregisterPopulation(AnimalPopulation population)
    {
        AnimalPopulations.Remove(population);
    }

    public void InitializeAnimalPopulations()
    {
        foreach (Species species in this.speciesList)
        {
            CreatePopulation(species, Vector2Int.zero);
        }
    }

    // Initialize GameObject with the specs from Species and place under PopulationsManager in Heirarchy
    public void CreatePopulation(Species species, Vector2Int location)
    {
        GameObject gameObject = Instantiate(new GameObject(), this.transform);
        gameObject.name = species._speciesType + "Population";
        gameObject.AddComponent<AnimalPopulation>().InitializeFromSpecies(species);
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

    public string NeedName()
    {
        return "AnimalPopulation";
    }
}
