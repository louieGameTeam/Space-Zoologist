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

    public Atmosphere Copy(Atmosphere from){
        gasX = from.gasX;
        gasY = from.gasY;
        gasZ = from.gasZ;
        temperature = from.temperature;
        return this;
    }
}
