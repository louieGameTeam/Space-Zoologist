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

    public FoodSourceNeedSystem(ReservePartitionManager rpm, string needName) : base(needName)
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

                foreach (Population consumer in foodDistributionOutput.Keys)
                {
                    consumer.UpdateNeed(foodSourceCalculator.FoodSourceName, foodDistributionOutput[consumer]);
                }
            }
        }
    }

    public void AddFoodSource(FoodSource foodSource)
    {
        this.foodSourceCalculators[foodSource.Species.SpeciesName].AddFoodSource(foodSource);
    }

    public override void AddConsumer(Life life)
    {
        // TODO: Add consumer to each food source Ns based on its need string name
        //foreach ()
    }
}