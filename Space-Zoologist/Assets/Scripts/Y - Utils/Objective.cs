using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectiveStatus { Completed, InProgress, Failed }

/// <summary>
/// Parent class of all types of abjective
/// </summary>
public abstract class Objective
{
    public abstract ObjectiveStatus Status { get; }
    public abstract ObjectiveStatus UpdateStatus();
    public abstract string GetObjectiveText();
}

/// <summary>
/// Objective type to keep a population of some size for some time
/// </summary>
public class SurvivalObjective : Objective
{
    public List<Population> Populations = default;
    public AnimalSpecies AnimalSpecies { get; private set; }
    public byte TargetPopulationCount { get; private set; }
    public byte TargetPopulationSize { get; private set; }
    public byte satisfiedPopulationCount { get; private set; }
    public int totalPopulationCount { get; private set; }
    public float TargetTime { get; private set; }

    public float timer { get; private set; }
    private ObjectiveStatus status;

    public override ObjectiveStatus Status => this.status;

    public SurvivalObjective(AnimalSpecies animalSpecies, byte targetPopulationCount, byte targetPopulationSize, float targetTime)
    {
        this.Populations = new List<Population>();
        this.AnimalSpecies = animalSpecies;
        this.TargetPopulationCount = targetPopulationCount;
        this.TargetPopulationSize = targetPopulationSize;
        this.TargetTime = targetTime;
        this.status = ObjectiveStatus.InProgress;
    }

    public override ObjectiveStatus UpdateStatus()
    {
        satisfiedPopulationCount = 0;
        totalPopulationCount = 0;

        foreach (Population population in this.Populations)
        {
            totalPopulationCount += population.Count;

            // Found a population that has enough pop count
 /*           if (population.Count >= this.TargetPopulationSize)
            {
                satisfiedPopulationCount++;
            }*/

            /*
             * Note - while TargetPopulationCount exists, the intended design is
             * to calculate objective completion based on totalPopulationCount
            */

            // Have met the population number requirement
            if (totalPopulationCount >= this.TargetPopulationSize)
            {

                if (this.timer >= this.TargetTime)
                {
                    this.status = ObjectiveStatus.Completed;
                    return ObjectiveStatus.Completed;
                }
                else
                {
                    this.timer += Time.deltaTime;
                }

                break;
            }
            // reset timer if requirement not met
            else
            {
                this.timer = 0f;
            }
        }
        this.status = ObjectiveStatus.InProgress;
        return ObjectiveStatus.InProgress;
    }

    public override string GetObjectiveText()
    {

        string displayText = "";
        string population = "population";
        string timeLabel = "minute";
        float targetTime = this.TargetTime / 60f;
        if (this.TargetPopulationCount > 1)
        {
            population += "s";
        }
        if (!(this.TargetTime <= 120f))
        {
            timeLabel += "s";
        }
        if (this.TargetTime < 60f)
        {
            targetTime = this.TargetTime;
            timeLabel = "seconds";
        }
        if (this.TargetTime.Equals(0f))
        {
            displayText += $"Reach a population size of {this.TargetPopulationSize} {this.AnimalSpecies.ID.Data.Name.Get(ItemName.Type.English)}s\n\n";
            displayText += $"Current population size: {totalPopulationCount}\n\n";
            return displayText;
        }
        displayText += $"Maintain at least {this.satisfiedPopulationCount}/{this.TargetPopulationCount} ";
        displayText += $"{this.AnimalSpecies.ID.Data.Name.Get(ItemName.Type.English)} {population} with a count of {this.TargetPopulationSize}";
        displayText += $" for {targetTime} {timeLabel} ";
        displayText += $"[{this.Status.ToString()}] [{Math.Round(this.timer, 0)}/{this.TargetTime}]\n";

        return displayText;
    }
}

/// <summary>
/// Objective to have x amount of money left when level is completed 
/// </summary>
public class ResourceObjective : Objective
{
    public int amountToKeep { get; private set; }

    public override ObjectiveStatus Status => this.status;

    private ObjectiveStatus status;

    public ResourceObjective(int amountToKeep)
    {
        this.amountToKeep = amountToKeep;
        this.status = ObjectiveStatus.InProgress;
    }

    public override ObjectiveStatus UpdateStatus()
    {
        if (GameManager.Instance.Balance >= this.amountToKeep)
        {
            this.status = ObjectiveStatus.Completed;
        }
        else
        {
            this.status = ObjectiveStatus.Failed;
        }

        return this.status;
    }

    public override string GetObjectiveText()
    {
        return $"Have at least ${this.amountToKeep} left when level is complete [{this.status}]\n";
    }
}