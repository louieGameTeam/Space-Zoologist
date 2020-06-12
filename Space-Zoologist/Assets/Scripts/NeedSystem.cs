using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class that all NeedSystems will inherit from. Every need system will have a list of populations 
/// that have the need that the need system is in charge of, and keeps this need up to date for all of its 
/// populations.
/// </summary>
abstract public class NeedSystem
{
    public string NeedName { get; private set; }
    public bool isDirty = default;
    protected List<Population> populations = new List<Population>();
    protected List<FoodSource> foodSources = new List<FoodSource>();

    public NeedSystem(string needName)
    {
        NeedName = needName;
    }

    virtual public void AddPopulation(Population population)
    {
        isDirty = true;
        populations.Add(population);
    }

    virtual public bool RemovePopulation(Population population)
    {
        isDirty = true;
        return populations.Remove(population);
    }

    virtual public void AddFoodSource(FoodSource foodSource)
    {
        foodSources.Add(foodSource);
        isDirty = true;
    }

    virtual public bool RemoveFoodSource(FoodSource foodSource)
    {
        return foodSources.Remove(foodSource);
    }

    abstract public void UpdateSystem();
}
