using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles neeed value updates of all the `FoodSource` type need
/// </summary>
public class FoodSourceNeedSystem : NeedSystem
{
    //private List<FoodSource> foodSources = new List<FoodSource>();
    private readonly ReservePartitionManager rpm = null;

    // Food name to food calculators
    private Dictionary<string, FoodSourceCalculator> foodSourceCalculators = new Dictionary<string, FoodSourceCalculator>();

    public FoodSourceNeedSystem(ReservePartitionManager rpm, NeedType needType = NeedType.FoodSource) : base(needType)
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
                if (rpm.PopulationAccessbilityStatus[consumer])
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

                Debug.Log($"{foodSourceCalculator.FoodSourceName} calculator updated");
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

        this.foodSourceCalculators[foodSource.Species.SpeciesName].AddSource(foodSource);

        this.isDirty = true;
    }

    public override void AddConsumer(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            // Check if the need is a 'FoodSource' type
            if (need.NeedType == NeedType.FoodSource)
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

        this.isDirty = true;
    }

    public override bool RemoveConsumer(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            // Check if the need is a 'FoodSource' type
            if (need.NeedType == NeedType.FoodSource)
            {
                Debug.Assert(this.foodSourceCalculators[need.NeedName].RemoveConsumer((Population)life), "Remove conumer failed!");
            }
        }

        this.isDirty = true;

        return true;
    }

    public override void MarkAsDirty()
    {
        base.MarkAsDirty();

        foreach (FoodSourceCalculator foodSourceCalculator in this.foodSourceCalculators.Values)
        {
            foodSourceCalculator.MarkDirty();
        }
    }
}