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
    protected List<Population> populations = new List<Population>();

    public NeedSystem(string needName)
    {
        NeedName = needName;
    }

    virtual public void AddPopulation(Population population)
    {
        populations.Add(population);
    }

    virtual public bool RemovePopulation(Population population)
    {
        return populations.Remove(population);
    }
    abstract public void UpdateSystem();
}
