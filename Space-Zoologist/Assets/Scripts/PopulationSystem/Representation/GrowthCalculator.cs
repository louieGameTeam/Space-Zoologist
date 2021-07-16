using System;
using System.Collections.Generic;
using UnityEngine;

public enum GrowthStatus {declining, stagnate, growing}

/// <summary>
/// Determines the rate and status of growth for each population based off of their most severe need that isn't being met
/// </summary>
public class GrowthCalculator
{
    public GrowthStatus GrowthStatus { get; private set; }
    public Dictionary<NeedType, bool> IsNeedMet = new Dictionary<NeedType, bool>();
    public int GrowthCountdown = 0;
    public int DecayCountdown = 0;
    public float FoodRating => foodRating;
    public float WaterRating => waterRating;
    public float TerrainRating => terrainRating;
    Population Population = default;
    private float foodRating = 0f;
    private float waterRating = 0f;
    private float terrainRating = 0f;
    public float populationIncreaseRate = 0f;

    public GrowthCalculator(Population population)
    {
        this.Population = population;
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
        1. if any need not met, handle growth logic and set growth status
        2. if all needs met, handle growth logic
    */
    public void CalculateGrowth()
    {
        this.GrowthStatus = GrowthStatus.growing;
        int numAnimals = Population.AnimalPopulation.Count;

        populationIncreaseRate = 0f;
        // 1.
        Dictionary<NeedType, bool> resetNeedTracker = new Dictionary<NeedType, bool>(IsNeedMet);
        foreach (KeyValuePair<NeedType, bool> need in IsNeedMet)
        {
            if (!IsNeedMet[need.Key])
            {
                //Debug.Log(need.Key + " is not met");
                GrowthStatus = GrowthStatus.declining;
                if (need.Key.Equals(NeedType.FoodSource))
                {
                    populationIncreaseRate += foodRating;
                }
                if (need.Key.Equals(NeedType.Liquid))
                {
                    populationIncreaseRate += waterRating;
                }
                if (need.Key.Equals(NeedType.Terrain))
                {
                    populationIncreaseRate += terrainRating;
                }
            }
            resetNeedTracker[need.Key] = false;
        }
        if (this.GrowthStatus.Equals(GrowthStatus.declining))
        {
            populationIncreaseRate *= numAnimals * Population.Species.GrowthScaleFactor;
            if (populationIncreaseRate < numAnimals * -0.25f)
            {
                populationIncreaseRate = numAnimals * -0.25f;
            }
        }
        // 2.
        else
        {
            populationIncreaseRate = (foodRating + waterRating + terrainRating) * Population.Species.GrowthScaleFactor * numAnimals;
            if (populationIncreaseRate > numAnimals * 1.5f)
            {
                populationIncreaseRate = numAnimals * 1.5f;
            }
            //Debug.Log(Population.gameObject.name + " will increase at a rate of " + populationIncreaseRate);

        }
        populationIncreaseRate = (int)Math.Round(populationIncreaseRate, 0, MidpointRounding.AwayFromZero);
        IsNeedMet = resetNeedTracker;
    }

    public void CalculateWaterNeed()
    {
        float totalNeedWaterTiles = 0f;
        float percentPureWater = 0f;
        float waterSourceSize = 0f;
        float neededPureWaterThreshold = 0f;
        int numAnimals = Population.AnimalPopulation.Count;
        foreach (KeyValuePair<string, Need> need in Population.Needs)
        {
            if (need.Value.NeedType.Equals(NeedType.Terrain) && need.Key.Equals("Liquid"))
            {
                waterSourceSize = need.Value.NeedValue;
                totalNeedWaterTiles = need.Value.GetMaxThreshold() * numAnimals;
                //Debug.Log("Total needed water tiles: " + totalNeedWaterTiles);
            }
            if (need.Value.NeedType.Equals(NeedType.Liquid) && need.Key.Equals("Water"))
            {
                percentPureWater = need.Value.NeedValue;
                neededPureWaterThreshold = need.Value.GetMaxThreshold();
            }
        }
        float waterTilesUsed = 0;
        if (waterSourceSize > totalNeedWaterTiles)
        {
            waterTilesUsed = totalNeedWaterTiles;
        }
        else
        {
            waterTilesUsed = waterSourceSize;
        }
        if (waterTilesUsed >= totalNeedWaterTiles && percentPureWater >= neededPureWaterThreshold)
        {
            IsNeedMet[NeedType.Liquid] = true;
            waterRating = 1 + (percentPureWater - neededPureWaterThreshold) * 100.0f;
        }
        else if (waterTilesUsed >= totalNeedWaterTiles && percentPureWater < neededPureWaterThreshold)
        {
            IsNeedMet[NeedType.Liquid] = false;
            waterRating = (percentPureWater - neededPureWaterThreshold) * 100.0f;
        }
        else
        {
            IsNeedMet[NeedType.Liquid] = false;
            waterRating = (waterTilesUsed - totalNeedWaterTiles) / numAnimals;
        }
        //Debug.Log(Population.gameObject.name + " water Rating: " + waterRating + ", water source size: "+ waterTilesUsed);
    }

    // Updates IsNeedMet and foodRating
    public void CalculateFoodNeed(float availablePreferredFood, float availableCompatibleFood)
    {
        float totalMinFoodNeeded = 0f;
        int numAnimals = Population.AnimalPopulation.Count;
        foreach (KeyValuePair<string, Need> need in Population.Needs)
        {
            if (need.Value.NeedType.Equals(NeedType.FoodSource))
            {
                totalMinFoodNeeded = need.Value.GetMinThreshold() * numAnimals;
                break;
            }
        }
        float totalFoodConsumed = availablePreferredFood + availableCompatibleFood;
        if (totalFoodConsumed >= totalMinFoodNeeded)
        {
            IsNeedMet[NeedType.FoodSource] = true;
            foodRating = (availablePreferredFood / numAnimals) + ((totalFoodConsumed - totalMinFoodNeeded) / numAnimals);
        }
        else
        {
            IsNeedMet[NeedType.FoodSource] = false;
            foodRating = (totalFoodConsumed - totalMinFoodNeeded) / numAnimals;
        }
        //Debug.Log(Population.gameObject.name + " food rating: " + foodRating + ", preferred food value: " + availablePreferredFood + ", compatible food value: " + availableCompatibleFood);

    }

    public void CalculateTerrainNeed()
    {
        float totalNeededTiles = 0f;
        float comfortableTilesOccupied = 0f;
        float survivableTilesOccupied = 0f;
        float totalTilesOccupied = 0f;
        float availableComfortableTiles = 0f;
        float availableSurvivableTiles = 0f;
        int numAnimals = Population.AnimalPopulation.Count;
        foreach (KeyValuePair<string, Need> need in Population.Needs)
        {
            if (need.Value.NeedType.Equals(NeedType.Terrain) && !need.Key.Equals("Liquid"))
            {
                if (need.Value.IsPreferred)
                {
                    availableComfortableTiles = need.Value.NeedValue;
                }
                else
                {
                    availableSurvivableTiles += need.Value.NeedValue;
                }
                totalNeededTiles = need.Value.GetMaxThreshold() * numAnimals;
            }
        }
        if (availableComfortableTiles >= totalNeededTiles)
        {
            comfortableTilesOccupied = totalNeededTiles;
        }
        else
        {
            comfortableTilesOccupied = availableSurvivableTiles;
        }
        if (availableSurvivableTiles >= totalNeededTiles - comfortableTilesOccupied)
        {
            survivableTilesOccupied = totalNeededTiles - comfortableTilesOccupied;
        }
        else
        {
            survivableTilesOccupied = availableSurvivableTiles;
        }
        totalTilesOccupied = survivableTilesOccupied + comfortableTilesOccupied;
        if (totalTilesOccupied >= totalNeededTiles)
        {
            IsNeedMet[NeedType.Terrain] = true;
            terrainRating = 1 + comfortableTilesOccupied / numAnimals;
        }
        else
        {
            IsNeedMet[NeedType.Terrain] = false;
            terrainRating = (totalTilesOccupied - totalNeededTiles) / numAnimals;
        }
        //Debug.Log(Population.gameObject.name + " terrain Rating: " + terrainRating + ", comfortable tiles: " + comfortableTilesOccupied + ", survivable tiles: " + survivableTilesOccupied);
    }

    public bool ReadyForDecay()
    {
        this.DecayCountdown--;
        if (this.DecayCountdown == 0)
        {
            this.DecayCountdown = Population.Species.GrowthRate;
            return true;
        }
        return false;
    }

    public bool ReadyForGrowth()
    {
        this.GrowthCountdown--;
        if (this.GrowthCountdown == 0)
        {
            this.GrowthCountdown = Population.Species.GrowthRate;
            return true;
        }
        return false;
    }
}
