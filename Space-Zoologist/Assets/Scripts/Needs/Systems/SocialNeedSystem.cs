using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialNeedSystem : NeedSystem
{
    private readonly ReservePartitionManager rpm = default;

    public SocialNeedSystem(NeedType needType = NeedType.Social) : base(needType)
    {
        this.rpm = GameManager.Instance.m_reservePartitionManager;
    }

    /* 
     * Friend set = min num of animals in friend populaiton
     */
    public override void UpdateSystem()
    {
        foreach (Life life in Consumers)
        {
            if (life.GetType() == typeof(Population))
            {
                Population population = (Population)life;
                foreach (KeyValuePair<string, Need> need in population.Needs)
                {
                    if (need.Value.NeedType.Equals(NeedType.Social))
                    {
                        int needValue = 0;
                        int numCompatibleFriendSets = int.MaxValue;
                        int numPreferredFriendSets = int.MaxValue;
                        int numPreferred = 0;
                        int numCompatible = 0;
                        foreach (Population friend in rpm.Populations)
                        {
                            if (friend.Species.SpeciesName.Equals(need.Value.NeedName))
                            {
                                if (rpm.CanAccessPopulation(population, friend))
                                {
                                    if (need.Value.IsPreferred)
                                    {
                                        numPreferred += 1;
                                        if (friend.AnimalPopulation.Count < numPreferredFriendSets)
                                        {
                                            numPreferredFriendSets = friend.AnimalPopulation.Count;
                                        }
                                    }
                                    else
                                    {
                                        numCompatible += 1;
                                        if (friend.AnimalPopulation.Count < numCompatibleFriendSets)
                                        {
                                            numCompatibleFriendSets = friend.AnimalPopulation.Count;
                                        }
                                    }
                                }
                            }
                        }
                        population.UpdateNeed(need.Value.NeedName, needValue);
                    }
                }
            }
        }
    }
}
