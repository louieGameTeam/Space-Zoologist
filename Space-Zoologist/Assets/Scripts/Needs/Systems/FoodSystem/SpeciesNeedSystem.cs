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

    // Species name to food calculators
    private Dictionary<string, SpeciesCalculator> speciesCalculators = new Dictionary<string, SpeciesCalculator>();

    public SpeciesNeedSystem(ReservePartitionManager rpm, NeedType needType = NeedType.Species) : base(needType)
    {
        this.rpm = rpm;
    }

    public override bool CheckState()
    {
        bool needUpdate = false;

        foreach (SpeciesCalculator speciesCalculator in this.speciesCalculators.Values)
        {
            // Check if consumer is dirty
            foreach (Population consumer in speciesCalculator.Consumers)
            {
                if (consumer.GetAccessibilityStatus())
                {
                    speciesCalculator.MarkDirty();
                    needUpdate = true;
                    break;
                }
            }

            // If consumer is already dirty check next food source calculator
            if (needUpdate)
            {
                break;
            }
            // Check if consumed population is dirty
            else
            {
                foreach (Population population in speciesCalculator.Populations)
                {
                    if (population.HasAccessibilityChanged)
                    {
                        speciesCalculator.MarkDirty();
                        needUpdate = true;
                        break;
                    }
                }
            }
        }

        return needUpdate;
    }

   
    public void AddPopulation(Population population)
    {
        if (!this.speciesCalculators.ContainsKey(population.Species.SpeciesName))
        {
            this.speciesCalculators.Add(population.Species.SpeciesName, new SpeciesCalculator(rpm, population.Species.SpeciesName));
        }

        this.speciesCalculators[population.Species.SpeciesName].AddSource(population);

        this.isDirty = true;
    }

    public override void AddConsumer(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            // Check if the need is a 'Species' type
            if (need.NeedType == NeedType.Species)
            {
                // Create a food source calculator for this food source,
                // if not already exist
                if (!this.speciesCalculators.ContainsKey(need.NeedName))
                {
                    this.speciesCalculators.Add(need.NeedName, new SpeciesCalculator(rpm, need.NeedName));
                }

                // Add consumer to food source calculator
                this.speciesCalculators[need.NeedName].AddConsumer((Population)life);
            }
        }

        this.isDirty = true;
    }

    public override void MarkAsDirty()
    {
        base.MarkAsDirty();

        foreach (SpeciesCalculator speciesCalculator in this.speciesCalculators.Values)
        {
            speciesCalculator.MarkDirty();
        }
    }

    public override void UpdateSystem()
    {
        foreach (SpeciesCalculator speciesCalculator in this.speciesCalculators.Values)
        {
            if (speciesCalculator.IsDirty)
            {
                var distribution = speciesCalculator.CalculateDistribution();

                // distibution returns null when not thing to be updated
                if (distribution != null)
                {
                    foreach (Population consumer in distribution.Keys)
                    {
                        consumer.UpdateNeed(speciesCalculator.SpeciesName, distribution[consumer]);
                    }
                }
            }
        }

        this.isDirty = false;
    }
}