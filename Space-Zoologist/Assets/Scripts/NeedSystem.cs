using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
