using System.Collections.Generic;

public enum GrowthStatus {increasing, stabalized, decreasing}
/*
    Use conditions of each need to determine
    1. if the population is increasing, stabalized, or decreasing
    2. at what rate the population size is changing
*/
public class GrowthCalculator
{
    public GrowthStatus GrowthStatus { get; private set; }
    public float GrowthRate { get; private set; }

    public GrowthCalculator()
    {
        this.GrowthRate = 10000000;
        this.GrowthStatus = GrowthStatus.stabalized;
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
            this.GrowthStatus = GrowthStatus.stabalized;
            this.GrowthRate = 0;
        }
        else if (worstSeverity > 6)
        {
            this.GrowthStatus = GrowthStatus.decreasing;
            // 15, 30, 45, 60 depending on how severe
            this.GrowthRate = (6 - worstSeverity) * 15 + 75;
        }
        else
        {
            this.GrowthStatus = GrowthStatus.increasing;
            this.GrowthRate = population.Species.GrowthRate + totalSeverity;
        }
    }
}
