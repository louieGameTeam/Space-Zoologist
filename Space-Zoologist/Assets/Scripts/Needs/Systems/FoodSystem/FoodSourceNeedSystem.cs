using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles neeed value updates of all the `FoodSource` type need
/// </summary>
public class FoodSourceNeedSystem : NeedSystem
{

    public static readonly Dictionary<ItemID, float> foodDominanceRatios = new Dictionary<ItemID, float>() 
    {
        {ItemRegistry.FindWithName("Cow"), 0.3f}, 
        {ItemRegistry.FindWithName("Anteater"), 0.25f}, 
        {ItemRegistry.FindWithName("Goat"), 0.20f}, 
        {ItemRegistry.FindWithName("Slug"), 0.15f}, 
        {ItemRegistry.FindWithName("Spider"), 0.10f}
    };

    // Food name to food calculators
    private Dictionary<ItemID, FoodSourceCalculator> foodSourceCalculators = new Dictionary<ItemID, FoodSourceCalculator>();

    public FoodSourceNeedSystem(NeedType needType = NeedType.FoodSource) : base(needType)
    {
    }

    public override bool CheckState()
    {
        bool needUpdate = false;
        foreach (FoodSourceCalculator foodSourceCalculator in this.foodSourceCalculators.Values)
        {
            // Check if consumer is dirty
            foreach (Population consumer in foodSourceCalculator.Consumers)
            {
                if (consumer.GetAccessibilityStatus() || consumer.Count != consumer.PrePopulationCount)
                {
                    foodSourceCalculator.MarkDirty();
                    needUpdate = true;
                    break;
                }
            }
            // If consumer is already dirty check next food source calculator, otherwise check the terrain
            if (needUpdate)
            {
                continue;
            }
            else
            {
                needUpdate = this.CheckFoodSourcesTerrain(foodSourceCalculator);
            }
        }
        return needUpdate;
    }

    // foodSource.GetAccessibilityStatus() is Expensive!
    private bool CheckFoodSourcesTerrain(FoodSourceCalculator foodSourceCalculator)
    {
        foreach (FoodSource foodSource in foodSourceCalculator.FoodSources)
        {
            if (foodSource.GetAccessibilityStatus())
            {
                foodSourceCalculator.MarkDirty();
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
        // 1. Reset all calculators remaining food
        foreach (FoodSourceCalculator foodSourceCalculator in this.foodSourceCalculators.Values)
        {
            foodSourceCalculator.ResetCalculator();
        }

        // Create a new list that is sorted by the food dominance of the populations
        // This way, dominant species get the food first
        List<Population> populations = new List<Population>(GameManager.Instance.m_reservePartitionManager.Populations);
        populations.Sort(new DominanceComparer());

        // 2. Iterate through populations based on most dominant
        foreach (Population population in populations)
        {
            float preferredAmount = 0;
            float compatibleAmount = 0;

            // 3. Iterate through needs starting with preferred (inefficient, could be refactored to first calculate list of ordered needs)
            for (int j = 0; j <= 1; j++)
            {
                foreach (KeyValuePair<string, Need> need in population.Needs)
                {
                    // Try to find an item id with the given need name
                    ItemID needID = ItemRegistry.FindWithName(need.Key);

                    // 4. Calculate preferred and available food, skipping if need already met
                    if (!need.Value.NeedType.Equals(NeedType.FoodSource) || !foodSourceCalculators.ContainsKey(needID))
                    {
                        continue;
                    }

                    if (j == 0 && need.Value.IsPreferred)
                    {
                        preferredAmount += foodSourceCalculators[needID].CalculateDistribution(population);
                        continue;
                    }

                    if (j == 1 && !need.Value.IsPreferred)
                    {
                        compatibleAmount += foodSourceCalculators[needID].CalculateDistribution(population);
                    }
                }
            }
            population.UpdateFoodNeed(preferredAmount, compatibleAmount);
        }
    }

    public void AddFoodSource(FoodSource foodSource)
    {
        if (!this.foodSourceCalculators.ContainsKey(foodSource.Species.ID))
        {
            this.foodSourceCalculators.Add(foodSource.Species.ID, new FoodSourceCalculator(foodSource.Species.ID));
        }

        this.foodSourceCalculators[foodSource.Species.ID].AddSource(foodSource);

        this.isDirty = true;
    }

    public void RemoveFoodSource(FoodSource foodSource) {
        if (this.foodSourceCalculators.ContainsKey(foodSource.Species.ID))
        {
            this.foodSourceCalculators[foodSource.Species.ID].RemoveSource(foodSource);

            this.isDirty = true;
        }
    }

    public override void AddConsumer(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            // Check if the need is a 'FoodSource' type
            if (need.NeedType == NeedType.FoodSource)
            {
                // Try to find an item id with the given name
                ItemID needID = ItemRegistry.FindWithName(need.NeedName);

                // Create a food source calculator for this food source,
                // if not already exist
                if (!this.foodSourceCalculators.ContainsKey(needID))
                {
                    this.foodSourceCalculators.Add(needID, new FoodSourceCalculator(need.NeedName));
                }

                // Add consumer to food source calculator
                this.foodSourceCalculators[needID].AddConsumer((Population)life);
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
                ItemID needID = ItemRegistry.FindWithName(need.NeedName);
                Debug.Assert(this.foodSourceCalculators[needID].RemoveConsumer((Population)life), "Remove conumer failed!");
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

    private class DominanceComparer : IComparer<Population>
    {
        public int Compare(Population a, Population b)
        {
            return (int)(100*(b.FoodDominance - a.FoodDominance));
        }
    }
}