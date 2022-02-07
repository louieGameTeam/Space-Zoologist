using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatoryPreySystem : NeedSystem
{

    private readonly TileDataController gridSystem = default;
    private readonly ReservePartitionManager rpm = default;

    public PredatoryPreySystem(NeedType needType = NeedType.Prey) : base(needType)
    {
        this.rpm = GameManager.Instance.m_reservePartitionManager;
        this.gridSystem = GameManager.Instance.m_tileDataController;
    }

    /* 
     * Population decays at a rate equal to min(numPredatorsNearby, totalTilesSharedWithPredator)
     */
    public override void UpdateSystem()
    {
        foreach (Life life in Consumers)
        {
            if (life.GetType() == typeof(Population))
            {
                Population prey = (Population)life;
                foreach (KeyValuePair<string, Need> need in prey.Needs)
                {
                    if (need.Value.NeedType.Equals(NeedType.Prey))
                    {
                        int needValue = 0;
                        foreach (Population potentialPredator in rpm.Populations)
                        {
                            if (potentialPredator.Species.SpeciesName.Equals(need.Value.NeedName))
                            {
                                int numOverlapTiles = rpm.NumOverlapTiles(prey, potentialPredator);
                                if (numOverlapTiles > potentialPredator.Count)
                                {
                                    needValue += potentialPredator.Count;
                                }
                                else
                                {
                                    needValue += numOverlapTiles;
                                }
                            }
                        }
                        prey.UpdateNeed(need.Value.NeedName, needValue);
                    }
                }
            }
        }
    }
}
