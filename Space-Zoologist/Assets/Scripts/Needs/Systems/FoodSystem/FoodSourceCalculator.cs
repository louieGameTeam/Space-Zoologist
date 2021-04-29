using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Calculates food distribution of a certain food type
/// </summary>
public class FoodSourceCalculator : NeedCalculator
{
    public string FoodSourceName => this.foodSourceName;
    public List<FoodSource> FoodSources => this.foodSources;
    public List<Population> Consumers => this.consumers;
    public bool IsDirty => this.isDirty;

    private string foodSourceName = default;
    private readonly ReservePartitionManager rpm = null;
    private List<FoodSource> foodSources = new List<FoodSource>();
    private List<Population> consumers = new List<Population>();
    private bool isDirty = default;

    // Holds which FoodSources each population has access to.
    private Dictionary<Population, HashSet<FoodSource>> accessibleFoodSources = new Dictionary<Population, HashSet<FoodSource>>();
    // Holds which populations have access to each FoodSource, the opposite of accessibleFoodSources.
    private Dictionary<FoodSource, HashSet<Population>> populationsWithAccess = new Dictionary<FoodSource, HashSet<Population>>();

    Dictionary<Population, float> distributedFood = new Dictionary<Population, float>();

    public FoodSourceCalculator(ReservePartitionManager rpm, string foodSourceName)
    {
        this.foodSourceName = foodSourceName;
        this.rpm = rpm;
    }

    public void MarkDirty()
    {
        this.isDirty = true;
    }


    public void AddConsumer(Population consumer)
    {
        if (this.consumers.Contains(consumer))
        {
            return;
        }

        this.consumers.Add(consumer);

        //Population population = (Population)life;
        accessibleFoodSources.Add(consumer, new HashSet<FoodSource>());
        foreach (FoodSource foodSource in foodSources)
        {
            if (rpm.CanAccess(consumer, foodSource.Position))
            {
                accessibleFoodSources[consumer].Add(foodSource);
                populationsWithAccess[foodSource].Add(consumer);
            }
        }

        this.isDirty = true;
    }

    public void AddSource(Life source)
    {
        FoodSource foodSource = (FoodSource)source;

        this.foodSources.Add(foodSource);

        populationsWithAccess.Add(foodSource, new HashSet<Population>());
        foreach (Population population in Consumers)
        {
            if (rpm.CanAccess(population, foodSource.Position))
            {
                accessibleFoodSources[population].Add(foodSource);
                populationsWithAccess[foodSource].Add(population);
            }
        }

        this.isDirty = true;
    }

    public bool RemoveConsumer(Population consumer)
    {
        this.isDirty = true;
 
        this.consumers.Remove(consumer);
        this.accessibleFoodSources.Remove(consumer);
        // Assert would not be included in depolyment built
        // Debug.Assert(this.consumers.Remove(consumer), "Consumer removal failure");
        // Debug.Assert(this.accessibleFoodSources.Remove(consumer), "Removal of consumer in AccessibleFoodSources failed!");

        return true;
    }

    public bool RemoveSource(Life source)
    {
        FoodSource foodSource = (FoodSource)source;

        this.isDirty = true;

        Debug.Assert(!this.foodSources.Remove(foodSource), "FoodSource removal failure");
        Debug.Assert(!this.populationsWithAccess.Remove(foodSource), "Removal of foodsource in populationsWithAccess failed!");

        return true;
    }

    public Dictionary<Population, float> CalculateDistribution()
    {
        if (foodSources.Count == 0 || consumers.Count == 0)
        {
            Debug.Log("Food output not calculated");
            this.isDirty = false;
            return null;
        }

        // When accessbility of population changes a full reset should be triggered
        foreach (Population population in consumers)
        {
            if (population.HasAccessibilityChanged)
            {
                // Debug.Log($"{population} triggered a accessible list reset");

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
        foreach (Population population in consumers)
        {
            float availableFood = 0.0f;
            float amountRequiredPerIndividualForGoodCondition = population.Needs[this.foodSourceName].GetThreshold(NeedCondition.Good, -1, false);
       
            float amountRequiredForGoodCondition = amountRequiredPerIndividualForGoodCondition * population.Count;
            foreach (FoodSource foodSource in accessibleFoodSources[population])
            {
                Debug.Log(population.gameObject.name + " dominance: " + population.Dominance + ", total local dominance: " + totalLocalDominance[foodSource] + " for " + foodSource.gameObject.name);
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

                // Save the distributed value
                if(!this.distributedFood.ContainsKey(population))
                {
                    this.distributedFood.Add(population, 0f);
                }
                this.distributedFood[population] = foodAcquiredPerIndividual;
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

            // Save the distributed value
            if (!this.distributedFood.ContainsKey(population))
            {
                this.distributedFood.Add(population, 0f);
            }
            this.distributedFood[population] = amountAcquiredPerIndividual;
        }

        // Done update not dirty any more
        isDirty = false;

        return this.distributedFood;
    }
}
