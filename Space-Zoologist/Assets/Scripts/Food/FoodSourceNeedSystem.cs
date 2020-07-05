using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO: have consumer take food one after another
// TODO: 

public class FoodSourceNeedSystem : NeedSystem
{
    private List<FoodSource> foodSources = new List<FoodSource>();
    private readonly ReservePartitionManager rpm = null;
    public FoodSourceNeedSystem(string needName, ReservePartitionManager rpm) : base(needName)
    {
        this.rpm = rpm;
    }

    /// <summary>
    /// Updates how much all the registered populations take from all of the FoodSourceNeedSystem's food sources and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
        if (foodSources.Count == 0) return;

        // TODO: this dictionaries could be part of rpm

        // Holds which FoodSources each population has access to.
        Dictionary<Population, HashSet<FoodSource>> accessibleFoodSources = new Dictionary<Population, HashSet<FoodSource>>();
        // Holds which populations have access to each FoodSource, the opposite of accessibleFoodSources.
        Dictionary<FoodSource, HashSet<Population>> populationsWithAccess = new Dictionary<FoodSource, HashSet<Population>>();
        // Holds how much food each FoodSource has left after Populations take from them.
        Dictionary<FoodSource, float> amountFoodRemaining = new Dictionary<FoodSource, float>();

        // Initialize accessibleFoodSources, populationsWithAccess, amd amountFoodRemaining.
        foreach (FoodSource foodSource in foodSources)
        {
            populationsWithAccess.Add(foodSource, new HashSet<Population>());
            amountFoodRemaining.Add(foodSource, foodSource.FoodOutput);
            foreach (Population population in lives)
            {
                if (!accessibleFoodSources.ContainsKey(population))
                {
                    accessibleFoodSources.Add(population, new HashSet<FoodSource>());
                }
                if (rpm.CanAccess(population, foodSource.Position))
                {
                    accessibleFoodSources[population].Add(foodSource);
                    populationsWithAccess[foodSource].Add(population);
                }
            }
        }

        // Holds the sum of the Dominance of all Populations that have access to the FoodSource for each FoodSource.
        Dictionary<FoodSource, float> totalLocalDominance = new Dictionary<FoodSource, float>();
        // Holds the remaining Dominance of each FoodSource after populations have already taken their share of food.
        Dictionary<FoodSource, float> localDominanceRemaining = new Dictionary<FoodSource, float>();

        // Initialize totalLocalDominance and localDominanceRemaining.
        foreach (FoodSource foodSource in foodSources)
        {
            float total = populationsWithAccess[foodSource].Sum(p => p.Dominance);
            totalLocalDominance.Add(foodSource, total);
            localDominanceRemaining.Add(foodSource, total);
        }

        // Holds the populations that will not have enough food to give them a good condition.
        HashSet<Population> populationsWithNotEnough = new HashSet<Population>();

        // Foreach population, if it is in good condition from the food available to it, then take its portion and update its need,
        // else, add it to the set of populations that will not have enough. The populations without enough will then split what is 
        // remaining based on the ratio of their dominance to the localRemainingDominance for each of their FoodSources.
        foreach (Population population in lives)
        {
            float availableFood = 0.0f;
            float amountRequiredPerIndividualForGoodCondition = population.Species.Needs[base.NeedName].GetThreshold(NeedCondition.Good, -1, false);
            float amountRequiredForGoodCondition = amountRequiredPerIndividualForGoodCondition * population.Count;
            foreach (FoodSource foodSource in accessibleFoodSources[population])
            {
                availableFood += foodSource.FoodOutput * (population.Dominance / totalLocalDominance[foodSource]);
            }

            // If the food available to the Population is more than enough, only take enough and update its need.
            if (availableFood >= amountRequiredForGoodCondition)
            {
                float foodToTakeRatio = amountRequiredForGoodCondition / availableFood;
                float totalFoodAcquired = 0.0f;
                foreach (FoodSource foodSource in accessibleFoodSources[population])
                {
                    float foodAcquired = foodToTakeRatio * foodSource.FoodOutput * (population.Dominance / totalLocalDominance[foodSource]);
                    amountFoodRemaining[foodSource] -= foodAcquired;
                    totalFoodAcquired += foodAcquired;
                    localDominanceRemaining[foodSource] -= population.Dominance;
                    //Debug.Log($"{population.Species.SpeciesName} population took {foodAcquired} food from foodsource at {foodSource.Position}");
                }
                float foodAcquiredPerIndividual = totalFoodAcquired / population.Count;
                population.UpdateNeed(NeedName, foodAcquiredPerIndividual);
            }
            // Otherwise, add to the set of populationsWithNotEnough to be processed later.
            else
            {
                populationsWithNotEnough.Add(population);
            }
        }

        // For those Populations who will not have enough to be in good condition, split what food remains from each FoodSource.
        foreach (Population population in populationsWithNotEnough)
        {
            float foodAcquired = 0.0f;
            foreach (FoodSource foodSource in accessibleFoodSources[population])
            {
                float dominanceRatio = population.Dominance / localDominanceRemaining[foodSource];
                foodAcquired += dominanceRatio * amountFoodRemaining[foodSource];
            }
            float amountAcquiredPerIndividual = foodAcquired / population.Count;
            population.UpdateNeed(this.NeedName, amountAcquiredPerIndividual);
        }

        // Done update not dirty any more
        isDirty = false;
    }

    public void AddFoodSource(FoodSource foodSource)
    {
        foodSources.Add(foodSource);
    }
}
