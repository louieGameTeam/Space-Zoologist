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

    // Holds which consumed population each consumer population has access to.
    Dictionary<Population, HashSet<Population>> accessiblePopulation = new Dictionary<Population, HashSet<Population>>();
    // Holds which consumer populations have access to each consumed population, the opposite of accessiblePopulation.
    Dictionary<Population, HashSet<Population>> populationsWithAccess = new Dictionary<Population, HashSet<Population>>();

    public SpeciesNeedSystem(ReservePartitionManager rpm, string needName) : base(needName)
    {
        this.rpm = rpm;
    }

    public override bool CheckState()
    {
        // Checks on consumer
        if (base.CheckState())
        {
            return true;
        }

        foreach (Population consumedSource in this.populations)
        {
            if(rpm.PopulationAccessbilityStatus[consumedSource])
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Updates and updates the associated need in the Population's needs
    /// </summary>
    public override void UpdateSystem()
    {

        // Stop update if there is no population to distribut
        if (populations.Count == 0 || Consumers.Count == 0)
        {
            this.isDirty = false;
            return;
        }

        // When accessbility of population changes a full reset should be triggered
        foreach (Population consumer in Consumers)
        {
            if (rpm.PopulationAccessbilityStatus[consumer])
            {
                //Debug.Log($"{consumer} triggered a reset");

                foreach (Population consumed in populations)
                {
                    if (rpm.CanAccess(consumer, consumed.transform.position))
                    {
                        accessiblePopulation[consumer].Add(consumed);
                        populationsWithAccess[consumed].Add(consumer);
                    }
                }
                //rpm.PopulationAccessbilityStatus[consumer] = false;
            }
        }

        // Holds how much consumed population count each population has left after consumer populations take from them.
        Dictionary<Population, float> amountPopulationCountRemaining = new Dictionary<Population, float>();

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

            // Reset amountPopulationCountRemaining
            amountPopulationCountRemaining.Add(population, population.Count);
        }

        // Holds the populations that will not have enough food to give them a good condition.
        HashSet<Population> populationsWithNotEnough = new HashSet<Population>();

        // Holds the ratio of population count of the consumed population to each consumer population
        // Note: accessibleAreaRatio[consumer][consumed]
        Dictionary<Population, Dictionary<Population, float>> accessibleAreaRatio = new Dictionary<Population, Dictionary<Population, float>>();

        // Foreach population, if it is in good condition from the food available to it, then take its portion and update its need,
        // else, add it to the set of populations that will not have enough. The populations without enough will then split what is 
        // remaining based on the ratio of their dominance to the localRemainingDominance for each of their FoodSources.
        foreach (Population life in Consumers)
        {
            int availablePopulationCount = 0;
            float amountRequiredPerIndividualForGoodCondition = life.Species.Needs[base.NeedName].GetThreshold(NeedCondition.Good, -1, false);
            float amountRequiredForGoodCondition = amountRequiredPerIndividualForGoodCondition * life.Count;

            foreach (Population population in accessiblePopulation[life])
            {
                // All area consumed population has access to
                List<Vector3Int> accessibleArea = rpm.GetLocationsWithAccess(population);
                // Where both the consumer and consumed population has access to
                List<Vector3Int> overlapArea = accessibleArea.Where(cell => rpm.CanAccess((Population)life, cell)).ToList();

                accessibleAreaRatio.Add(life, new Dictionary<Population, float>());

                accessibleAreaRatio[life][population] = (float)overlapArea.Count / (float)accessibleArea.Count;
                int accessiblePopulationCout = (int)Math.Floor(population.Count * accessibleAreaRatio[life][population]);
                availablePopulationCount += (int)Math.Floor(accessiblePopulationCout * (life.Dominance / totalLocalDominance[population]));

                Debug.Log($"{life.Species.SpeciesName} {life.GetInstanceID()} can took {availablePopulationCount}");
            }

            // If the food available to the Population is more than enough, only take enough and update its need.
            if (availablePopulationCount >= amountRequiredForGoodCondition)
            {
                float totalPopulationCountAcquired = 0.0f;
                foreach (Population population in accessiblePopulation[life])
                {
                    // Take as much as the amount it needs to be in good condition
                    float populationCountAcquired = amountRequiredForGoodCondition;
                    amountPopulationCountRemaining[population] -= populationCountAcquired;
                    totalPopulationCountAcquired += populationCountAcquired;
                    localDominanceRemaining[population] -= life.Dominance;
                    Debug.Log($"{life.Species.SpeciesName} population took {populationCountAcquired} food from foodsource at {population.Species.SpeciesName}");
                }
                float populationCountAcquiredPerIndividual = totalPopulationCountAcquired / life.Count;
                life.UpdateNeed(NeedName, populationCountAcquiredPerIndividual);
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
            float totalPopulationCountAcquired = 0.0f;
            foreach (Population population in accessiblePopulation[life])
            {
                //// All area consumed population has access to
                //List<Vector3Int> accessibleArea = rpm.GetLocationsWithAccess(population);
                //// Where both the consumer and consumed population has access to
                //List<Vector3Int> overlapArea = accessibleArea.Where(cell => rpm.CanAccess((Population)life, cell)).ToList();
                //float accessibleAreaRatio = (float)overlapArea.Count / (float)accessibleArea.Count;

                float dominanceRatio = life.Dominance / localDominanceRemaining[population];
                totalPopulationCountAcquired += (float)Math.Floor(dominanceRatio * accessibleAreaRatio[life][population] * amountPopulationCountRemaining[population]);
                Debug.Log($"{life.Species.SpeciesName} {life.GetInstanceID()} population took {totalPopulationCountAcquired} pop count from {population.Species.SpeciesName} {population.GetInstanceID()}");
            }
            float amountAcquiredPerIndividual = totalPopulationCountAcquired / life.Count;
            life.UpdateNeed(this.NeedName, amountAcquiredPerIndividual);
        }

        // Done update not dirty any more
        this.isDirty = false;
    }

    public void AddPopulation(Population population)
    {
        populations.Add(population);

        populationsWithAccess.Add(population, new HashSet<Population>());
        foreach (Population consumer in Consumers)
        {
            if (rpm.CanAccessPopulation(consumer, population))
            {
                accessiblePopulation[consumer].Add(population);
                populationsWithAccess[population].Add(consumer);
            }
        }

        this.isDirty = true;
    }

    public override void AddConsumer(Life life)
    {
        base.AddConsumer(life);

        Population consumer = (Population)life;
        accessiblePopulation.Add(consumer, new HashSet<Population>());
        foreach (Population population in populations)
        {
            if (rpm.CanAccessPopulation(consumer, population))
            {
                accessiblePopulation[consumer].Add(population);
                populationsWithAccess[population].Add(consumer);
            }
        }

        this.isDirty = true;
    }
}