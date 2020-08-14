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
public class SymbiosisNeedSystem : NeedSystem
{
    private List<Population> populations = new List<Population>();
    private readonly ReservePartitionManager rpm = null;

    // Species name to food calculators
    private Dictionary<string, SymbiosisCalculator> symbiosisCalculators = new Dictionary<string, SymbiosisCalculator>();

    public SymbiosisNeedSystem(ReservePartitionManager rpm, NeedType needType = NeedType.Symbiosis) : base(needType)
    {
        this.rpm = rpm;
    }

    public override bool CheckState()
    {
        bool needUpdate = false;

        foreach (SymbiosisCalculator symbiosisCalculator in this.symbiosisCalculators.Values)
        {
            // Check if consumer is dirty
            foreach (Population consumer in symbiosisCalculator.Consumers)
            {
                if (consumer.GetAccessibilityStatus() || consumer.Count != consumer.PrePopulationCount)
                {
                    symbiosisCalculator.MarkDirty();
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
                foreach (Population population in symbiosisCalculator.Populations)
                {
                    if (population.GetAccessibilityStatus() || population.Count != population.PrePopulationCount)
                    {
                        symbiosisCalculator.MarkDirty();
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
        if (!this.symbiosisCalculators.ContainsKey(population.Species.SpeciesName))
        {
            this.symbiosisCalculators.Add(population.Species.SpeciesName, new SymbiosisCalculator(rpm, population.Species.SpeciesName));
        }

        this.symbiosisCalculators[population.Species.SpeciesName].AddSource(population);

        this.isDirty = true;
    }

    public override void AddConsumer(Life life)
    {
        foreach (Need need in life.GetNeedValues().Values)
        {
            // Check if the need is a 'Species' type
            if (need.NeedType == NeedType.Symbiosis)
            {
                // Create a food source calculator for this food source,
                // if not already exist
                if (!this.symbiosisCalculators.ContainsKey(need.NeedName))
                {
                    this.symbiosisCalculators.Add(need.NeedName, new SymbiosisCalculator(rpm, need.NeedName));
                }

                // Add consumer to food source calculator
                this.symbiosisCalculators[need.NeedName].AddConsumer((Population)life);
            }
        }

        this.isDirty = true;
    }

    public override void MarkAsDirty()
    {
        base.MarkAsDirty();

        foreach (SymbiosisCalculator SymbiosisCalculator in this.symbiosisCalculators.Values)
        {
            SymbiosisCalculator.MarkDirty();
        }
    }

    public override void UpdateSystem()
    {
        foreach (SymbiosisCalculator SymbiosisCalculator in this.symbiosisCalculators.Values)
        {
            if (SymbiosisCalculator.IsDirty)
            {
                var distribution = SymbiosisCalculator.CalculateDistribution();

                // distibution returns null when not thing to be updated
                if (distribution != null)
                {
                    foreach (Population consumer in distribution.Keys)
                    {
                        consumer.UpdateNeed(SymbiosisCalculator.SpeciesName, distribution[consumer]);
                    }
                }
            }
        }

        this.isDirty = false;
    }
}