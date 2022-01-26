using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableGrowthCalculator
{
    #region Public Typedefs
    [System.Serializable]
    public class NeedMetEntry
    {
        public string need;
        public bool met;

        public NeedMetEntry(KeyValuePair<NeedType, bool> entry)
        {
            need = entry.Key.ToString();
            met = entry.Value;
        }
    }
    #endregion

    #region Public Fields
    public float maxFreshWaterTilePercent = GrowthCalculator.maxFreshWaterTilePercent;
    public float maxSaltTilePercent = GrowthCalculator.maxSaltTilePercent;
    public float maxBacteriaTilePercent = GrowthCalculator.maxBacteriaTilePercent;

    public string growthStatus;
    public int growthCountdown;
    public int decayCountdown;
    public float foodRating;
    public float waterRating;
    public float terrainRating;
    public float populationIncreaseRate;
    public List<NeedMetEntry> isNeedMet;
    #endregion

    #region Constructors
    public SerializableGrowthCalculator(GrowthCalculator calculator)
    {
        growthStatus = calculator.GrowthStatus.ToString();
        growthCountdown = calculator.GrowthCountdown;
        decayCountdown = calculator.DecayCountdown;
        foodRating = calculator.FoodRating;
        waterRating = calculator.WaterRating;
        terrainRating = calculator.TerrainRating;
        populationIncreaseRate = calculator.populationIncreaseRate;

        // Create a list with all need met entries
        IEnumerable<NeedMetEntry> entries = calculator.IsNeedMet
            .Select(entry => new NeedMetEntry(entry));
        isNeedMet = new List<NeedMetEntry>(entries);
    }
    #endregion
}