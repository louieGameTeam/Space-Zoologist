using System;
using System.Collections.Generic;
using UnityEngine;

public enum GrowthStatus {declining, stagnate, growing}

/// <summary>
/// Determines the rate and status of growth for each population based off of their most severe need that isn't being met
/// </summary>
public class GrowthCalculator
{
    // Huh? Are these values outdated?
    // Better check the design documents to see how it's supposed to go
    public const float maxFreshWaterTilePercent = 0.98f; //Someday this will need to be changed to get the value from the scriptableObject
    public const float maxSaltTilePercent = 0.04f; //Someday this will need to be changed to get the value from the scriptableObject
    public const float maxBacteriaTilePercent = 0.09f; //Someday this will need to be changed to get the value from the scriptableObject

    public GrowthStatus GrowthStatus { get; private set; }
    public Dictionary<NeedType, bool> IsNeedMet = new Dictionary<NeedType, bool>();
    public int GrowthCountdown = 0;
    public int DecayCountdown = 0;
    public float FoodRating => foodRating;
    public float WaterRating => waterRating;
    public float TerrainRating => terrainRating;
    Population population = default;
    private float foodRating = 0f;
    private float waterRating = 0f;
    private float terrainRating = 0f;
    public float populationIncreaseRate = 0f;

    public GrowthCalculator(Population population)
    {
        this.population = population;
        this.GrowthCountdown = population.Species.GrowthRate;
        this.DecayCountdown = population.Species.DecayRate;
        this.GrowthStatus = GrowthStatus.stagnate;
    }

    public void setupNeedTracker(NeedType need)
    {
        if (IsNeedMet.ContainsKey(need))
        {
            return;
        }
        IsNeedMet.Add(need, false);
    }

    /*
        0. if any predators nearby, population set to decrease next day
        1. if any need not met, handle growth logic and set growth status
        2. if all needs met, handle growth logic
    */
    public void CalculateGrowth()
    {
        float predatorValue = calculatePredatorPreyNeed();
        if (predatorValue > 0f)
        {
            this.GrowthStatus = GrowthStatus.declining;
            this.DecayCountdown = 1;
            this.populationIncreaseRate = -1 * predatorValue;
            return;
        }
        this.CalculateTerrainNeed();
        this.CalculateWaterNeed();
        this.GrowthStatus = GrowthStatus.growing;
        
        foreach (KeyValuePair<NeedType, bool> need in new Dictionary<NeedType, bool>(IsNeedMet))
        {
            if (!need.Key.Equals(NeedType.Prey) && !IsNeedMet[need.Key])
            {
                //If any need is not being met, set the growth status to declining
                GrowthStatus = GrowthStatus.declining;
            }
            IsNeedMet[need.Key] = false;
        }

        populationIncreaseRate = 0f;

        if(this.GrowthStatus == GrowthStatus.growing)
        {
            //Scales the increase rate between 100% and 200% population increase
            populationIncreaseRate = (waterRating + foodRating + terrainRating)/3f;
        }
        else
        {
            //Scales the increase rate between 0% and 100% population decrease (could potentially wipe out a population if the player is failing as much as possible)
            populationIncreaseRate = (Mathf.Min(waterRating, 0) + Mathf.Min(foodRating, 0) + Mathf.Min(terrainRating, 0))/3f;
        }

        Debug.Log("Population status: " + this.GrowthStatus.ToString() + ", increase rate: " + populationIncreaseRate);
    }

    public void CalculateWaterNeed()
    {
        if (population.NeededWaterTilesPresent)
        {
            //Debug.Log("Water need met");
            IsNeedMet[NeedType.Liquid] = true;
            waterRating = 0;

            // Update water rating for each liquid need
            foreach (LiquidNeedConstructData data in population.species.LiquidNeeds)
            {
                LiquidNeed waterNeed = population.Needs[data.ID] as LiquidNeed;
                float percent = waterNeed.NeedValue;
                float minNeeded = data.MinThreshold;
                float maxWaterTilePercent = GetMaxWaterTilePercent(data.ID.WaterIndex);
                waterRating += (percent - minNeeded) / (maxWaterTilePercent - minNeeded);
            }
        }
        else
        {
            IsNeedMet[NeedType.Liquid] = false;
            waterRating = (population.WaterTilesUsed - population.TotalWaterTilesRequired) / population.TotalWaterTilesRequired;
        }

        //Debug.Log(population.gameObject.name + " water Rating: " + waterRating + ", water source size: "+ waterTilesUsed);
    }

    // Updates IsNeedMet and foodRating
    public void CalculateFoodNeed(float availablePreferredFood, float availableCompatibleFood)
    {
        int numAnimals = population.AnimalPopulation.Count;
        float totalMinFoodNeeded = numAnimals * population.species.MinFoodRequired;
        float totalMaxFoodNeeded = numAnimals * population.species.MaxFoodRequired;
        float totalFoodConsumed = Mathf.Min(availablePreferredFood + availableCompatibleFood, totalMaxFoodNeeded);

        if (totalFoodConsumed >= totalMinFoodNeeded)
        {
            IsNeedMet[NeedType.FoodSource] = true;
            foodRating = 0.5f * ((totalFoodConsumed - totalMinFoodNeeded) / (totalMaxFoodNeeded - totalMinFoodNeeded)) + 0.5f * (availablePreferredFood / totalFoodConsumed);
        }
        else
        {
            IsNeedMet[NeedType.FoodSource] = false;
            foodRating = (totalFoodConsumed - totalMinFoodNeeded) / totalMinFoodNeeded;
        }

        //Debug.Log(population.gameObject.name + " food rating: " + foodRating + ", total food eaten: " + totalFoodConsumed + ", percent of food is preferred: " + (availablePreferredFood / totalFoodConsumed));
    }

    public void CalculateTerrainNeed()
    {
        int numAnimals = population.AnimalPopulation.Count;

        float totalNeededTiles = population.species.TerrainTilesRequired * numAnimals;
        float availablePreferredTiles = 0f;
        float availableSurvivableTiles = 0f;

        float preferredTilesOccupied = 0f;
        float survivableTilesOccupied = 0f;
        float totalTilesOccupied = 0f;

        foreach (KeyValuePair<ItemID, Need> need in population.Needs)
        {
            if (need.Value.NeedType.Equals(NeedType.Terrain))
            {
                if (need.Value.IsPreferred)
                {
                    availablePreferredTiles += need.Value.NeedValue;
                }
                else
                {
                    availableSurvivableTiles += need.Value.NeedValue;
                }
            }
        }

        if (availablePreferredTiles >= totalNeededTiles)
        {
            preferredTilesOccupied = totalNeededTiles;
        }
        else
        {
            preferredTilesOccupied = availableSurvivableTiles;
        }

        if (availableSurvivableTiles >= totalNeededTiles - preferredTilesOccupied)
        {
            survivableTilesOccupied = totalNeededTiles - preferredTilesOccupied;
        }
        else
        {
            survivableTilesOccupied = availableSurvivableTiles;
        }

        totalTilesOccupied = survivableTilesOccupied + preferredTilesOccupied;
        if (totalTilesOccupied >= totalNeededTiles)
        {
            IsNeedMet[NeedType.Terrain] = true;
            terrainRating = preferredTilesOccupied / totalNeededTiles;
        }
        else
        {
            IsNeedMet[NeedType.Terrain] = false;
            terrainRating = (totalTilesOccupied - totalNeededTiles) / totalNeededTiles;
        }

        //Debug.Log(population.gameObject.name + " terrain Rating: " + terrainRating + ", preferred tiles: " + preferredTilesOccupied + ", survivable tiles: " + survivableTilesOccupied);
    }

    public float calculatePredatorPreyNeed()
    {
        float predatorValue = 0;
        foreach (KeyValuePair<ItemID, Need> need in population.Needs)
        {
            if (need.Value.NeedType.Equals(NeedType.Prey))
            {
                predatorValue += need.Value.NeedValue;
            }
        }
        return predatorValue;
    }

    /// <summary>
    /// Compute the new population size when the population grows/shrinks
    /// </summary>
    /// <returns></returns>
    public int CalculateNextPopulationSize()
    {
        float delta = population.Count * populationIncreaseRate;

        // If population is growing then move up to next int
        if (GrowthStatus == GrowthStatus.growing)
        {
            delta = Mathf.Ceil(delta);
        }
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

    public static float GetMaxWaterTilePercent(int waterIndex)
    {
        if (waterIndex == 0) return maxFreshWaterTilePercent;
        else if (waterIndex == 1) return maxSaltTilePercent;
        else if (waterIndex == 2) return maxBacteriaTilePercent;
        else throw new ArgumentException(
            $"No max water tile percent associated with index '{waterIndex}'. " +
            $"Please use a different water index or add another tile percent " +
            $"to the source code for the {nameof(GrowthCalculator)}");
    }
}
