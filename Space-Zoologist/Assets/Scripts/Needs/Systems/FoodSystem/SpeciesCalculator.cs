﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using UnityEngine;

public class SpeciesCalculator : NeedCalculator
{
    public string SpeciesName => this.speciesName;
    public List<Population> Consumers => this.consumers;
    public List<Population> Populations => this.populations;
    public bool IsDirty => this.isDirty;

    private string speciesName = default;
    private List<Population> consumers = new List<Population>();
    private List<Population> populations = new List<Population>();
    private bool isDirty = default;
    private readonly ReservePartitionManager rpm = null;

    // Holds which consumed population each consumer population has access to.
    Dictionary<Population, HashSet<Population>> accessiblePopulation = new Dictionary<Population, HashSet<Population>>();
    // Holds which consumer populations have access to each consumed population, the opposite of accessiblePopulation.
    Dictionary<Population, HashSet<Population>> populationsWithAccess = new Dictionary<Population, HashSet<Population>>();

    // How much pop count each consumer was distributed
    Dictionary<Population, float> amountConsumerWasDistributed = new Dictionary<Population, float>();

    Dictionary<Population, int> distributedAmount = new Dictionary<Population, int>();

    public SpeciesCalculator(ReservePartitionManager rpm, string speciesName)
    {
        this.speciesName = speciesName;
        this.rpm = rpm;
    }

    public void MarkDirty()
    {
        this.isDirty = true;
    }

    public void AddSource(Life source)
    {
        Population population = (Population)source;

        this.populations.Add(population);

        populationsWithAccess.Add(population, new HashSet<Population>());
        foreach (Population consumer in Consumers)
        {
            if (rpm.CanAccess(consumer, population.GetPosition()))
            {
                accessiblePopulation[consumer].Add(population);
                populationsWithAccess[population].Add(consumer);
            }
        }

        this.isDirty = true;
    }

    public bool RemoveSource(Life source)
    {
        Population population = (Population)source;

        Debug.Assert(!this.populations.Remove(population), "Population removal failure!");
        Debug.Assert(!this.populationsWithAccess.Remove(population), "REmoval of cosumed pop from populationsWithAccess failed");

        this.isDirty = true;

        return true;
    }

    public bool RemoveConsumer(Population consumer)
    {
        this.isDirty = true;

        Debug.Assert(!this.consumers.Remove(consumer), "Consumer removal failure!");
        Debug.Assert(!this.accessiblePopulation.Remove(consumer), "Removal of consumer from accessiblePopulation failed");

        return true;
    }

    public void AddConsumer(Population consumer)
    {
        if (this.consumers.Contains(consumer))
        {
            return;
        }

        this.consumers.Add(consumer);

        //Population population = (Population)life;
        accessiblePopulation.Add(consumer, new HashSet<Population>());
        foreach (Population population in this.populations)
        {
            if (rpm.CanAccess(consumer, population.GetPosition()))
            {
                accessiblePopulation[consumer].Add(population);
                populationsWithAccess[population].Add(consumer);
            }
        }

        this.isDirty = true;
    }

    public void RemoveAnimalFromConsumedPopulation()
    {
        foreach (Population population in this.distributedAmount.Keys)
        {
            population.RemoveAnimal(distributedAmount[population]);
            //Debug.Log($"Removed {distributedAmount[population]} from {population.Species.SpeciesName}");
        }
    }

    public Dictionary<Population, float> CalculateDistribution()
    {

        // Stop update if there is no population to distribut
        if (populations.Count == 0 || Consumers.Count == 0)
        {
            this.isDirty = false;
            return null;
        }

        // When accessbility of population changes a full reset should be triggered
        foreach (Population consumer in Consumers)
        {
            if (consumer.HasAccessibilityChanged)
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
            }
        }

        // Holds how much consumed population count each population has left after consumer populations take from them.
        Dictionary<Population, float> amountPopulationCountRemaining = new Dictionary<Population, float>();

        // Holds the sum of the Dominance of all Populations that have access to the consumed population.
        Dictionary<Population, float> totalLocalDominance = new Dictionary<Population, float>();
        // Holds the remaining Dominance of each consumed population after populations have already taken their share.
        Dictionary<Population, float> localDominanceRemaining = new Dictionary<Population, float>();

        Dictionary<Population, int> newDistributedAmount = new Dictionary<Population, int>();
        foreach (Population population in this.populations)
        {
            newDistributedAmount[population] = 0;
        }

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
        foreach (Population consumer in Consumers)
        {
            int availablePopulationCount = 0;
            float amountRequiredPerIndividualForGoodCondition = consumer.Needs[this.speciesName].GetThreshold(NeedCondition.Good, -1, false);
            float amountRequiredForGoodCondition = amountRequiredPerIndividualForGoodCondition * consumer.Count;

            foreach (Population population in accessiblePopulation[consumer])
            {
                // All area consumed population has access to
                List<Vector3Int> accessibleArea = rpm.GetLocationsWithAccess(population);
                // Where both the consumer and consumed population has access to
                List<Vector3Int> overlapArea = accessibleArea.Where(cell => rpm.CanAccess((Population)consumer, cell)).ToList();

                accessibleAreaRatio.Add(consumer, new Dictionary<Population, float>());

                accessibleAreaRatio[consumer][population] = (float)overlapArea.Count / (float)accessibleArea.Count;
                int accessiblePopulationCout = (int)Math.Floor(population.Count * accessibleAreaRatio[consumer][population]);
                availablePopulationCount += (int)Math.Floor(accessiblePopulationCout * (consumer.Dominance / totalLocalDominance[population]));
            }

            // If the food available to the Population is more than enough, only take enough and update its need.
            if (availablePopulationCount >= amountRequiredForGoodCondition)
            {
                float totalPopulationCountAcquired = 0.0f;
                foreach (Population population in accessiblePopulation[consumer])
                {
                    // Take as much as the amount it needs to be in good condition
                    amountPopulationCountRemaining[population] -= amountRequiredForGoodCondition;
                    totalPopulationCountAcquired += amountRequiredForGoodCondition;
                    newDistributedAmount[population] += (int)totalPopulationCountAcquired;
                    localDominanceRemaining[population] -= consumer.Dominance;
                }
                float populationCountAcquiredPerIndividual = totalPopulationCountAcquired / consumer.Count;
                this.amountConsumerWasDistributed[consumer] = populationCountAcquiredPerIndividual;
            }
            // Otherwise, add to the set of populationsWithNotEnough to be processed later.
            else
            {
                populationsWithNotEnough.Add(consumer);
            }
        }

        // For those Populations who will not have enough to be in good condition, split what food remains from each FoodSource.
        foreach (Population consumer in populationsWithNotEnough)
        {
            float totalPopulationCountAcquired = 0.0f;
            foreach (Population population in accessiblePopulation[consumer])
            {
                float dominanceRatio = consumer.Dominance / localDominanceRemaining[population];
                totalPopulationCountAcquired += (float)Math.Floor(dominanceRatio * accessibleAreaRatio[consumer][population] * amountPopulationCountRemaining[population]);
                newDistributedAmount[population] += (int)totalPopulationCountAcquired;
                //Debug.Log($"{life.Species.SpeciesName} {life.GetInstanceID()} population took {totalPopulationCountAcquired} pop count from {population.Species.SpeciesName} {population.GetInstanceID()}");
            }
            float amountAcquiredPerIndividual = totalPopulationCountAcquired / consumer.Count;
            this.amountConsumerWasDistributed[consumer] = amountAcquiredPerIndividual;
        }

        // Done update not dirty any more
        this.isDirty = false;

        this.distributedAmount = newDistributedAmount;

        return this.amountConsumerWasDistributed;
    }
}
