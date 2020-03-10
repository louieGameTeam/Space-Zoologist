using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;


/*
 * This distribution system (Realistic) is assuming that populations will stop consuming food after its
 * need is in the good rating. And that populations can have access to foodsource in different regions.
 */

/*
 * Note:
 *  - the food source type is an enum (int).
 *  - need to add error handling.
 *  - TPS test with all access.
 */

public class FoodDistributionSystem : MonoBehaviour
{

    // private float getFoodSourceOutput(FoodSource foodSource) { return foodSource.getOutput(); }
    private float getPopulationDominace(AnimalPopulation population) { return population.Dominace; }
    private int getPopulationSize(AnimalPopulation population) { return population.PopSize; }

    // Get a list of animal population that has access to given food source
    private List<AnimalPopulation> getPopulationsCanAccess(FoodSource foodSource)
    {
        List<AnimalPopulation> populationsCanAccess = new List<AnimalPopulation>();

        // TODO: get list of animal population that has access to foodSource. (RPS)

        return populationsCanAccess;
    }

    private float getTotoalDominaceOfPopulations(List<AnimalPopulation> populations)
    {
        float totalDominace = 0f;

        foreach (AnimalPopulation population in populations)
        {
            totalDominace += getPopulationDominace(population);
        }

        return totalDominace;
    }


    // Get list of populations that can consume given food source in given list of population
    private List<AnimalPopulation> getPopulationsThatConsumeFoodSource(List<AnimalPopulation> allPopulations, FoodSource foodSource)
    {
        List<AnimalPopulation> canConsumePopulations = new List<AnimalPopulation>();

        foreach (AnimalPopulation population in allPopulations)
        {
            // TODO: check in population can consume foodSourceType. (AnimalPopulation)
            //if (population.IsEdible(foodSource))
            //{
            //    canConsumePopulations.Add(population);
            //}
        }

        return canConsumePopulations;
    }

    private float getCompetionRating(List<AnimalPopulation> populations)
    {
        float competitionRating = 0;
        List<float> populationDominaces = new List<float>();


        // Get all dominaces in a list
        foreach (AnimalPopulation population in populations)
        {
            populationDominaces.Add(getPopulationDominace(population));
        }

        // Compute competition rating(standard deviviation)
        float avg = populationDominaces.Average();
        competitionRating = (float)Math.Sqrt(populationDominaces.Average(v => Math.Pow(v - avg, 2)));

        return competitionRating;
    }



    private void distributeFoodSource(FoodSource foodSource, List<AnimalPopulation> populations)
    {
        float totalDominace = getTotoalDominaceOfPopulations(populations);


        foreach (AnimalPopulation population in populations)
        {
            // float populationFood = getPopulationDominace(population) / totalDominace * getFoodSourceOutput(foodSource);
            // float foodPerIndividual = populationFood / getPopulationSize(population);

            // TODO: update food source need with foodPerIndividual
            //var ListOfNeeds = population.GetNeeds();

            //foreach(Need need in ListOfNeeds)
            //{
            //    if()
            //}
        }
    }

    // This function will be envoked when a type of food source is marked "dirty"
    public void update(List<FoodSource> foodSources)
    {
        var ratingAndPopulationPair = new SortedList();
        var foodSourceAndCanCosumePopulation = new Dictionary<FoodSource, List<AnimalPopulation>>();

        foreach (FoodSource foodSource in foodSources)
        {
            List<AnimalPopulation> animalPopulations = getPopulationsCanAccess(foodSource);
            List<AnimalPopulation> populationsThatCanConsumeFoodSource = getPopulationsThatConsumeFoodSource(animalPopulations, foodSource);

            foodSourceAndCanCosumePopulation.Add(foodSource, populationsThatCanConsumeFoodSource);

            float competionRating = getCompetionRating(populationsThatCanConsumeFoodSource);
            ratingAndPopulationPair.Add(competionRating, foodSource);
        }

        foreach (DictionaryEntry pair in ratingAndPopulationPair)
        {
            distributeFoodSource((FoodSource)pair.Value, foodSourceAndCanCosumePopulation[(FoodSource)pair.Value]);
        }
    }
}