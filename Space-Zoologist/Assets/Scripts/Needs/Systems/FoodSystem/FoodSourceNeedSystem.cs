using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Handles neeed value updates of all the `FoodSource` type need
/// </summary>
public class FoodSourceNeedSystem : NeedSystem
{

    public static readonly Dictionary<SpeciesType, float> foodDominanceRatios = new Dictionary<SpeciesType, float>() 
    {
        {SpeciesType.Cow, 0.3f}, 
        {SpeciesType.Anteater, 0.25f}, 
        {SpeciesType.Goat, 0.20f}, 
        {SpeciesType.Slug, 0.15f}, 
        {SpeciesType.Spider, 0.10f}
    };

    // Food name to food calculators
    private Dictionary<string, FoodSourceCalculator> foodSourceCalculators = new Dictionary<string, FoodSourceCalculator>();

    public FoodSourceNeedSystem(NeedType needType = NeedType.FoodSource) : base(needType) {}

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

            // 3. Get food needs seperated by preference
            List<KeyValuePair<string, Need>> preferredNeeds = new List<KeyValuePair<string, Need>>();
            List<KeyValuePair<string, Need>> compatibleNeeds = new List<KeyValuePair<string, Need>>();

            foreach (KeyValuePair<string, Need> need in population.Needs.ToList ()) {
                if (!need.Value.NeedType.Equals (NeedType.FoodSource) || !foodSourceCalculators.ContainsKey (need.Key)) {
                    continue;
                }

                if (need.Value.IsPreferred) 
                {
                    preferredNeeds.Add(need);
                } else 
                {
                    compatibleNeeds.Add(need);
                }
            }

            // 4. Iterate through food sources and calculate distribution, starting with preferred needs
            foreach (KeyValuePair<string, Need> need in preferredNeeds) 
            {
                preferredAmount += foodSourceCalculators [need.Key].CalculateDistribution (population);
            }

            foreach (KeyValuePair<string, Need> need in compatibleNeeds) 
            {
                compatibleAmount += foodSourceCalculators [need.Key].CalculateDistribution (population);
            }

            population.UpdateFoodNeed(preferredAmount, compatibleAmount);
        }
    }

    public void AddFoodSource(FoodSource foodSource)
    {
        if (!this.foodSourceCalculators.ContainsKey(foodSource.Species.SpeciesName))
        {
            this.foodSourceCalculators.Add(foodSource.Species.SpeciesName, new FoodSourceCalculator(foodSource.Species.SpeciesName));
        }

        this.foodSourceCalculators[foodSource.Species.SpeciesName].AddSource(foodSource);

        this.isDirty = true;
    }

    public void RemoveFoodSource(FoodSource foodSource) {
        if (this.foodSourceCalculators.ContainsKey(foodSource.Species.SpeciesName))
        {
            this.foodSourceCalculators[foodSource.Species.SpeciesName].RemoveSource(foodSource);

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
                // Create a food source calculator for this food source,
                // if not already exist
                if (!this.foodSourceCalculators.ContainsKey(need.NeedName))
                {
                    this.foodSourceCalculators.Add(need.NeedName, new FoodSourceCalculator(need.NeedName));
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

    private class DominanceComparer : IComparer<Population>
    {
        public int Compare(Population a, Population b)
        {
            return (int)(100*(b.FoodDominance - a.FoodDominance));
        }
    }
}