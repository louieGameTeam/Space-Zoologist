using System.Collections.Generic;

public enum GrowthStatus {increasing, stabilized, decreasing}

/// <summary>
/// Determines the rate and status of growth for each population based off of their most severe need that isn't being met
/// </summary>
public class GrowthCalculator
{
    public GrowthStatus GrowthStatus { get; private set; }
    public float GrowthRate { get; private set; }

    public GrowthCalculator()
    {
        this.GrowthRate = 0;
        this.GrowthStatus = GrowthStatus.stabilized;
    }

    public void CalculateGrowth(Population population)
    {
        int worstSeverity = 0;
        int totalSeverity = 0;
        foreach(KeyValuePair<string, Need> need in population.Needs)
        {
            switch(need.Value.GetCondition(need.Value.Value))
            {
                case NeedCondition.Bad:
                    if (need.Value.Severity > worstSeverity)
                    {
                        worstSeverity = need.Value.Severity;
                    }
                    totalSeverity += need.Value.Severity;
                    break;
                default:
                    break;
            }
        }
        if (worstSeverity == 5 || worstSeverity == 6)
        {
            this.GrowthStatus = GrowthStatus.stabilized;
            this.GrowthRate = 0;
        }
        else if (worstSeverity > 6)
        {
            this.GrowthStatus = GrowthStatus.decreasing;
            // 15, 30, 45, 60 seconds depending on how severe
            this.GrowthRate = (6 - worstSeverity) * 15 + 75;
        }
        else
        {
            this.GrowthStatus = GrowthStatus.increasing;
            this.GrowthRate = population.Species.GrowthRate + totalSeverity;
        }
    }
}
