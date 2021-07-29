using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO refactor out temperature
public enum AtmosphereComponent { GasX, GasY, GasZ, Temperature };

/// <summary>
/// A class that represents the atmospheric composition of an area.
/// </summary>
[System.Serializable]
public class AtmosphericComposition
{
    [SerializeField] public float GasX = 0.0f;
    [SerializeField] public float GasY = 0.0f;
    [SerializeField] public float GasZ = 0.0f;
    [SerializeField] private float temperature = 0.0f;

    public float Temperature { get => temperature; }

    public AtmosphericComposition()
    {
        GasX = GasY = GasZ = temperature = 0;
    }

    public AtmosphericComposition(float _GasX, float _GasY, float _GasZ, float _temperature)
    {
        GasX = _GasX;
        GasY = _GasY;
        GasZ = _GasZ;
        temperature = _temperature;
    }

    public AtmosphericComposition(AtmosphericComposition from)
    {
        GasX = from.GasX; GasY = from.GasY; GasZ = from.GasZ; temperature = from.temperature;
    }

    public AtmosphericComposition Copy(AtmosphericComposition from)
    {
        GasX = from.GasX;
        GasY = from.GasY;
        GasZ = from.GasZ;
        temperature = from.temperature;
        return this;
    }

    public static AtmosphericComposition operator +(AtmosphericComposition lhs, AtmosphericComposition rhs)
    {
        return new AtmosphericComposition((lhs.GasX + rhs.GasX) / 2.0f, (lhs.GasY + rhs.GasY) / 2.0f,
            (lhs.GasZ + rhs.GasZ) / 2.0f, (lhs.temperature + rhs.temperature) / 2.0f);
    }

    public static AtmosphericComposition Merge(AtmosphericComposition atmOne, float atmOneSize, AtmosphericComposition atmTwo, float atmTwoSize) {
        float fraction = atmOneSize / (atmOneSize + atmTwoSize);
        return new AtmosphericComposition(
            atmOne.GasX * fraction + atmTwo.GasX * (1 - fraction),
            atmOne.GasY * fraction + atmTwo.GasY * (1 - fraction),
            atmOne.GasZ * fraction + atmTwo.GasZ * (1 - fraction),
            atmOne.Temperature * fraction + atmTwo.Temperature * (1 - fraction)
            );
    }

    public static KeyValuePair<AtmosphericComposition, float> Merge(KeyValuePair<AtmosphericComposition, float> atmOne, KeyValuePair<AtmosphericComposition, float> atmTwo)
    {
        return new KeyValuePair<AtmosphericComposition, float>( Merge(atmOne.Key,atmOne.Value,atmTwo.Key,atmTwo.Value), atmOne.Value + atmTwo.Value );
    }


    public static AtmosphericComposition Merge(Dictionary<AtmosphericComposition, float> compositions)
    {
        KeyValuePair<AtmosphericComposition, float> result = new KeyValuePair<AtmosphericComposition, float>(new AtmosphericComposition(), 0);
        foreach (var composition in compositions) {
            result = Merge(result, composition);
        }

        return result.Key;
    }

    public override string ToString()
    {
        return "GasX = " + GasX + " GasY = " + GasY + " GasZ = " + GasZ + " Temp = " + temperature;
    }

    /// <summary>
    /// Get the composition of the atmoshpere including temerature, in the order of AtmoshpereComponent enum
    /// </summary>
    /// <returns></returns>
    public float[] GetComposition()
    {
        float[] composition = { GasX, GasY, GasZ, temperature };
        return composition;
    }

    public float[] ConvertAtmosphereComposition()
    {
        float[] composition = { GasX, GasY, GasZ };
        return composition;
    }
}

