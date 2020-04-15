using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simple atmosphere class, not used yet.
//TODO to be expanded.
public class Atmosphere
{
    float gasX, gasY, gasZ, temperature;
    public float GasX { get; private set; }

    public Atmosphere() {
        gasX = gasY = gasZ = temperature = 0;
    }

    public Atmosphere(float x, float y, float z, float t) {
        gasX = x; gasY = y; gasZ = z; temperature = t;
    }

    public Atmosphere(Atmosphere from)
    {
        gasX = from.gasX; gasY = from.gasY; gasZ = from.gasZ; temperature = from.temperature;
    }

    public Atmosphere Copy(Atmosphere from){
        gasX = from.gasX;
        gasY = from.gasY;
        gasZ = from.gasZ;
        temperature = from.temperature;
        return this;
    }

    public static Atmosphere operator +(Atmosphere lhs, Atmosphere rhs) {
        return new Atmosphere(lhs.gasX + rhs.gasX, lhs.gasY + rhs.gasY, lhs.gasZ + rhs.gasZ, lhs.temperature + rhs.temperature);
    }
}
