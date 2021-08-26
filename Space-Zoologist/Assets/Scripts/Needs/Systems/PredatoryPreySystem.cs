﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatoryPreySystem : NeedSystem
{

    private readonly GridSystem gridSystem = default;
    private readonly ReservePartitionManager rpm = default;

    public PredatoryPreySystem(ReservePartitionManager rpm, GridSystem gridSystem, NeedType needType = NeedType.Prey) : base(needType)
    {
        this.rpm = rpm;
        this.gridSystem = gridSystem;
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