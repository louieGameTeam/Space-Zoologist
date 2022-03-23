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
    public bool HasNeedCache => GameManager.Instance.Needs.HasCache(Population);
    public NeedAvailability Availabilty => GameManager.Instance.Needs.Availability.GetAvailability(population);
    public NeedRating Rating => GameManager.Instance.Needs.Ratings.GetRating(population);
    public Population Population => population;
    public int GrowthCountdown { get; private set; } = 0;
    public int DecayCountdown { get; private set; } = 0;
    /// <summary>
    /// Rate of change for the population, 1f for doubling the population
    /// and -1f for removing the population
    /// </summary>
    public float ChangeRate
    {
        get
        {
            // If there are some predators then the decay rate depends on the number of predators
            if (Rating.PredatorCount > 0)
            {
                float ratio = -Rating.PredatorCount / (float)population.Count;
                return Mathf.Max(ratio, -1f);
            }
            // If there are no predators and all needs are met then compute the total change rate
            else if (Rating.FoodNeedIsMet && Rating.TerrainNeedIsMet && Rating.WaterNeedIsMet)
            {
                return (Rating.FoodRating + Rating.TerrainRating + Rating.WaterRating - 3) / 3f;
            }
            // If there are no predators but not all needs are met then
            // the population will decline
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
