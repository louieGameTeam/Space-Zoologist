using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableGrowthCalculator
{
    #region Public Fields
    public float changeRate;
    public string growthStatus;
    public int growthCountdown;
    public int decayCountdown;
    #endregion

    #region Constructors
    public SerializableGrowthCalculator(GrowthCalculator calculator)
    {
        if (GameManager.Instance.needRatings.HasRating(calculator.Population))
        {
            changeRate = calculator.ChangeRate;
            growthStatus = calculator.GrowthStatus.ToString();
        }
        growthCountdown = calculator.GrowthCountdown;
        decayCountdown = calculator.DecayCountdown;
    }
    #endregion
}