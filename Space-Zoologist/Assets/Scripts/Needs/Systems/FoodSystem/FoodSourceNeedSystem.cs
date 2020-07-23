using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO: have consumer take food one after another
// TODO: have a way to determine if a full reset of the accessible list

public class FoodSourceNeedSystem : NeedSystem
{
    private List<FoodSource> foodSources = new List<FoodSource>();
    private readonly ReservePartitionManager rpm = null;

    // Holds which FoodSources each population has access to.
    private Dictionary<Population, HashSet<FoodSource>> accessibleFoodSources = new Dictionary<Population, HashSet<FoodSource>>();
    // Holds which populations have access to each FoodSource, the opposite of accessibleFoodSources.
    private Dictionary<FoodSource, HashSet<Population>> populationsWithAccess = new Dictionary<FoodSource, HashSet<Population>>();

    private Dictionary<FoodSource, int[]> ConsumedFoodSourceAcceiableTerrain = new Dictionary<FoodSource, int[]>();

    public FoodSourceNeedSystem(ReservePartitionManager rpm, string needName) : base(needName)
    {
        this.rpm = rpm;
    }

    public override bool CheckState()
    {
        if (base.CheckState())
        {
            return true;
        }

        foreach(FoodSource foodSource in this.foodSources)
        {
            if (foodSource.GetAccessibilityStatus())
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates how much all the registered populations take from all of the FoodSourceNeedSystem's food sources and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
        if (foodSources.Count == 0 || Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        // When accessbility of population changes a full reset should be triggered
        foreach (Population population in Consumers)
        {
            if (rpm.PopulationAccessbilityStatus[population])
            {
                //Debug.Log($"{population} triggered a accessible list reset");

                foreach (FoodSource foodSource in foodSources)
                {
                    if (rpm.CanAccess(population, foodSource.Position))
                    {
                        accessibleFoodSources[population].Add(foodSource);
                        populationsWithAccess[foodSource].Add(population);
                    }
                }
                //rpm.PopulationAccessbilityStatus[population] = false;
            }
        }

        // TODO: this dictionaries could be part of rpm

        // Holds how much food each FoodSource has left after Populations take from them.
        Dictionary<FoodSource, float> amountFoodRemaining = new Dictionary<FoodSource, float>();

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

            // Reset amountFoodRemaining to initial food output 
            amountFoodRemaining.Add(foodSource, foodSource.FoodOutput);
        }

        // Holds the populations that will not have enough food to give them a good condition.
        HashSet<Population> populationsWithNotEnough = new HashSet<Population>();

        // Foreach population, if it is in good condition from the food available to it, then take its portion and update its need,
        // else, add it to the set of populations that will not have enough. The populations without enough will then split what is 
        // remaining based on the ratio of their dominance to the localRemainingDominance for each of their FoodSources.
        foreach (Population population in Consumers)
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

        populationsWithAccess.Add(foodSource, new HashSet<Population>());
        foreach (Population population in Consumers)
        {
            if (rpm.CanAccess(population, foodSource.Position))
            {
                accessibleFoodSources[population].Add(foodSource);
                populationsWithAccess[foodSource].Add(population);
            }
        }

        // Add current accessible terrain info
        if (!this.ConsumedFoodSourceAcceiableTerrain.ContainsKey(foodSource))
        {
            this.ConsumedFoodSourceAcceiableTerrain.Add(foodSource, TileSystem.ins.CountOfTilesInRange(Vector3Int.FloorToInt(foodSource.GetPosition()), foodSource.Species.RootRadius));
        }

        this.isDirty = true;
    }

    public override void AddConsumer(Life life)
    {
        base.AddConsumer(life);

        Population population = (Population)life;
        accessibleFoodSources.Add(population, new HashSet<FoodSource>());
        foreach (FoodSource foodSource in foodSources)
        {
            if (rpm.CanAccess(population, foodSource.Position))
            {
                accessibleFoodSources[population].Add(foodSource);
                populationsWithAccess[foodSource].Add(population);
            }
        }
    }
}