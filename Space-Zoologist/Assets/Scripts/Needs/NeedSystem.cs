using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Abstract class that all NeedSystems will inherit from. Every need system will have a list of populations 
/// that have the need that the need system is in charge of, and keeps this need up to date for all of its 
/// populations.
/// </summary>
abstract public class NeedSystem
{
    public string NeedName { get; private set; }
    public bool IsDirty => this.isDirty;

    protected bool isDirty = default;
    protected List<Life> Consumers = new List<Life>();

    protected Dictionary<FoodSource, int[]> FoodSourceAcceiableTerrain = new Dictionary<FoodSource, int[]>();

    public NeedSystem(string needName)
    {
        NeedName = needName;
    }

    public void MarkAsDirty()
    {
        this.isDirty = true;
    }

    virtual public void AddConsumer(Life life)
    {
        this.isDirty = true;
        this.Consumers.Add(life);

        if(life.GetType() == typeof(FoodSource))
        {
            if (!this.FoodSourceAcceiableTerrain.ContainsKey((FoodSource)life))
            {
                this.FoodSourceAcceiableTerrain.Add((FoodSource)life, TileSystem.ins.CountOfTilesInRange(Vector3Int.FloorToInt(((FoodSource)life).GetPosition()), ((FoodSource)life).Species.RootRadius));
            }
        }
    }

    virtual public bool RemoveConsumer(Life life)
    {
        this.isDirty = true;
        return this.Consumers.Remove(life);
    }

    // Check state has changed
    virtual public bool CheckState()
    {
        foreach (Life consumer in this.Consumers)
        {
            if(consumer.GetType() == typeof(Population))
            {
                if (ReservePartitionManager.ins.PopulationAccessbilityStatus[(Population)consumer])
                {
                    return true;
                }
            }
            else if(consumer.GetType() == typeof(FoodSource))
            {
                // TODO: Check if a food source's accessiable terrain has changed
                //return true;

                var preTerrain = this.FoodSourceAcceiableTerrain[(FoodSource)consumer];
                var curTerrain = TileSystem.ins.CountOfTilesInRange(Vector3Int.FloorToInt(((FoodSource)consumer).GetPosition()), ((FoodSource)consumer).Species.RootRadius);

                if (!preTerrain.SequenceEqual(curTerrain))
                {
                    this.FoodSourceAcceiableTerrain[(FoodSource)consumer] = curTerrain;
                    return true;
                }
            }
            else
            {
                Debug.Assert(true, "Consumer type error");
            }
        }

        return false;
    }

    abstract public void UpdateSystem();
}