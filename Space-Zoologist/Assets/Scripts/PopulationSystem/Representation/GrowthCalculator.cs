using System;
using System.Collections.Generic;
using UnityEngine;

public enum GrowthStatus { growing, stagnant, decaying }

/// <summary>
/// Determines when a population is ready to grow/decay
/// and by how much
/// </summary>
public class GrowthCalculator
{
    #region Public Properties
    public NeedAvailability Availabilty => GameManager.Instance.Needs.Availability.GetAvailability(population);
    public NeedRating Rating => GameManager.Instance.Needs.Ratings.GetRating(population);
    /// <summary>
    /// Rate of change for the population, 1f for doubling the population
    /// and -1f for removing the population
    /// </summary>
    public float ChangeRate
    {
        get
        {
            if (Rating.FoodNeedIsMet && Rating.TerrainNeedIsMet && Rating.WaterNeedIsMet)
            {
                return (Rating.FoodRating + Rating.TerrainRating + Rating.WaterRating - 3) / 3f;
            }
            else
            {
                float rate = 0f;

                // Decrease the rate by every unmet need
                if (!Rating.FoodNeedIsMet) rate += Rating.FoodRating - 1;
                if (!Rating.TerrainNeedIsMet) rate += Rating.TerrainRating - 1;
                if (!Rating.WaterNeedIsMet) rate += Rating.WaterRating - 1;

                return rate / 3f;
            }
        }
    }
    public GrowthStatus GrowthStatus
    {
        get
        {
            if (ChangeRate > 0.001f) return GrowthStatus.growing;
            else if (ChangeRate < -0.001f) return GrowthStatus.decaying;
            else return GrowthStatus.stagnant;
        }
    }
    public Population Population => population;
    public int GrowthCountdown { get; private set; } = 0;
    public int DecayCountdown { get; private set; } = 0;
    #endregion

    #region Private Fields
    private Population population = default;
    #endregion

    #region Constructors
    public GrowthCalculator(Population population)
    {
        this.population = population;
        GrowthCountdown = population.Species.GrowthRate;
        DecayCountdown = population.Species.DecayRate;
    }
    #endregion

    #region Public Methods
    // Keep this so we know how the predator-prey used to be computed
    //public void CalculateGrowth()
    //{
    //    float predatorvalue = calculatepredatorpreyneed();
    //    if (predatorvalue > 0f)
    //    {
    //        this.growthstatus = growthstatus.declining;
    //        this.decaycountdown = 1;
    //        this.populationincreaserate = -1 * predatorvalue;
    //        return;
    //    }
    //}

    // Used to compute the predator prey need,
    // this should be moved to the NeedRatingFactory
    //public float calculatePredatorPreyNeed()
    //{
    //    float predatorValue = 0;
    //    foreach (KeyValuePair<ItemID, Need> need in population.Needs)
    //    {
    //        if (need.Value.NeedType.Equals(NeedType.Prey))
    //        {
    //            predatorValue += need.Value.NeedValue;
    //        }
    //    }
    //    return predatorValue;
    //}

    /// <summary>
    /// Compute the new population size when the population grows/shrinks
    /// </summary>
    /// <returns></returns>
    public int CalculateNextPopulationSize()
    {
        float delta = population.Count * ChangeRate;

        // If population is growing then move up to next int
        if (GrowthStatus == GrowthStatus.growing) delta = Mathf.Ceil(delta);
        // If population is declining then move down to lower int
        else delta = Mathf.Floor(delta);

        // Return count plus the change
        return population.Count + (int)delta;
    }

    public bool ReadyForDecay()
    {
        this.DecayCountdown--;
        if (this.DecayCountdown == 0)
        {
            this.DecayCountdown = population.Species.DecayRate;
            return true;
        }
        return false;
    }

    public bool ReadyForGrowth()
    {
        this.GrowthCountdown--;
        if (this.GrowthCountdown == 0)
        {
            this.GrowthCountdown = population.Species.GrowthRate;
            return true;
        }
        return false;
    }
    #endregion
}
