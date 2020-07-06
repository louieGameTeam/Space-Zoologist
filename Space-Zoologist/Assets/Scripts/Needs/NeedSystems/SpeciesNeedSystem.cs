using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System; // Math.Floor()

/// <summary>
/// NeedSystem for species need.
/// </summary>
/// <param name="lives">A list of <c>Life</c> consumer populations, in this system is grenteed to be only <c>Population</c></param>
/// <param name="populations">A internal list of <c>Population</c> as consumed animal populations</param>
public class SpeciesNeedSystem : NeedSystem
{
    private List<Population> populations = new List<Population>();
    private readonly ReservePartitionManager rpm = null;
    public SpeciesNeedSystem(string needName, ReservePartitionManager rpm) : base(needName)
    {
        this.rpm = rpm;
    }

    /// <summary>
    /// Updates and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {
        // Tester update
        //foreach (Life live in lives)
        //{
        //    live.UpdateNeed(this.NeedName, 10f);
        //}

        // Stop update if there is no population to distribut
        if (populations.Count == 0 || lives.Count == 0) return;

        // Holds which consumed population each consumer population has access to.
        Dictionary<Population, HashSet<Population>> accessiblePopulation = new Dictionary<Population, HashSet<Population>>();
        // Holds which consumer populations have access to each consumed population, the opposite of accessiblePopulation.
        Dictionary<Population, HashSet<Population>> populationsWithAccess = new Dictionary<Population, HashSet<Population>>();
        // Holds how much consumed population count each population has left after consumer populations take from them.
        Dictionary<Population, float> amountPopulationCountRemaining = new Dictionary<Population, float>();

        // Initialize accessiblePopulation, populationsWithAccess, amd amountPopulationCountRemaining.
        foreach (Population population in populations)
        {
            populationsWithAccess.Add(population, new HashSet<Population>());
            amountPopulationCountRemaining.Add(population, population.Count);
            foreach (Population live in lives)
            {
                if (!accessiblePopulation.ContainsKey(live))
                {
                    accessiblePopulation.Add(live, new HashSet<Population>());
                }
                if (rpm.CanAccessPopulation(live, population))
                {
                    accessiblePopulation[live].Add(population);
                    populationsWithAccess[population].Add(live);
                }
            }
        }

        // Holds the sum of the Dominance of all Populations that have access to the consumed population.
        Dictionary<Population, float> totalLocalDominance = new Dictionary<Population, float>();
        // Holds the remaining Dominance of each consumed population after populations have already taken their share.
        Dictionary<Population, float> localDominanceRemaining = new Dictionary<Population, float>();

        // Initialize totalLocalDominance and localDominanceRemaining.
        foreach (Population population in populations)
        {
            float total = populationsWithAccess[population].Sum(p => p.Dominance);
            totalLocalDominance.Add(population, total);
            localDominanceRemaining.Add(population, total);
        }

        // Holds the populations that will not have enough food to give them a good condition.
        HashSet<Population> populationsWithNotEnough = new HashSet<Population>();

        // Holds the ratio of population count of the consumed population to each consumer population
        // accessibleAreaRatio[consumer][consumed]
        Dictionary<Population, Dictionary<Population, float>> accessibleAreaRatio = new Dictionary<Population, Dictionary<Population, float>>();

        // Foreach population, if it is in good condition from the food available to it, then take its portion and update its need,
        // else, add it to the set of populations that will not have enough. The populations without enough will then split what is 
        // remaining based on the ratio of their dominance to the localRemainingDominance for each of their FoodSources.
        foreach (Population life in lives)
        {
            int availableFood = 0;
            float amountRequiredPerIndividualForGoodCondition = life.Species.Needs[base.NeedName].GetThreshold(NeedCondition.Good, -1, false);
            float amountRequiredForGoodCondition = amountRequiredPerIndividualForGoodCondition * life.Count;

            foreach (Population population in accessiblePopulation[life])
            {
                // All area consumed population has access to
                List<Vector3Int> accessibleArea = rpm.GetLocationsWithAccess(population);
                // Where both the consumer and consumed population has access to
                List<Vector3Int> overlapArea = accessibleArea.Where(cell => rpm.CanAccess((Population)life,cell)).ToList();

                accessibleAreaRatio.Add(life, new Dictionary<Population, float>());

                accessibleAreaRatio[life][population] = (float)overlapArea.Count / (float)accessibleArea.Count;
                int accessiblePopulationCout = (int)Math.Floor(population.Count * accessibleAreaRatio[life][population]);
                availableFood += (int)Math.Floor(accessiblePopulationCout * (life.Dominance / totalLocalDominance[population]));

                Debug.Log($"{life.Species.SpeciesName} {life.GetInstanceID()} can took {availableFood}");
            }

            // If the food available to the Population is more than enough, only take enough and update its need.
            if (availableFood >= amountRequiredForGoodCondition)
            {
                float totalFoodAcquired = 0.0f;
                foreach (Population population in accessiblePopulation[life])
                {
                    // Take as much as the amount it needs to be in good condition
                    float foodAcquired = amountRequiredForGoodCondition;
                    amountPopulationCountRemaining[population] -= foodAcquired;
                    totalFoodAcquired += foodAcquired;
                    localDominanceRemaining[population] -= life.Dominance;
                    Debug.Log($"{life.Species.SpeciesName} population took {foodAcquired} food from foodsource at {population.Species.SpeciesName}");
                }
                float foodAcquiredPerIndividual = totalFoodAcquired / life.Count;
                life.UpdateNeed(NeedName, foodAcquiredPerIndividual);
            }
            // Otherwise, add to the set of populationsWithNotEnough to be processed later.
            else
            {
                populationsWithNotEnough.Add(life);
            }
        }

        // For those Populations who will not have enough to be in good condition, split what food remains from each FoodSource.
        foreach (Population life in populationsWithNotEnough)
        {
            float foodAcquired = 0.0f;
            foreach (Population population in accessiblePopulation[life])
            {
                //// All area consumed population has access to
                //List<Vector3Int> accessibleArea = rpm.GetLocationsWithAccess(population);
                //// Where both the consumer and consumed population has access to
                //List<Vector3Int> overlapArea = accessibleArea.Where(cell => rpm.CanAccess((Population)life, cell)).ToList();
                //float accessibleAreaRatio = (float)overlapArea.Count / (float)accessibleArea.Count;

                float dominanceRatio = life.Dominance / localDominanceRemaining[population];
                foodAcquired += (float)Math.Floor(dominanceRatio * accessibleAreaRatio[life][population] * amountPopulationCountRemaining[population]);
                Debug.Log($"{life.Species.SpeciesName} {life.GetInstanceID()} population took {foodAcquired} food from {population.Species.SpeciesName} {population.GetInstanceID()}");
            }
            float amountAcquiredPerIndividual = foodAcquired / life.Count;
            life.UpdateNeed(this.NeedName, amountAcquiredPerIndividual);
        }

        //// Done update not dirty any more
        //isDirty = false;
    }

    public void AddPopulation(Population population)
    {
        populations.Add(population);
    }
}
