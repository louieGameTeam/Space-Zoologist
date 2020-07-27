using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// TODO: have consumer take food one after another
// TODO: have a way to determine if a full reset of the accessible list

public class FoodSourceNeedSystem : NeedSystem
{
    //private List<FoodSource> foodSources = new List<FoodSource>();
    private readonly ReservePartitionManager rpm = null;

    // Holds which FoodSources each population has access to.
    //private Dictionary<Population, HashSet<FoodSource>> accessibleFoodSources = new Dictionary<Population, HashSet<FoodSource>>();
    // Holds which populations have access to each FoodSource, the opposite of accessibleFoodSources.
    //private Dictionary<FoodSource, HashSet<Population>> populationsWithAccess = new Dictionary<FoodSource, HashSet<Population>>();

    //private 

    // Food name to food calculators
    private Dictionary<string, FoodSourceCalculator> foodSourceCalculators = new Dictionary<string, FoodSourceCalculator>();

    public FoodSourceNeedSystem(ReservePartitionManager rpm, string needName = "FoodSource") : base(needName)
    {
        this.rpm = rpm;
    }

    public override bool CheckState()
    {
        bool needUpdate = false;

        foreach (FoodSourceCalculator foodSourceCalculator in this.foodSourceCalculators.Values)
        {
            // Check if consumer is dirty
            foreach (Population consumer in foodSourceCalculator.Consumers)
            {
                if (consumer.GetAccessibilityStatus())
                {
                    foodSourceCalculator.MarkDirty();
                    needUpdate = true;
                    break;
                }
            }

            // If consumer is already dirty check next food source calculator
            if (needUpdate)
            {
                break;
            }
            // Check if food source is dirty
            else
            {
                foreach (FoodSource foodSource in foodSourceCalculator.FoodSources)
                {
                    if (foodSource.GetAccessibilityStatus())
                    {
                        foodSourceCalculator.MarkDirty();
                        needUpdate = true;
                        break;
                    }
                }
            }
        }

        return needUpdate;
    }

    /// <summary>
    /// Updates how much all the registered populations take from all of the FoodSourceNeedSystem's food sources and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
        foreach (FoodSourceCalculator foodSourceCalculator in this.foodSourceCalculators.Values)
        {
            if (foodSourceCalculator.IsDirty)
            {
                var foodDistributionOutput = foodSourceCalculator.CalculateDistribution();

                // foodDistributionOutput returns null is not thing to be updated
                if (foodDistributionOutput != null)
                {
                    foreach (Population consumer in foodDistributionOutput.Keys)
                    {
                        consumer.UpdateNeed(foodSourceCalculator.FoodSourceName, foodDistributionOutput[consumer]);
                    }
                }
            }
        }

        this.isDirty = false;
    }

    public void AddFoodSource(FoodSource foodSource)
    {
        if (!this.foodSourceCalculators.ContainsKey(foodSource.Species.SpeciesName))
        {
            this.foodSourceCalculators.Add(foodSource.Species.SpeciesName, new FoodSourceCalculator(rpm, foodSource.Species.SpeciesName));
        }

        this.foodSourceCalculators[foodSource.Species.SpeciesName].AddFoodSource(foodSource);
    }

    public override void AddConsumer(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            // Check if the need is a 'FoodSource' type
            if (need.NeedType == "FoodSource")
            {
                // Create a food source calculator for this food source,
                // if not already exist
                if (!this.foodSourceCalculators.ContainsKey(need.NeedName))
                {
                    this.foodSourceCalculators.Add(need.NeedName, new FoodSourceCalculator(rpm, need.NeedName));
                }

                // Add consumer to food source calculator
                this.foodSourceCalculators[need.NeedName].AddConsumer((Population)life);
            }
        }
    }

    public override bool RemoveConsumer(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            // Check if the need is a 'FoodSource' type
            if (need.NeedType == "FoodSource")
            {
                Debug.Assert(this.foodSourceCalculators[need.NeedName].RemoverConsumer((Population)life), "Remove conumer failed!");
            }
        }

        return true;
    }
}