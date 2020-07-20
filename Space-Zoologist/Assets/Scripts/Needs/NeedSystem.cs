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
    
    protected List<Life> Consumers = new List<Life>();

    public NeedSystem(string needName)
    {
        NeedName = needName;
    }

    virtual public void AddConsumer(Life life)
    {
        this.isDirty = true;
        this.Consumers.Add(life);
    }

    virtual public bool RemoveConsumer(Life life)
    {
        this.isDirty = true;
        return this.Consumers.Remove(life);
    }

    abstract public void UpdateSystem();
}