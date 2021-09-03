using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTerrainNeedSystem : NeedSystem
{
    private readonly ReservePartitionManager rpm = default;

    public TreeTerrainNeedSystem(NeedType needType = NeedType.TreeTerrain) : base(needType)
    {
        this.rpm = GameManager.Instance.m_reservePartitionManager;
    }

    /* 
     * Assuming
     */
    public override void UpdateSystem()
    {
        foreach (Life life in Consumers)
        {
            if (life.GetType() == typeof(Population))
            {
                Population population = (Population)life;
                int numSources = 0;
                foreach (KeyValuePair<string, Need> need in population.Needs)
                {
                    if (need.Value.NeedType.Equals(NeedType.TreeTerrain))
                    {
                        List<FoodSource> foodSources = GameManager.Instance.m_foodSourceManager.GetFoodSourcesWithSpecies(need.Value.NeedName);
                        foreach (FoodSource foodSource in foodSources)
                        {
                            if (GameManager.Instance.m_reservePartitionManager.CanAccess(population, foodSource.Position))
                            {
                                numSources += 1;
                            }
                        }
                        need.Value.UpdateNeedValue(numSources);
                    }
                }
            }
        }
    }
}
