using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeciesTrace
{
    // The name of the species being traced.
    [SerializeField] private string name;
    // A dictionary that takes as keys the integer day and takes as values a 3-tuple containing quality of food, water, and terrain respectively.
    [SerializeField] private Dictionary<int, Tuple<float, float, float>> quality;
    // A dictionary that takes as keys the integer day and takes as values a 3-tuple containing quantity of food, water, and terrain respectively.
    [SerializeField] private Dictionary<int, Tuple<float, float, float>> quantity;
    // A boolean indicating whether this species thriving needs were met in the level.
    [SerializeField] private bool thrived;
    // A boolean indicating whether this species was in decline in the level.
    [SerializeField] private bool declined;
    // The amount of time this species spent thriving.
    [SerializeField] private float thriveTime;
    // The amount of time this species spent in decline.
    [SerializeField] private float declineTime;

    // PUBLIC GETTERS/SETTERS
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public Dictionary<int, Tuple<float, float, float>> Quality
    {
        get { return quality; }
        set { quality = value; }
    }

    public Dictionary<int, Tuple<float, float, float>> Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }

    public bool Thrived
    {
        get { return thrived; }
        set { thrived = value; }
    }

    public bool Declined
    {
        get { return declined; }
        set { declined = value; }
    }

    public float ThriveTime
    {
        get { return thriveTime; }
        set { thriveTime = value; }
    }

    public float DeclineTime
    {
        get { return declineTime; }
        set { declineTime = value; }
    }
}
